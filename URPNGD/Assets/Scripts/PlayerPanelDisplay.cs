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
    public LobbyPlayerPanel playerPanelPrefab;
    
    private List<LobbyPlayerPanel> playerPanels;

    private void Start()
    {
        //spawn playerHud
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            RefreshPlayerPanels();
        }
    }

    private void Awake()
    {
        playerPanels = new List<LobbyPlayerPanel>();
    }
    
    private void AddPlayerPanel(PlayerInfo info) {
        LobbyPlayerPanel newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(playerScrollContent.transform, false);
        newPanel.SetName(info.m_PlayerName);
        newPanel.SetColor(info.color);
        newPanel.SetReady(info.isReady);
        playerPanels.Add(newPanel);
    }
    
    private void RefreshPlayerPanels() {
        foreach (LobbyPlayerPanel panel in playerPanels) {
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
        
    }

    private void OnClientDisconnected(ulong clientId)
    {
        
    }
}
