using System;
using Summer.Multiplayer;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class ShaneLobby : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerPanel[] lobbyPlayers;
    [SerializeField] private GameObject playerPrefab;
    //holds a list of network players
    private NetworkList<PlayerInfo> nwPlayers;

    private void Awake()
    {
        nwPlayers = new NetworkList<PlayerInfo>();
    }

    void Start()
    {
        UpdateConnListServerRpc(NetworkManager.LocalClientId);
        //Debug.Log(PlayerPrefs.GetString("PName"));
        //Debug.Log(nwPlayers[0].PlayerName);
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("StartingServer");
        if (IsClient)
        {
            nwPlayers.OnListChanged += PlayersInfoChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedHandle;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectedHandle;
            //handle for people connected
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                ClientConnectedHandle(client.ClientId);
            }
        }
    }

    /*private void OnDestroy()
    {
        nwPlayers.OnListChanged -= PlayersInfoChanged;
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedHandle;
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectedHandle;
        }
    }*/

    private void PlayersInfoChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        //update the UI lobby
        int index = 0;
        foreach (PlayerInfo connectedplayer in nwPlayers)
        {
            lobbyPlayers[index].playerName.text = connectedplayer.m_PlayerName;
            index++;
            Debug.Log(connectedplayer.m_PlayerName);
        }
    }

    public void StartGame()
    {
        if (IsServer)
        {
            //spawn player prefab for each connected client
            //  TODO: get their mesh and spawn that version
            foreach (PlayerInfo tmpClient in nwPlayers)
            {
                GameObject playerSpawn = Instantiate(playerPrefab, new Vector3(2f, 1f, 7f), Quaternion.identity);
                playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(tmpClient.networkClientID);
                Debug.Log("Player spawned for: " + tmpClient.m_PlayerName);
            }

            SceneManager.LoadScene("MainLevel");
        }
        else
        {
            Debug.Log("You are not the host");
        }
    }
    

    //HANDLES
    private void HandleClientConnected(ulong clientId)
    {
        UpdateConnListServerRpc(clientId);
        Debug.Log("A Player has connected ID: " + clientId);
        Debug.Log(nwPlayers[0].m_PlayerName);
    }

    [ServerRpc]

    private void UpdateConnListServerRpc(ulong clientId)
    {
        nwPlayers.Add(new PlayerInfo(clientId, PlayerPrefs.GetString("PName"), false));
    }

    private void ClientDisconnectedHandle(ulong clientId)
    {
        Debug.Log("TODO");
    }
    
    private void ClientConnectedHandle(ulong clientId)
    {
        throw new NotImplementedException();
    }

}
