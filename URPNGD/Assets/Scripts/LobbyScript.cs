using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace Summer.Multiplayer
{
    public class LobbyScript : NetworkBehaviour
    {
        public LobbyPlayerPanel playerPanelPrefab;
        public GameObject playersPanel;
        public GameObject playerScrollContent;
        public TMP_Text txtPlayerNumber;
        public Button btnStart;
        public Button btnReady;
        public Player playerPrefab;

        private NetworkList<PlayerInfo> allPlayers = new NetworkList<PlayerInfo>();
        private List<LobbyPlayerPanel> playerPanels = new List<LobbyPlayerPanel>();

        public void Start()
        {
            if (IsHost)
            {
                AddPlayerToList(NetworkManager.LocalClientId);
                RefreshPlayerPanels();
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;
                btnReady.gameObject.SetActive(false);
            }
            
            //must be after host connects to signals
            base.OnNetworkSpawn();

            if (IsClient && !IsHost)
            {
                allPlayers.OnListChanged += ClientOnAllPlayersChanged;
                btnStart.gameObject.SetActive(false);
                btnReady.onClick.AddListener(ClientOnReadyClicked);
            }

            txtPlayerNumber.text = $"Player #{NetworkManager.LocalClientId}";
        }

        [ServerRpc(RequireOwnership = false)]

        public void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            int playerIndex = FindPlayerIndex(clientId);
            PlayerInfo info = allPlayers[playerIndex];

            info.isReady = !info.isReady;
            allPlayers[playerIndex] = info;

            info = allPlayers[playerIndex];

            RefreshPlayerPanels();

            int readyCount = 0;
            foreach (PlayerInfo readyInfo in allPlayers)
            {
                if (readyInfo.isReady)
                {
                    readyCount += 1;
                }
            }

            btnStart.enabled = readyCount == allPlayers.Count - 1;
        }

        private int FindPlayerIndex(ulong clientId)
        {
            var idx = 0;
            var found = false;

            while (idx < allPlayers.Count && !found)
            {
                if (allPlayers[idx].clientId == clientId)
                {
                    found = true;
                }
                else
                {
                    idx += 1;
                }
            }

            if (!found)
            {
                idx = -1;
            }

            return idx;
        }

        private void ClientOnAllPlayersChanged(NetworkListEvent<PlayerInfo> changeEvent)
        {
            RefreshPlayerPanels();
        }

        private void HostOnClientConnected(ulong clientId)
        {
            btnStart.enabled = false;
            AddPlayerToList(clientId);
            RefreshPlayerPanels();
        }

        private void HostOnClientDisconnected(ulong clientId)
        {
            int index = FindPlayerIndex(clientId);
            if (index != -1)
            {
                allPlayers.RemoveAt(index);
                RefreshPlayerPanels();
            }
        }

        private void AddPlayerToList(ulong clientId)
        {
            allPlayers.Add(new PlayerInfo(clientId, false));
        }

        private void AddPlayerPanel(PlayerInfo info)
        {
            LobbyPlayerPanel newPanel = Instantiate(playerPanelPrefab);
            //makes the playerScrollContent it's parent
            newPanel.transform.SetParent(playerScrollContent.transform, false);
            newPanel.SetName($"Player {info.clientId.ToString()}");
            newPanel.SetReady(info.isReady);
            playerPanels.Add(newPanel);
        }

        private void ClientOnReadyClicked()
        {
            ToggleReadyServerRpc();
        }

        public void RefreshPlayerPanels()
        {
            foreach (LobbyPlayerPanel panel in playerPanels)
            {
                Destroy(panel.gameObject);
            }
            playerPanels.Clear();

            foreach (PlayerInfo pi in allPlayers)
            {
                AddPlayerPanel(pi);
            }
        }
    }
}
