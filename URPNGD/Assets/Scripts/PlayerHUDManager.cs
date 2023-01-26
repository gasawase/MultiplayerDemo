using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// controls player HUD and everything displayed
/// </summary>


// holds the network variable of the player health 
// sends info to all clients that are received from player attributes

public class PlayerHUDManager : NetworkBehaviour
{
    public GameObject _playerHUD;
    
    [SerializeField] public Sprite[] listOfSprites;
    [SerializeField] public TMP_Text playerNameTxt; //this is controlled in GameManager under RecievePlayerNameClientRpc
    [SerializeField] public PlayerPanelDisplay playerPanelPrefab;
    [SerializeField] public TMP_Text playerClientId;
    public GameObject playersContent;
    
    private GameManager _gameMgr;
    private List<PlayerPanelDisplay> _playerPanelDisplays;

    private void Awake()
    {
        _gameMgr = GetComponent<GameManager>();
        _playerPanelDisplays = new List<PlayerPanelDisplay>();
        playerClientId.text = NetworkManager.LocalClientId.ToString();
        RefreshPlayerPanels();
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            if (pi.clientId == NetworkManager.LocalClientId)
            {
                playerNameTxt.text = pi.m_PlayerName;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        _playerHUD.SetActive(IsOwner);
        GameData.Instance.allPlayers.OnListChanged += ClientOnAllPlayersChanged;
    }

    private void RefreshPlayerPanels()
    {
        foreach (PlayerPanelDisplay panel in _playerPanelDisplays)
        {
            Destroy(panel.gameObject);
        }
        _playerPanelDisplays.Clear();

        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            if (pi.clientId != NetworkManager.LocalClientId)
            {
                AddPlayerPanel(pi);
            }
            //AddPlayerPanel(pi);
        }
    }
    
    private void AddPlayerPanel(PlayerInfo info)
    {
        PlayerPanelDisplay newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(playersContent.transform, false);
        newPanel.SetName(info.m_PlayerName);
        //newPanel.SetSpriteLoc(info.);
        //newPanel.RefreshHealth(100); //is there a way to get the client health here? do i call a client rpc here?
        _playerPanelDisplays.Add(newPanel);
    }
    
    private void ClientOnAllPlayersChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        RefreshPlayerPanels();
    }

}