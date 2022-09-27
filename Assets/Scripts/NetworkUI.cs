using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]

public class NetworkUI : MonoBehaviour
{
    NetworkManager networkManager;
    GameController gameController;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 100));

        if(networkManager.IsClient)
        {
            GUILayout.Label(gameController.GetScoreFromClientId(networkManager.LocalClientId).ToString());
        }

        GUILayout.EndArea();
    }
}
