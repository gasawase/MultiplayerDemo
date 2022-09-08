using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class LobbyScript : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerPanel[] lobbyPlayers;
    [SerializeField] private GameObject playerPrefab;
    //holds a list of network players
    private NetworkList<PlayerInfo>
}
