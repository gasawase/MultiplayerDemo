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
    [SerializeField] public Sprite[] listOfSprites;
    [SerializeField] public TMP_Text playerNameTxt; //this is controlled in GameManager under RecievePlayerNameClientRpc
    [SerializeField] public PlayerPanelDisplay playerPanelPrefab;
    [SerializeField] public TMP_Text playerClientId;

    [SerializeField] public GameObject testPanel;
    [SerializeField] public TMP_Text playerNameHolder;
    [SerializeField] public TMP_Text playerHealthHolder;

    [SerializeField] public GameObject blackScreen;
    [SerializeField] public float fadeSpeed = 5;
    
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
        //AssignCorrectName();
        RefreshPlayerPanels();
        playerClientId.text = this.NetworkManager.LocalClientId.ToString();
        GameData.Instance.allPlayers.OnListChanged += ClientOnAllPlayersChanged; 
        // get info for player name and such
        // disable the black screen
        StartCoroutine(FadeBlackPanel());
    }

    public IEnumerator FadeBlackPanel()
    {
        float alphaVal = 100;
        Color objectColor;

        while (blackScreen.GetComponent<Image>().color.a > 0)
        {
            objectColor = blackScreen.GetComponent<Image>().color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackScreen.GetComponent<Image>().color = objectColor;
            yield return null;
        }
        
        blackScreen.SetActive(false);
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
            // if this pi is the local player then spawn it on the top right            
            if (pi.clientId == NetworkManager.LocalClientId)
            {
                AddThisPlayerPanel(pi);
            }
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
        //newPanel.SetSpriteLoc(info.);
        //newPanel.RefreshHealth(100); //is there a way to get the client health here? do i call a client rpc here?
        _playerPanelDisplays.Add(newPanel);
        listOfPlayerHealthIds.Add(info.clientId, newPanel.playerHealth.value);


        // for each new panel, add an event listener that will track that players health

        //newPanel.playerHealth.onValueChanged.AddListener(delegate { OtherPlayerHealthManager();  });
    }
    
    private void AddThisPlayerPanel(PlayerInfo info)
    {
        //TODO: need to lock this and place this at the top right of the hud
        
        PlayerPanelDisplay newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(this.transform, false);
        newPanel.SetName(info.m_PlayerName);
        newPanel.SetClientId(info.clientId);
        //newPanel.SetSpriteLoc(info.);
        //newPanel.RefreshHealth(100); //is there a way to get the client health here? do i call a client rpc here?
        _playerPanelDisplays.Add(newPanel);
        listOfPlayerHealthIds.Add(info.clientId, newPanel.playerHealth.value);
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
            // where the clientID matches the NetworkManager.LocalClientId, assign the name here
            if (pi.clientId == NetworkManager.LocalClientId)
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