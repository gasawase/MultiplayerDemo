using System;
using Summer.Multiplayer;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : NetworkBehaviour
{
    //get the nanme
    //allow them to enter if the server is open
    [SerializeField] private TMP_InputField playerName;
    //private int clientId;
    
    //private NetworkList<PlayerInfo> nwPlayers;
    private NetworkList<PlayerInfo> nwPlayers = new NetworkList<PlayerInfo>();

    //if conncected successfully, add them to the player list
    private void Start()
    {
        //nwPlayers = new NetworkList<PlayerInfo>();
        //activate the server
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }
    
    public void HostButtClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartHost();
        Debug.Log("Started Host");
        UpdateConnListServerRpc(NetworkManager.LocalClientId);
        Debug.Log(PlayerPrefs.GetString("PName"));
        Debug.Log(nwPlayers[0].PlayerName);
        SceneManager.LoadScene("Lobby");
        
        //have them select a character here
    }

    public void StartGameButtClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartClient();
        Debug.Log("Started client");
        UpdateConnListServerRpc(NetworkManager.LocalClientId);
        Debug.Log(PlayerPrefs.GetString("PName"));
        Debug.Log(nwPlayers[0].PlayerName);

        //wait for the host to select before switching to the new scene
        
        //have them select a character here
    }
    
    
    /*public void onStartGameClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartClient();
        UpdateConnListServerRpc(NetworkManager.LocalClientId);
        Debug.Log(PlayerPrefs.GetString("PName"));
        Debug.Log(nwPlayers[0].PlayerName);
        //set player name 
        
        //check if the server is active
        
        //if(server)
        //client id will be in playerinfo

    }*/
    
    //HANDLERS
    
    private void HandleServerStarted()
    {
        throw new NotImplementedException();
    }
    
    [ServerRpc(RequireOwnership = false)]
    
    private void UpdateConnListServerRpc(ulong clientId)
    {
        nwPlayers.Add(new PlayerInfo(clientId, PlayerPrefs.GetString("PName"), false));
    }
    
}
