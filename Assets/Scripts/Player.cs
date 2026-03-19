using UnityEngine;
using Unity.Netcode;
using MapData;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// Route is for connecting neighbors
// first two is start and end
// last three is total cost, tunnel and boat
using Route = System.Tuple<MapData.StationName, MapData.StationName, int, int, int>;

public class Player : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the NetworkBehaviour is created
    void Start()
    {
        mouseLeftClick = InputSystem.actions.FindAction("Click");
        if (mouseLeftClick != null )
        {
            mouseLeftClick.performed += OnMouseClickAction;
        }

        keyboardWASD = InputSystem.actions.FindAction("Move");
        if ( keyboardWASD != null )
        {
            keyboardWASD.performed += OnKeyboardWASDAction;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        }
    }

    private void OnKeyboardWASDAction(InputAction.CallbackContext obj)
    {
        Vector2 vector2 = obj.ReadValue<Vector2>();
        Debug.Log("Keyboard input: " + vector2.ToString());
    }

    private HashSet<StationName> m_connectedStations;
    private int m_remainingCarts = 45;
    private HandDeck m_handDeck;

    // input actions
    private InputAction mouseLeftClick;
    private InputAction mouseRightClick;
    private InputAction keyboardWASD;
}
