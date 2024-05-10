using Unity.Netcode;
using UnityEngine;

public class ZoneControllerMultiplayer : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            NetworkObject playerNetworkObject = other.gameObject.GetComponent<NetworkObject>();
            if (playerNetworkObject != null && IsServer)
            {
                EliminatePlayer(playerNetworkObject.NetworkObjectId);
                // ScoreManager.Instance.PlayerFell(playerNetworkObject.NetworkObjectId); // Eğer skor takibi yapacaksanız bu satırı aktifleştirebilirsiniz.
                GameManagerMultiplayer.instance.PlayerEliminatedServerRpc();
            }
        }
    }

    private void EliminatePlayer(ulong networkId)
    {
        EliminatePlayerClientRpc(networkId);
    }

    [ClientRpc]
    private void EliminatePlayerClientRpc(ulong networkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkId, out NetworkObject networkObject))
        {
            networkObject.gameObject.SetActive(false); // GameObject'i deaktive et
        }
    }
}

