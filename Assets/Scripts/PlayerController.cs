using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<Vector3> networkPos = new NetworkVariable<Vector3>();

    ulong localClientId;

    public ulong GetLocalClientId()
    {
        return localClientId;
    }

    public void Move()
    {
        if (IsOwner)
        {
            Vector3 moveDir = Vector3.Normalize(transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * 3;
            transform.position += moveDir * Time.deltaTime;


            if (IsServer)
            {
                networkPos.Value = transform.position;
            }
            else
            {
                MoveServerRpc(transform.position);
            }   
        }
        else
        {
            transform.position = networkPos.Value;
        }
    }

    [ServerRpc]
    void MoveServerRpc(Vector3 pos)
    {
         networkPos.Value = pos;
    }

    [ServerRpc]
    void SetLocalClientIdServerRpc(ulong p_clientId)
    {
        localClientId = p_clientId;
    }

    private void Start()
    {
        if(IsOwner && IsClient)
        {
            localClientId = NetworkManager.LocalClientId;

            SetLocalClientIdServerRpc(localClientId);
        }
    }


    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
