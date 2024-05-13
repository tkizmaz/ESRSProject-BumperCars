using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;



public class UIManager : MonoBehaviour
{

    public static UIManager instance;
    public static UIManager Instance { get; private set; }

    [SerializeField]
    GameObject lobbySettingsPanel;
    [SerializeField]
    GameObject multiplayerPanel;
    [SerializeField]
    GameObject joinLobbyPanel;
    [SerializeField]
    GameObject createdLobbyPanel;
    [SerializeField]
    TMPro.TMP_InputField lobbyName;
    [SerializeField]
    TMPro.TMP_InputField playerCount;
    [SerializeField]
    Toggle isPrivate;
    [SerializeField]
    TMPro.TMP_Text lobbyCodeText;
    [SerializeField]
    Button joinLobbyButton;
    [SerializeField]
    GameObject lobbyButtonPrefab;
    [SerializeField]
    public Transform gridLayoutGroup;
    [SerializeField]
    GameObject listLobbiesPanel;
    [SerializeField]
    TMPro.TMP_InputField lobbyCodeField;
    [SerializeField]
    TMPro.TMP_Text playerName;
    [SerializeField]
    TMPro.TMP_Text lobbyNameText;
    [SerializeField]
    GameObject playerTextPrefab;
    [SerializeField]
    Transform playlerListLayout;
    [SerializeField]
    Button startGameButton;
    [SerializeField]
    TMPro.TMP_InputField changeButtonField;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        LobbyManager.OnPlayerJoined += UpdateLobbyUI;
    }

    public void ChangePanelToLobbySettings()
    {
        multiplayerPanel.SetActive(false);
        lobbySettingsPanel.SetActive(true);
    }

    public void CreateLobby()
    {
        int playerCountSetting;
        int.TryParse(playerCount.text, out playerCountSetting);
        bool isLobbyPrivate = isPrivate.isOn;
        LobbyManager.instance.CreateLobby(lobbyName.text, playerCountSetting, isLobbyPrivate);
    }

    public void ListLobbies()
    {
        multiplayerPanel.SetActive(false);
        listLobbiesPanel.SetActive(true);
        LobbyManager.instance.ListLobbies();
    }

    public void ShowLobbies(Lobby lobby)
    {
        GameObject button = Instantiate(lobbyButtonPrefab, gridLayoutGroup.transform);
        TMPro.TextMeshProUGUI buttonText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        string buttonString = lobby.Name + "   " +lobby.Players.Count.ToString() +"/"+ lobby.MaxPlayers.ToString();
        buttonText.text = buttonString;
        button.GetComponent<Button>().onClick.AddListener(() => LobbyManager.instance.JoinLobby(lobby));
    }

    public void ShowJoinCode(string lobbyCode)
    {
        lobbyCodeText.text = "Lobby Code " + lobbyCode;
        lobbyCodeText.gameObject.SetActive(true);
    }

    public void OpenJoinLobbyPanel()
    {
        multiplayerPanel.SetActive(false);
        joinLobbyPanel.SetActive(true);
    }

    public void JoinLobbyByCode()
    {
        LobbyManager.instance.JoinLobbyByCode(lobbyCodeField.text);
    }

    public void ChangePanelToCreatedLobby(string playerNameInServer, string lobbyName)
    {
        lobbySettingsPanel.SetActive(false);
        joinLobbyPanel.SetActive(false);
        createdLobbyPanel.SetActive(true);
        playerName.text = playerNameInServer;
        lobbyNameText.text = "Lobby: "+ lobbyName;
    }

    public void PutPlayersInGrid(string eachPlayerName)
    {
        GameObject newText = Instantiate(playerTextPrefab, playlerListLayout.transform);
        newText.GetComponent<TMPro.TMP_Text>().text = eachPlayerName;
    }

    public void ClearPlayerList()
    {
        for (int i = 0; i < playlerListLayout.transform.childCount; i++)
        {
            Destroy(playlerListLayout.transform.GetChild(i).gameObject);
        }
    }

    public void UpdateLobbyUI()
    {
        ClearPlayerList();
        var nameList = LobbyManager.instance.GetAllPlayers();
        foreach(string name in nameList)
        {
            PutPlayersInGrid(name);
        }

    }

    public void StartGame()
    {
        LobbyManager.instance.StartGame();
    }

    public void ChangeName()
    {
        LobbyManager.instance.UpdatePlayerName(changeButtonField.text);
    }

    public void ChangeFromLobbyList(string playerNameInServer, string lobbyName)
    {
        listLobbiesPanel.SetActive(false);
        createdLobbyPanel.SetActive(true);
        playerName.text = playerNameInServer;
        lobbyNameText.text = "Lobby: " + lobbyName;
        startGameButton.gameObject.SetActive(false);

    }

    public void UpdateName(string newName)
    {
        playerName.text = newName;
    }

}
