using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    NetworkManager networkManager;
    GameController gameController;

    public GameObject regleInfoMenu;
    bool regleInfoMenuIsActive = false;

    bool onMenu1 = true;
    public GameObject Menu1;
    bool onMenu2 = false;
    public GameObject Menu2;
    bool onGame = false;

    public GameObject Display1;
    public GameObject Display2;
    public GameObject DisplayGame;

    public GameObject StartButton;

    bool clientWantHost = true;

    public Color targetPlayerColor = new Color(1f, 0f, 0f, 1f);

    private void Awake()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start()
    {
        

        if (Menu1) Menu1.SetActive(onMenu1);
        if (Menu2) Menu2.SetActive(onMenu2);

        if (Display1) Display1.SetActive(onMenu1);
        if (Display2) Display2.SetActive(onMenu2);
        if (DisplayGame) DisplayGame.SetActive(onGame);
    }

    public void SetRegleInfoMenu(bool state)
    {
        if (!regleInfoMenu)
            return;

        if (regleInfoMenuIsActive)
            return;

        regleInfoMenu.SetActive(state);

        regleInfoMenuIsActive = state;
    }

    public void ResetRegleInfoMenu()
    {
        if (!regleInfoMenu)
            return;

        regleInfoMenu.SetActive(false);

        regleInfoMenuIsActive = false;
    }

    public void SetGameSelected(bool isHostSelected)
    {
        if (!onMenu1)
            return;

        if (regleInfoMenuIsActive)
            return;

        clientWantHost = isHostSelected;

        onMenu1 = false;
        if (Menu1)
            Menu1.SetActive(onMenu1);
        if (Display1)
            Display1.SetActive(onMenu1);

        onMenu2 = true;
        if (Menu2)
            Menu2.SetActive(onMenu2);
        if (Display2)
            Display2.SetActive(onMenu2);
    }

    public void StartGame()
    {
        if (!onMenu2)
            return;

        onMenu2 = false;
        if (Menu2)
            Menu2.SetActive(onMenu2);
        if (Display2)
            Display2.SetActive(onMenu2);

        onGame = true;
        if(DisplayGame)
            DisplayGame.SetActive(onGame);

        if (clientWantHost)
        {
            networkManager.StartHost();
        }
        else
        {
            networkManager.StartClient();
        }
    }
}
