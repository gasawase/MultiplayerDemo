using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour 
{
    public static GameManager instance { get; private set; }
    public Player playerPrefab;

    public NetworkList<PlayerInfo> playerList;

    private void Awake()
    {
        playerList = new NetworkList<PlayerInfo>();
    }

    public void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
}