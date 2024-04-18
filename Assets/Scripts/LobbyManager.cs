using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager instance;
    public static LobbyManager Instance { get; private set; }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("SignedIn");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();


    }

    public async void CreateLobby(string lobbyName, int playerCount)
    {

        try
        {
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, playerCount);

            Debug.Log(lobby.Name + lobby.MaxPlayers);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }

        ListLobbies();
        
    }

    private async void ListLobbies()
    {
        QueryResponse query = await Lobbies.Instance.QueryLobbiesAsync();

        foreach(Lobby lobby in query.Results)
        {
            Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
        }
    }
}
