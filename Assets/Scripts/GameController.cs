using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

public struct ScoreMapping : INetworkSerializable, System.IEquatable<ScoreMapping>
{
    public ulong PlayerId;
    public int PlayerScore;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if(serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out PlayerId);
            reader.ReadValueSafe(out PlayerScore);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(PlayerId);
            writer.WriteValueSafe(PlayerScore);
        }
    }

    public bool Equals(ScoreMapping other)
    {
        return PlayerId == other.PlayerId && PlayerScore == other.PlayerScore;
    }
}

public class GameController : NetworkBehaviour
{
    NetworkManager networkManager;

    [SerializeField]
    GameObject winAreaPrefab;

    GameObject winAreaInstance;

    NetworkList<ScoreMapping> playerScore;

    GameObject[] spawnPointList;

    PlayerController[] playerControllers;


    private void Awake()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();

        playerScore = new NetworkList<ScoreMapping>();

        spawnPointList = GameObject.FindGameObjectsWithTag("SpawnPoint");

        playerControllers = new PlayerController[spawnPointList.Length];
        for (int i = 0; i < spawnPointList.Length; i++)
            playerControllers[i] = null;
    }

    void Start()
    {
        if (!winAreaPrefab)
            Debug.LogError("WinAreaPrefab not set");
    }

    public int AddPlayer(PlayerController playerController)
    {
        int i;
        for (i = 0; i < playerControllers.Length; i++)
        {
            if (!playerControllers[i])
                break;
        }

        if (i == playerControllers.Length)
            return -1;

        playerControllers[i] = playerController;
        return i;
    }

    public void RemovePlayer(int id)
    {
        if (id < 0 || id >= playerControllers.Length)
            return;

        playerControllers[id] = null;
    }

    public Vector3 GetSpawnPointPosition(int p_spawnPointId)
    {
        if(p_spawnPointId < 0 || p_spawnPointId >= spawnPointList.Length)
            p_spawnPointId = 0;

        return spawnPointList[p_spawnPointId].transform.position;
    }

    void IncrementPlayerScore(ulong p_playerId)
    {
        int index = -1;
        int oldScore = 0;

        for (int i = 0; i < playerScore.Count; i++)
        {
            if (playerScore[i].PlayerId == p_playerId)
            {
                index = i;

                oldScore = playerScore[i].PlayerScore;

                break;
            }
        }

        if (index != -1)
            playerScore.RemoveAt(index);

        ScoreMapping mapping = new ScoreMapping();
        mapping.PlayerId = p_playerId;
        mapping.PlayerScore = oldScore + 1;

        playerScore.Add(mapping);
    }

    public int GetScoreFromClientId(ulong p_playerId)
    {
        for (int i = 0; i < playerScore.Count; i++)
        {
            if (playerScore[i].PlayerId == p_playerId)
                return playerScore[i].PlayerScore;
        }

        return 0;
    }

    public void PlayerCollideWithWinArea(ulong p_playerId)
    {
        if(IsServer)
        {
            Debug.Log(p_playerId);

            IncrementPlayerScore(p_playerId);

            winAreaInstance.GetComponent<NetworkObject>().Despawn();
            Destroy(winAreaInstance);

            winAreaInstance = null;
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            if (!winAreaInstance && winAreaPrefab)
            {
                winAreaInstance = Instantiate(winAreaPrefab);

                winAreaInstance.transform.position = RandomNavmeshLocation(100f);

                winAreaInstance.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
