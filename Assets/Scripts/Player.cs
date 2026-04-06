
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Route is for connecting neighbors
// first two is start and end
// last three is total cost, tunnel and boat
using Route = System.Tuple<StationName, StationName, int, int, int>;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

enum CamDirection
{
    Forward,
    Backward,
    Left,
    Right,
    None
}

public class Player : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the NetworkBehaviour is created
    void Start()
    {
        mouseLeftClick = InputSystem.actions.FindAction("Click");
        if (mouseLeftClick != null)
        {
            mouseLeftClick.performed += OnMouseClickAction;
        }

        keyboardWASD = InputSystem.actions.FindAction("Move");
        if (keyboardWASD != null)
        {
            keyboardWASD.performed += OnKeyboardWASDAction;
            keyboardWASD.canceled += OnWASDReleased;
        }

        mouseRightClick = InputSystem.actions.FindAction("RightHold");
        if (mouseRightClick != null)
        {
            mouseRightClick.performed += OnRightMousePressed;
            mouseRightClick.canceled += OnRightMouseReleased;
        }

        mouseMove = InputSystem.actions.FindAction("Look");
        if (mouseMove != null)
        {
            mouseMove.performed += OnMouseMove;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Camera movement
        Camera maimCam = Camera.main;

        Vector3 cameraFront = maimCam.transform.forward;
        Vector3 cameraRight = maimCam.transform.right;

        maimCam.transform.position += (cameraFront * kbInput.y + cameraRight * kbInput.x) * Time.deltaTime * 10.0f;
    }

    public bool BuildRoute(Route p_route)
    {
        int cost = p_route.Item3;
        return true;
    }

    public int GetHandCardCount(CardColor p_color)
    {
        return m_handDeck.GetCardCount(p_color);
    }

    public void AddHandCard(CardColor p_color, int p_numOfNewCards = 1)
    {
        m_handDeck.AddCard(p_color, p_numOfNewCards);
    }

    public bool CheckHandCard(CardColor p_color, int p_numOfCardsToCheck = 1)
    {
        return m_handDeck.CheckCardSufficient(p_color, p_numOfCardsToCheck);
    }

    public bool RemoveHandCard(CardColor p_color, int p_numOfCardsToRemove = 1)
    {
        return m_handDeck.ChechAndUseCard(p_color, p_numOfCardsToRemove);
    }

    public void ResetHandDeck()
    {
        m_handDeck.ResetHandDeck();
    }

    public bool QueueEvent()
    {
        // false means no event at hand
        if (m_nextEvent == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool AddEvent(EventBase newEvent)
    {
        if (m_nextEvent == null)
        {
            m_nextEvent = newEvent;
            return true;
        }
        else
        {
            Debug.LogError("Already have an event at hand, cannot add new event");
            // add failed
            return false;
        }
    }

    public void HandleEvent()
    {
        if (m_nextEvent != null)
        {
            // do some stuff like switch-case
            m_nextEvent = null;
        }
    }

    private void OnMouseClickAction(InputAction.CallbackContext obj)
    {
        // do stuff
        Vector2 vector2 = Mouse.current.position.ReadValue();
        Ray rayOrigin = Camera.main.ScreenPointToRay(vector2);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, out hit))
        {
            Station hitStation = hit.collider.gameObject.GetComponent<Station>();
            if (hitStation != null)
            {
                Debug.Log("Clicked on station: " + hitStation.m_name.ToString());
            }

            Connection hitConnection = hit.collider.gameObject.GetComponent<Connection>();
            if (hitConnection != null)
            {
                // pop out UI to ask if player want to build route here
                Debug.Log("Clicked on connection: " + hitConnection.m_end1.ToString() + " - " + hitConnection.m_end2.ToString());
            }


        }
    }

    private void OnKeyboardWASDAction(InputAction.CallbackContext obj)
    {
        kbInput = obj.ReadValue<Vector2>();
    }

    private void OnWASDReleased(InputAction.CallbackContext obj)
    {
        kbInput = Vector2.zero;
    }

    private void OnRightMousePressed(InputAction.CallbackContext obj)
    {
        isRightMouseHold = true;
        Debug.Log("Right mouse clicked");
    }

    private void OnRightMouseReleased(InputAction.CallbackContext obj)
    {
        isRightMouseHold = false;
        Debug.Log("Right mouse released");
    }

    private void OnMouseMove(InputAction.CallbackContext obj)
    {
        if (isRightMouseHold)
        {
            // x+: right, x-: left, y+: up, y-: down
            Vector2 vector2 = obj.ReadValue<Vector2>();
            Camera.main.transform.SetPositionAndRotation(Camera.main.transform.position, Quaternion.Euler(
                Camera.main.transform.rotation.eulerAngles + new Vector3(-vector2.y, vector2.x, 0) * Time.deltaTime * 100.0f));
        }
    }

    private HashSet<StationName> m_connectedStations;
    private int m_remainingCarts = 45;
    private HandDeck m_handDeck;

    // input actions
    private InputAction mouseLeftClick;
    private InputAction mouseRightClick;
    private InputAction keyboardWASD;
    private InputAction mouseMove;

    // Camera control
    private bool isRightMouseHold = false;
    private Vector2 kbInput = Vector2.zero;

    // Event
    private EventBase m_nextEvent = null;
}
