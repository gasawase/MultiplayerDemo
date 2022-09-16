using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace Summer.Multiplayer
{
    public class LobbyScript : NetworkBehaviour
    {
        [SerializeField] private LobbyPlayerPanel[] lobbyPlayers;
        [SerializeField] private GameObject playerPrefab;
        
        private StartMenu _startMenu;

        //holds a list of network players
        private NetworkList<PlayerInfo> nwPlayers = new NetworkList<PlayerInfo>(); //network list of a custom class
        //private NetworkList<PlayerInfo> nwPlayers = new NetworkList<PlayerInfo>();

        private void Start()
        {
            UpdateConnListServerRpc(NetworkManager.LocalClientId);
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        }

        public override void NetworkStart()
        {
            Debug.Log("Starting Server");
            if (IsClient)
            {
                nwPlayers.OnListChanged += PlayerInfoChanged;
            }

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedHandle;
                NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectedHandle;
                //handle for people connected
                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients)
                {
                    ClientConnectedHandle(client.ClientId);
                }
            }
        }

        private void HandleClientConnected (ulong clientId)
        {
            
        }
        
        [ServerRpc]

        private void UpdateConnListServerRpc(ulong clientId)
        {
            nwPlayers.Add(new PlayerInfo(clientId, PlayerPrefs.GetString("PName"), false));
        }

        private void ClientConnectedHandle(ulong clientId)
        {
            
        }

        private void ClientDisconnectedHandle(ulong clientId)
        {
            
        }
    }    
}
