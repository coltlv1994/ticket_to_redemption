using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class MenuScript : MonoBehaviour
{
    [SerializeField] bool isMainMenu;

    [SerializeField] GameObject MainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isMainMenu)
        {
            MainMenu.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameSP()
    {
        MainMenu.SetActive(false);

        PlayerController.GetInstance().ChangeState(new PlayState());
    }
}
