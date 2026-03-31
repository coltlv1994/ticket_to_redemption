using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController GetInstance()
    {
        return m_instance;
    }
    
    public void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        m_stateMachine.Init();
    }

    public void ChangeState(StateBase newState)
    {
        m_stateMachine.ChangeState(newState);
    }   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeState(new IntroState());
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.Update();
    }

    private StateMachine m_stateMachine = new StateMachine();
    private static PlayerController m_instance;
}
