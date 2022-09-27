using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{

    NetworkManager networkManager;

    public void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    //void Update()
    //{
    //    if (IsServer)
    //    {
    //        float theta = Time.frameCount / 10.0f;
    //        transform.position = new Vector3((float)Math.Cos(theta), 0.0f, (float)Math.Sin(theta));
    //    }

    //}
    public void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;
        networkManager.LocalClient.PlayerObject.transform.position += move * Time.deltaTime;
    }
    void Update()
    {
        if (IsClient && !IsServer)
            Move();
    }
}