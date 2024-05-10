using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using static LobbyManager;
using System;

public class LobbyManager : MonoBehaviour
{
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float pollTimer;
    public static LobbyManager instance;
    public static LobbyManager Instance { get; private set; }
    private string playerName;
    public static event Action OnPlayerJoined;
    public const string KEY_START_GAME = "Start";
    public event EventHandler<EventArgs> OnGameStarted;


    private void Update()
    {

        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (joinedLobby != null && AuthenticationService.Instance.PlayerId == joinedLobby.HostId)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log($"Error sending heartbeat: {e}");
                }
            }
        }
    }


    private async void Authenticate(string playerName)
    {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerName);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    // Start is called before the first frame update
    private async void Start()
    {
        playerName= "Player" + UnityEngine.Random.Range(0, 1000);
        Authenticate(playerName);

    }

    public async void CreateLobby(string lobbyName, int playerCount, bool isLobbyPrivate)
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isLobbyPrivate,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, playerCount, createLobbyOptions);

            joinedLobby = lobby;

            UIManager.instance.ShowJoinCode(joinedLobby.LobbyCode);
            UIManager.instance.ChangePanelToCreatedLobby(playerName, joinedLobby.Name);
            UIManager.instance.UpdateLobbyUI();

        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }


    }

    public async void ListLobbies()
    {
        QueryResponse query = await Lobbies.Instance.QueryLobbiesAsync();
        foreach(Lobby lobby in query.Results)
        {
            UIManager.instance.ShowLobbies(lobby);
        }
    }

    public async void JoinLobby(Lobby lobby)
    {
        QueryResponse query = await Lobbies.Instance.QueryLobbiesAsync();
        await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
        {
            Player = GetPlayer()
        };
        try
        {
            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            PrintPlayers(joinedLobby);
            OnPlayerJoined?.Invoke();
            UIManager.instance.ShowJoinCode(joinedLobby.LobbyCode);
            UIManager.instance.ChangePanelToCreatedLobby(playerName, joinedLobby.Name);
            UIManager.instance.UpdateLobbyUI();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            pollTimer -= Time.deltaTime;
            if (pollTimer < 0f)
            {
                Debug.Log("poll");
                float lobbyUpdateTimerMax = 1.1f;
                pollTimer = lobbyUpdateTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                OnPlayerJoined?.Invoke(); // UI güncellemesi için event'i tetikle

                Debug.Log(joinedLobby.Data[KEY_START_GAME].Value);
                if (joinedLobby.Data[KEY_START_GAME].Value != "0")
                {
                    SceneManager.LoadScene(3);
                    if(!IsLobbyHost())
                    {
                        Debug.Log("acil");
                    }

                    RelayLobby.instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                    joinedLobby = null;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }


    public string getLobbyCode()
    {
        return joinedLobby.LobbyCode;
    }

    private void PrintPlayers(Lobby lobby)
    {
        foreach(Player player in lobby.Players)
        {
            Debug.Log(player.Id + player.Data["PlayerName"].Value);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> { { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) } }
        };
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, newPlayerName) } 

            }
        });

    }

    private async void LeaveLobby()
    {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
    }

    public List<string> GetAllPlayers()
    {
        List<string> nameList = new List<string>();
        foreach (Player player in joinedLobby.Players)
        {
            string name = player.Data["PlayerName"].Value;
            nameList.Add(name);
        }
        return nameList;
    }

    public async void StartGame()
    {
        if(IsLobbyHost())
        {
            try
            {
                Debug.Log("Start Game");

                SceneManager.LoadScene(3);
                string relayCode = await RelayLobby.instance.CreateRelay();


                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }

                    }
                });

                joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby()
    {
        if(joinedLobby != null && joinedLobby.Players != null)
        {
            foreach(Player player in joinedLobby.Players)
            {
                if(player.Id == AuthenticationService.Instance.PlayerId)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
