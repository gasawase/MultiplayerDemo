using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// controls player HUD and everything displayed
/// </summary>
public class PlayerPanelDisplay : NetworkBehaviour
{
    [SerializeField] public TMPro.TMP_Text displayNameText;
    public GameObject playerScrollContent;
    public PlayerPanelInGame playerPanelPrefab;
    
    private List<PlayerPanelInGame> playerPanels;

    private void Start()
    {
        //spawn playerHud
        // if (IsHost)
        // {
        //     NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        //     NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        // }
    }
    

    private void Awake()
    {
        //playerPanels = new List<LobbyPlayerPanel>();
    }

    public override void OnNetworkSpawn()
    {
        ////// this works; now i have to integrate the new player panels and health
        // if (IsHost)
        // {
        //     RefreshPlayerPanels();
        //     int myIndex = Convert.ToInt32(NetworkManager.LocalClientId);
        //     //int myIndex = GameData.Instance.FindPlayerIndex(NetworkManager.LocalClientId);
        //     if (myIndex != -1)
        //     {
        //         PlayerInfo info = GameData.Instance.allPlayers[myIndex];
        //         displayNameText.text = info.m_PlayerName;
        //     }
        // }
    }

    private void AddPlayerPanel(PlayerInfo info) {
        PlayerPanelInGame newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(playerScrollContent.transform, false);
        newPanel.SetName(info.m_PlayerName);
        newPanel.SetColor(info.color);
        newPanel.SetReady(info.isReady);
        playerPanels.Add(newPanel);
    }
    
    private void RefreshPlayerPanels() {
        foreach (PlayerPanelInGame panel in playerPanels) {
            Destroy(panel.gameObject);
        }
        playerPanels.Clear();

        foreach (PlayerInfo pi in GameData.Instance.allPlayers) {
            AddPlayerPanel(pi);
        }
    }
    
    ////////HANDLES/////////
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client Connected {clientId}");
        RefreshPlayerPanels();
        int myIndex = Convert.ToInt32(NetworkManager.LocalClientId);
        if (myIndex != -1)
        {
            PlayerInfo info = GameData.Instance.allPlayers[myIndex];
            displayNameText.text = info.m_PlayerName;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        
    }
}
