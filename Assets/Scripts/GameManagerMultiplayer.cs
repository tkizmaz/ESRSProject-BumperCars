using Unity.Netcode;
using UnityEngine;

public class GameManagerMultiplayer : NetworkBehaviour
{
    public static GameManagerMultiplayer instance;
    public int totalRounds = 3; // Toplam round sayısı
    private NetworkVariable<int> currentRound = new NetworkVariable<int>(1); 
    private bool isGameStarted = false;
    public NetworkVariable<int> activePlayers = new NetworkVariable<int>(0); 

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    [ServerRpc]
    public void StartRoundServerRpc(bool isFirstRound)
    {
        if(activePlayers.Value == 2 && isFirstRound)
        {
            Debug.Log("Round " + currentRound.Value + " başlıyor...");
            currentRound.Value++;

        }
        else if (currentRound.Value <= totalRounds && activePlayers.Value == 2 && !isFirstRound)
        {
            Debug.Log("Round " + currentRound.Value + " başlıyor...");
            currentRound.Value++;
        }
        else
        {
            EndGame();
        }
    }

    [ServerRpc]
    public void PlayerEliminatedServerRpc()
    {
        activePlayers.Value--; // Oyuncu elendiğinde sayıyı azalt
        if (activePlayers.Value <= 1)
        {
            activePlayers.Value = 2;
            SpawnManager.instance.RespawnAllPlayersServerRpc();
            StartRoundServerRpc(false);
        }
    }


    private void EndGame()
    {
        Debug.Log("Oyun bitti. Toplam oynanan round: " + (currentRound.Value - 1));
    }

    [ClientRpc]
    private void StartRoundClientRpc()
    {
        Debug.Log("Client'da round başladı.");
    }

    public void StartRound(bool isFirstRound)
    {
        StartRoundServerRpc(isFirstRound);
    }

    private void Update()
    {
        if (IsServer && !isGameStarted && activePlayers.Value >= 2)
        {
            isGameStarted = true;
            StartRound(true);
        }

        Debug.Log(activePlayers.Value);
    }
}
