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



//TODO: have some way to have a controller that instantiates and assigns the player attributes and player panel displays whenever a player spawns and despawns

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
    public List<PlayerPanelDisplay> _playerPanelDisplays;
    public Dictionary<ulong, float> listOfPlayerHealthIds;
    
    private GameManager _gameMgr;
    private PlayerAttributes[] playerAttributes;
    
    private void Awake()
    {
        _gameMgr = GetComponent<GameManager>();
        _playerPanelDisplays = new List<PlayerPanelDisplay>();
        listOfPlayerHealthIds = new Dictionary<ulong, float>();
    }

    /// <summary>
    /// Sets the player attributes variables
    /// </summary>
    private void SetPlayerAttributes()
    {
        // if this array is null, create the array. Else, clear it.
        if (playerAttributes == null)
        {
            playerAttributes = FindObjectsOfType<PlayerAttributes>();
        }
        else
        {
            for (int i = 0; i < playerAttributes.Length; i++)
            {
                playerAttributes[i] = null;
            }
        }

        foreach (PlayerAttributes attributes in playerAttributes)
        {
            attributes.SetHUDAwake(true);
            attributes.SetOwnerClientId();
            foreach (PlayerPanelDisplay panel in _playerPanelDisplays)
            {
                if (attributes.thisClientId == panel.personalClientId)
                {
                    attributes.hpBar = panel.playerHealth;
                }
            }
            Debug.Log("Ran for one attribute");
        }
    }

    /// <summary>
    /// Sets all attributes on the HUD including the player panels and fades
    /// to black when it's all done
    /// </summary>
    public void SetUpPlayerHUD()
    {
        //AssignCorrectName();
        RefreshPlayerPanels();
        SetPlayerAttributes();
        playerClientId.text = this.NetworkManager.LocalClientId.ToString();
        GameData.Instance.allPlayers.OnListChanged += ClientOnAllPlayersChanged; 
        // get info for player name and such
        // disable the black screen
        StartCoroutine(FadeBlackPanel());
    }

    /// <summary>
    /// A coroutine that fades the black panel out
    /// </summary>
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

    /// <summary>
    /// refreshes the player panel displays whenever the player info list changes aka whenever a player joins or leaves
    /// </summary>
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
    
    /// <summary>
    ///
    /// RUNS IF: pi.clientId != NetworkManager.LocalClientId
    /// </summary>
    /// <param name="info"></param>
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

    }
    
    /// <summary>
    ///
    /// RUNS IF: pi.clientId == NetworkManager.LocalClientId
    /// </summary>
    /// <param name="info"></param>
    private void AddThisPlayerPanel(PlayerInfo info)
    {
        PlayerPanelDisplay newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(this.transform, false);
        newPanel.SetName(info.m_PlayerName);
        newPanel.SetClientId(info.clientId);
        //newPanel.SetSpriteLoc(info.);
        //newPanel.RefreshHealth(100); //is there a way to get the client health here? do i call a client rpc here?
        _playerPanelDisplays.Add(newPanel);
        listOfPlayerHealthIds.Add(info.clientId, newPanel.playerHealth.value);
        
        
        
    }
    
    /// <summary>
    /// The event that is called whenever the PlayerInfo variable changes
    /// </summary>
    /// <param name="changeEvent"></param>
    private void ClientOnAllPlayersChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        RefreshPlayerPanels();
        SetPlayerAttributes();
    }
    
    /// <summary>
    /// for all players currently active, if the player in player
    /// info matches the owner client id, attach their name to the text panel
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="health"></param>
    /// <param name="playerWhoTookDamageId"></param>
    public void UpdatePlayersHealthUI(int health, ulong playerWhoTookDamageId)
    {
        // find/get the player panels
        // change health here

        foreach (var panel in GetListOfPlayerPanels())
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

    /// <summary>
    /// Gets a list of all player panels currently active
    /// </summary>
    /// <returns> list of player panels </returns>
    private GameObject[] GetListOfPlayerPanels()
    {
        GameObject[] listPlayerPanels = GameObject.FindGameObjectsWithTag("PlayerPanel");
        return listPlayerPanels;
    }

}