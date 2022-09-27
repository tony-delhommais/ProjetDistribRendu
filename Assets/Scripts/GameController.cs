using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

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
    [SerializeField]
    GameObject winAreaPrefab;

    GameObject winAreaInstance;

    NetworkList<ScoreMapping> playerScore;

    private void Awake()
    {
        playerScore = new NetworkList<ScoreMapping>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!winAreaPrefab)
            Debug.LogError("WinAreaPrefab not set");
    }

    bool PlayerIdExistInScoreList(ulong p_playerId)
    {
        for(int i = 0; i < playerScore.Count; i++)
        {
            if (playerScore[i].PlayerId == p_playerId)
                return true;
        }

        return false;
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

    void Update()
    {
        if (IsServer)
        {
            if (!winAreaInstance)
            {
                winAreaInstance = Instantiate(winAreaPrefab);

                winAreaInstance.transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

                winAreaInstance.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
