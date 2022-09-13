using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class LobbyScript : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerPanel[] lobbyPlayers;
    [SerializeField] private GameObject playerPrefab;

    private StartMenu _startMenu;
    
    //holds a list of network players
    //private NetworkList<PlayerInfo> nwPlayers = new NetworkList<PlayerInfo>(); //network list of a custom class
    //private NetworkList<PlayerInfo> nwPlayers = new NetworkList<PlayerInfo>();
    
    private void Leave() //need to figure out how to disconnect people and remove them
    {
        
    }

    
}