using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button server;
    [SerializeField] private Button host;
    [SerializeField] private Button client;

    private void Awake()
    {
        server.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();

        });

        host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        client.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
