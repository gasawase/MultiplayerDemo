using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TroubleshootLobby : NetworkBehaviour
{
    [SerializeField] private GameObject playerPanels;


    public struct PlayerData : IEquatable<PlayerData>
    {
        public ulong networkClientID;
        public bool networkPlayerReady;

        public PlayerData(ulong clientId, bool PlayerReady)
        {
            networkClientID = clientId;
            networkPlayerReady = PlayerReady;
        }

        public bool Equals(PlayerData other)
        {
            return networkClientID == other.networkClientID &&
                   networkPlayerReady == other.networkPlayerReady;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(networkClientID, networkPlayerReady);
        }
    }

    public NetworkList<PlayerData> playerDataStruct = new NetworkList<PlayerData>();
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void HandleClientConnected(ulong clientId)
    {
        UpdateConnectionListServerRPC(clientId);
        Debug.Log("A Player has connected ID: " + clientId);
        // sets waiting for player to false and activates playerdatapanel
        // playerPanels.transform.Find("WaitingForPlayer").gameObject.SetActive(false);
        // playerPanels.transform.Find("PlayerDataPanel").gameObject.SetActive(true);


    }

    [ServerRpc]
    private void UpdateConnectionListServerRPC(ulong clientId)
    {
        playerDataStruct.Add(new PlayerData(clientId, false));
        TestMethodClientRPC();

    }

    [ClientRpc]
    private void TestMethodClientRPC()
    {
        playerPanels.transform.Find("WaitingForPlayer").gameObject.SetActive(false);
        playerPanels.transform.Find("PlayerDataPanel").gameObject.SetActive(true);
        Debug.Log("ClientRPC Worked");
    }
}
