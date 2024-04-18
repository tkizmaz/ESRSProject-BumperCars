using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject lobbySettingsPanel;
    [SerializeField]
    GameObject multiplayerPanel;
    [SerializeField]
    TMPro.TMP_InputField lobbyName;
    [SerializeField]
    TMPro.TMP_InputField playerCount;

    public void ChangePanelToLobbySettings()
    {
        multiplayerPanel.SetActive(false);
        lobbySettingsPanel.SetActive(true);
    }

    public void CreateLobby()
    {;
        int playerCountSetting;
        int.TryParse(playerCount.text, out playerCountSetting);
        LobbyManager.instance.CreateLobby(lobbyName.text, playerCountSetting);
    }
}
