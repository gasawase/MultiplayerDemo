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

    [SerializeField] public GameObject testPanel;
    [SerializeField] public TMP_Text playerNameHolder;
    [SerializeField] public TMP_Text playerHealthHolder;
    
    public GameObject playersContent;
    
    private GameManager _gameMgr;
    public List<PlayerPanelDisplay> _playerPanelDisplays;
    public Dictionary<ulong, float> listOfPlayerHealthIds;

    private void Awake()
    {
        _gameMgr = GetComponent<GameManager>();
        _playerPanelDisplays = new List<PlayerPanelDisplay>();
        listOfPlayerHealthIds = new Dictionary<ulong, float>();
        
        playerClientId.text = NetworkManager.LocalClientId.ToString();
        RefreshPlayerPanels();
    }

    private void SetUpPlayerHUD()
    {
        AssignCorrectName();
        
    }

    public override void OnNetworkSpawn()
    {
        SetUpPlayerHUD();
        _playerHUD.SetActive(IsOwner);
        if (_playerHUD.activeInHierarchy == false)
        {
            Destroy(_playerHUD);
        }
        GameData.Instance.allPlayers.OnListChanged += ClientOnAllPlayersChanged; }

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
        }
    }
    
    private void AddPlayerPanel(PlayerInfo info)
    {
        PlayerPanelDisplay newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(playersContent.transform, false);
        newPanel.SetName(info.m_PlayerName);
        newPanel.SetClientId(info.clientId);
        Debug.Log($"newPanel client id: {newPanel.personalClientId}");
        //newPanel.SetSpriteLoc(info.);
        //newPanel.RefreshHealth(100); //is there a way to get the client health here? do i call a client rpc here?
        _playerPanelDisplays.Add(newPanel);
        listOfPlayerHealthIds.Add(info.clientId, newPanel.playerHealth.value);
        
        // for each new panel, add an event listener that will track that players health
        
        //newPanel.playerHealth.onValueChanged.AddListener(delegate { OtherPlayerHealthManager();  });
        Debug.Log($"listOfPlayerHealthAndPanels info.clientId: {info.clientId}");
    }
    
    private void ClientOnAllPlayersChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        RefreshPlayerPanels();
    }

    private void AssignCorrectName()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            if (pi.clientId == NetworkManager.LocalClientId)
            {
                playerNameTxt.text = pi.m_PlayerName;
            }
        }
    }

    public void UpdatePlayersHealthUI(int health, ulong playerWhoTookDamageId)
    {
        playerNameHolder.text = playerWhoTookDamageId.ToString();
        playerHealthHolder.text = health.ToString();
        Debug.Log($"Player {playerWhoTookDamageId} took {health} damage");
    }

}