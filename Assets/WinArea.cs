using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]  

public class WinArea : NetworkBehaviour
{
    GameController gameController;

    public NetworkVariable<Vector3> areaPosition = new NetworkVariable<Vector3>();
    
    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(IsOwner && IsServer)
        {
            
            if(other.gameObject.tag == "Player")
            {
                gameController.PlayerCollideWithWinArea(other.gameObject.GetComponent<PlayerController>().GetLocalClientId());
            }
        }
    }

    private void Update()
    {
        if(IsServer)
        {
            areaPosition.Value = transform.position;
        }
        else
        {
            transform.position = areaPosition.Value;
        }
    }
}
