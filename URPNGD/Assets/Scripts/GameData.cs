﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameData : NetworkBehaviour {
    private static GameData _instance;
    public static GameData Instance {
        get {
            return _instance;
        }
    }

    private int colorIndex = 0;
    private Color[] playerColors = new Color[] {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    public NetworkList<PlayerInfo> allPlayers;
    public TMP_InputField playerInputField;
    public string playerName = "";
    public NetworkVariable<FixedPlayerName> fixedPlayerName;
    public Dictionary<ulong, string> dictPNames = new Dictionary<ulong, string>();
    public Button btnSubmitName = null;
    [SerializeField]public GameObject playerPrefab;
    public GameObject spawnedPlayer = null;
    public int intOfCurrentMesh = 0;

    // --------------------------
    // Initialization
    // --------------------------
    public void Awake() {
        // allPlayers must be initialized even though we might be destroying
        // this instance.  Errors occur if we do not.
        allPlayers = new NetworkList<PlayerInfo>();

        // This isn't working as expected.  If you place another GameData in a
        // later scene, it causes an error.  I suspect this has something to
        // do with the NetworkList but I have not verified that yet.  It causes
        // Network related errors.
        if(_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        } else if(_instance != this) {
            Destroy(this);
        }
    }


    public override void OnNetworkSpawn() {
        if (IsHost) {
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;
            //AddPlayerToList(NetworkManager.LocalClientId);
        }
        
    }


    // --------------------------
    // Private
    // --------------------------
    private Color NextColor() {
        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if (colorIndex > playerColors.Length - 1) {
            colorIndex = 0;
        }
        return newColor;
    }


    // --------------------------
    // Events
    // --------------------------
    private void HostOnClientConnected(ulong clientId) {
        //SendPNameServerRpc(playerName); for when input is/was on login screen
        //AddPlayerToList(clientId);
    }

    private void HostOnClientDisconnected(ulong clientId) {
        int index = FindPlayerIndex(clientId);
        if (index != -1) {
            allPlayers.RemoveAt(index);
            dictPNames.Remove(clientId);
        }
    }

    public void SubmitButtonClicked()
    {
        playerInputField = GameObject.FindGameObjectWithTag("InputField").GetComponent<TMP_InputField>();
        string inpString = GameObject.FindGameObjectWithTag("InputField").name;
        playerName = playerInputField.text;
        SendPNameServerRpc(playerName);
        Debug.Log($"{playerName} is the name from the inpString below");
        Debug.Log($"{inpString} is the name of the input field");
        SpawnPlayerSelector();

        Button rightSelectorBut = GameObject.Find("RightArrow").GetComponent<Button>();
        rightSelectorBut.onClick.AddListener(IncreasePlayerMeshNum);
        Button leftSelectorBut = GameObject.Find("LeftArrow").GetComponent<Button>();
        leftSelectorBut.onClick.AddListener(DecreasePlayerMeshNum);
        Button selectPlayMeshBut = GameObject.Find("SubmitMesh").GetComponent<Button>();
        selectPlayMeshBut.onClick.AddListener(PlayerMeshSelected);
    }

    // --------------------------
    // Public
    // --------------------------
    public void AddPlayerToList(ulong clientId, string pName) {
        
        allPlayers.Add(new PlayerInfo(clientId, pName, NextColor(), false));
    }

    public void AddPlayerNameToDictionary(ulong clientId, string pName)
    {
        if (dictPNames.ContainsKey(clientId))
        {
            if (dictPNames[clientId] != null)
            {
                dictPNames[clientId] = pName;
            }
            else
            {
                dictPNames.Add(clientId, pName);
            }
        }
        Debug.Log($"{clientId} has name {pName}");

    }


    public int FindPlayerIndex(ulong clientId) {
        var idx = 0;
        var found = false;

        while (idx < allPlayers.Count && !found) {
            if (allPlayers[idx].clientId == clientId) {
                found = true;
            } else {
                idx += 1;
            }
        }
        if (!found) {
            idx = -1;
        }
        return idx;
    }

    public void SpawnPlayerSelector()
    {
        //spawn player prefab with the specific locations DONE
        //enable UI for selecting the player mesh DONE
        //add listeners and controls for selecting player mesh and for sending the data to player info
        Vector3 scaleChange = new Vector3(9.5f, 9.5f, 9.5f);
        Vector3 spawnPos = new Vector3(-2.98f, -10.55f, 21.03f);
        spawnedPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.Euler(0, 180, 0));
        spawnedPlayer.transform.localScale = scaleChange;
    }

    public void IncreasePlayerMeshNum()
    {
        //change meshes instead of enabling/disabling objects
        Mesh[] listOfMeshes = spawnedPlayer.GetComponent<Player>().listOfMeshes;
        SkinnedMeshRenderer skinnedMeshRenderer =
            GameObject.Find("MainPlayerObject").GetComponent<SkinnedMeshRenderer>();
        int lengthOfArray = listOfMeshes.Length; //number of objects in the array

        skinnedMeshRenderer.sharedMesh = listOfMeshes[intOfCurrentMesh];
        intOfCurrentMesh++;
        if (intOfCurrentMesh == lengthOfArray)
        {
            intOfCurrentMesh = 0;
        }
        Debug.Log(intOfCurrentMesh);
    }
    public void DecreasePlayerMeshNum()
    {
        Mesh[] listOfMeshes = spawnedPlayer.GetComponent<Player>().listOfMeshes;
        SkinnedMeshRenderer skinnedMeshRenderer =
            GameObject.Find("MainPlayerObject").GetComponent<SkinnedMeshRenderer>();
        int lengthOfArray = listOfMeshes.Length; //number of objects in the array
        Debug.Log(lengthOfArray);

        skinnedMeshRenderer.sharedMesh = listOfMeshes[intOfCurrentMesh];
        intOfCurrentMesh--;
        Debug.Log(intOfCurrentMesh);
        if (intOfCurrentMesh < 0)
        {
            intOfCurrentMesh = lengthOfArray-1;
            Debug.Log(intOfCurrentMesh);
        }

    }

    public void PlayerMeshSelected()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendPNameServerRpc(string pName, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"Host got name:  {pName}");

        SendPNameClientRpc(pName, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void SendPNameClientRpc(string pName, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("sendPNameClientRpc ran");
        AddPlayerNameToDictionary(clientId, pName);
        AddPlayerToList(clientId, pName);
    }
}