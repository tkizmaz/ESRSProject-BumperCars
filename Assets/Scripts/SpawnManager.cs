using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager instance;
    public List<Transform> spawnPoints;
    public GameObject playerPrefab;
    private NetworkList<bool> isSpawnPointUsed = new NetworkList<bool>();
    private NetworkList<int> playerSpawnIndices = new NetworkList<int>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
                isSpawnPointUsed.Add(false);

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        int spawnIndex = FindAvailableSpawnIndex();
        if (spawnIndex == -1)
        {
            Debug.LogError("All spawn points are used.");
            return;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        if (!networkObject.IsSpawned)
        {
            networkObject.SpawnAsPlayerObject(clientId);
            isSpawnPointUsed[spawnIndex] = true;
            playerSpawnIndices.Add(spawnIndex);
        }
    }

    private int FindAvailableSpawnIndex()
    {
        for (int i = 0; i < isSpawnPointUsed.Count; i++)
            if (!isSpawnPointUsed[i])
                return i;
        return -1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnAllPlayersServerRpc()
    {
        if (!IsServer)
        {
            Debug.LogError("RespawnAllPlayersServerRpc is called on a client.");
            return;
        }

        Debug.Log("Server: Resetting spawn points and respawning all players.");
        ResetSpawnPoints();
        foreach (var clientPair in NetworkManager.Singleton.ConnectedClients)
        {
            var player = clientPair.Value.PlayerObject;
            if (player != null)
            {
                int spawnIndex = FindAvailableSpawnIndex();
                if (spawnIndex != -1)
                {
                    Transform spawnPoint = spawnPoints[spawnIndex];
                    Debug.Log($"Server: Calling UpdatePlayerClientRpc for client {clientPair.Key} to respawn at index {spawnIndex}.");
                    UpdatePlayerClientRpc(clientPair.Key, spawnPoint.position, spawnPoint.rotation);
                }
                else
                {
                    Debug.LogError("Server: No available spawn points.");
                }
            }
            else
            {
                Debug.LogError($"Server: No player object found for client {clientPair.Key}");
            }
        }
    }




    [ClientRpc]
    private void UpdatePlayerClientRpc(ulong clientId, Vector3 position, Quaternion rotation)
    {
        Debug.Log($"RPC called for {clientId}, Local Client ID: {NetworkManager.Singleton.LocalClientId}");
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            var player = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (player != null)
            {
                player.transform.position = position;
                player.transform.rotation = rotation;
                player.gameObject.SetActive(true);
                Debug.Log($"Client: Player {clientId} reactivated at position {position}");
            }
        }
    }



    private void ResetSpawnPoints()
    {
        for (int i = 0; i < isSpawnPointUsed.Count; i++)
        {
            isSpawnPointUsed[i] = false; // Tüm spawn noktalarını kullanılmadı olarak işaretle
        }
    }

}
