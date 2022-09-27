using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    NetworkManager networkManager;
    GameController gameController;

    [SerializeField] private Transform camera;
    float mouseSensitivity = 2f;
    private float upLimit = -10f;
    private float downLimit = 30f;
    [SerializeField] private float speed = 3f;
    private Vector3 moveDirection = Vector3.zero;

    public NetworkVariable<Vector3> networkPos = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> networkRot = new NetworkVariable<Quaternion>();
    public NetworkVariable<Color> networkPlayerColor = new NetworkVariable<Color>();

    ulong localClientId;

    NetworkVariable<int> netSpawnID = new NetworkVariable<int>();
    int spawnID;

    private void Awake()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public ulong GetLocalClientId()
    {
        return localClientId;
    }

    public void Move()
    {
        if (IsOwner)
        {
            /*Vector3 moveDir = Vector3.Normalize(transform.forward * Input.GetAxis("Vertical")) * 3;
            transform.position += moveDir * Time.deltaTime;

            transform.Rotate(0, Input.GetAxis("Horizontal") * 20 * Time.deltaTime, 0);*/

            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                GetComponent<Animator>().SetBool("walk", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("walk", false);
            }

            moveDirection = new Vector3(-Input.GetAxis("Vertical"), 0, 0);
            moveDirection = transform.TransformDirection(moveDirection);

            transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * speed * 50);

            transform.position += moveDirection * Time.deltaTime * speed;

            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            else if (Input.GetMouseButton(1))
            {
                float horizontalRotation = Input.GetAxis("Mouse X");
                float verticalRotation = Input.GetAxis("Mouse Y");

                transform.Rotate(0, horizontalRotation * mouseSensitivity, 0);
                camera.Rotate(-verticalRotation * mouseSensitivity, 0, 0);

                Vector3 currentRotation = camera.localEulerAngles;
                if (currentRotation.x > 180)
                    currentRotation.x -= 360;
                currentRotation.x = Mathf.Clamp(currentRotation.x, upLimit, downLimit);
                camera.localRotation = Quaternion.Euler(currentRotation);
            }

            else if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            ///////////////////////////////////////////////////

            if (IsServer)
            {
                networkPos.Value = transform.position;
                networkRot.Value = transform.rotation;
            }
            else
            {
                MoveServerRpc(transform.position);
                RotateServerRpc(transform.rotation);
            }   
        }
        else
        {
            transform.position = networkPos.Value;
            transform.rotation = networkRot.Value;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsOwner && IsClient)
        {
            localClientId = networkManager.LocalClientId;

            SetLocalClientIdServerRpc(localClientId);

            SetPlayerColorServerRpc(GameObject.FindGameObjectWithTag("Canca").GetComponent<MenuScript>().targetPlayerColor);

            spawnID = netSpawnID.Value;
        }

        if(!IsOwner)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        
        if(IsServer)
        {
            spawnID = gameController.AddPlayer(this);
            networkPos.Value = gameController.GetSpawnPointPosition(spawnID);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if(IsServer)
        {
            gameController.RemovePlayer(spawnID);
        }
    }

    [ServerRpc]
    void SetPlayerColorServerRpc(Color playerColor)
    {
        networkPlayerColor.Value = playerColor;
    }

    [ServerRpc]
    void MoveServerRpc(Vector3 pos)
    {
         networkPos.Value = pos;
    }

    [ServerRpc]
    void RotateServerRpc(Quaternion rot)
    {
        networkRot.Value = rot;
    }

    [ServerRpc]
    void SetLocalClientIdServerRpc(ulong p_playerId)
    {
        localClientId = p_playerId;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        GetComponentInChildren<PlayerColor>().materialColor = networkPlayerColor.Value;
    }
}
