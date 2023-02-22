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
    }

    public void SetUpPlayerHUD()
    {
        AssignCorrectName();
        // enable player panels and get the data for the player panels
        _playerHUD.SetActive(IsOwner);
        
        // if the hud that has been spawned is not of the owner then delete it 
        if (_playerHUD.activeInHierarchy == false)
        {
            Destroy(_playerHUD);
        }
        RefreshPlayerPanels();
        playerClientId.text = this.NetworkManager.LocalClientId.ToString();
        // get info for player name and such
        // disable the black screen
    }

    public override void OnNetworkSpawn()
    {
        // SetUpPlayerHUD();
        // _playerHUD.SetActive(IsOwner);
        // if (_playerHUD.activeInHierarchy == false)
        // {
        //     Destroy(_playerHUD);
        // }
        // GameData.Instance.allPlayers.OnListChanged += ClientOnAllPlayersChanged; 
        // RefreshPlayerPanels();
    }
    

    private void RefreshPlayerPanels()
    {
        // for all players in the list of player panels that have been added, destroy them
        foreach (PlayerPanelDisplay panel in _playerPanelDisplays)
        {
            Destroy(panel.gameObject);
        }
        _playerPanelDisplays.Clear();

        // for all players currently in the game data
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            // if the player id does not match the local client id of this specific player
            // then don't add the panel
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

    // for all players currently active, if the player in player
    // info matches the owner client id, attach their name to the text panel
    private void AssignCorrectName()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            // if (pi.clientId == NetworkManager.LocalClientId)
            // {
            //     playerNameTxt.text = pi.m_PlayerName;
            // }
            
            // where the clientID matches the networkobject.ownerclientid, assign the name here
            if (pi.clientId == NetworkObject.OwnerClientId)
            {
                playerNameTxt.text = pi.m_PlayerName;
            }
        }
    }

    public void UpdatePlayersHealthUI(int health, ulong playerWhoTookDamageId)
    {
        // find/get the player panels
        // change health here

        GameObject[] listPlayerPanels = GetListOfPlayerPanels();

        foreach (var panel in listPlayerPanels)
        {
            PlayerPanelDisplay ppdisplayScript = panel.GetComponent<PlayerPanelDisplay>();

            if (ppdisplayScript.personalClientId == playerWhoTookDamageId)
            {
                ppdisplayScript.playerHealth.value = health;
            }
        }
        
        playerNameHolder.text = playerWhoTookDamageId.ToString();
        playerHealthHolder.text = health.ToString();
        Debug.Log($"Player {playerWhoTookDamageId} took {health} damage");
    }

    private GameObject[] GetListOfPlayerPanels()
    {
        GameObject[] listPlayerPanels = GameObject.FindGameObjectsWithTag("PlayerPanel");
        return listPlayerPanels;
    }

}