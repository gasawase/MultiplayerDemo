using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class StartMenu : NetworkBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private GameObject hostButton;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) { return; }
        
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public void HostButtClicked()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started");
        SceneManager.LoadScene("Lobby");
        
        //have them select a character here
    }

    public void StartGameButtClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client Started");
        
        //wait for the host to select before switching to the new scene
        
        //have them select a character here
    }

    public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;

        //var connectionData = request.Payload; //any additional connection data 
        
        response.Approved = true; //are they approved or not
        response.CreatePlayerObject = true; //do they create a player object

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = quaternion.identity;

        switch (NetworkManager.Singleton.ConnectedClients.Count)
        {
            case 1:
                spawnPos = new Vector3(0f, 0f, 0.61f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 2:
                spawnPos = new Vector3(1.492f, 0f, -0.681f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
        }
    }

    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }
    private void HandleClientConnected(ulong clientId) //called on the server every time a client joins; also when on a client side when they themselves join
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            hostButton.SetActive(true);
        }
        
    }
    
    private void HandleClientDisconnect(ulong clientId) //called on the server every time a client joins; also when on a client side when they themselves join
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            hostButton.SetActive(false);
        }
        
    }
}
