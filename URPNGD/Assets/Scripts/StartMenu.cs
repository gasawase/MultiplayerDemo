using System;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : NetworkBehaviour
{
    //[SerializeField] private TMP_InputField playerName;
    public Button btnHost;
    public Button btnClient;
    public TMP_Text txtStatus;

    private void Start()
    {
        btnHost.onClick.AddListener(onHostClicked);
        btnClient.onClick.AddListener(onClientClicked);
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void onHostClicked()
    {
        btnClient.gameObject.SetActive(false);
        btnClient.gameObject.SetActive(false);
        txtStatus.text = "Starting Host";
        StartHost();
    }

    private void onClientClicked()
    {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        txtStatus.text = "Waiting on Host";
        NetworkManager.Singleton.StartClient();
    }

    // public void HostButtClicked()
    // {
    //     //PlayerPrefs.SetString("PName", playerName.text);
    //     NetworkManager.Singleton.StartHost();
    //     Debug.Log("Started Host");
    //     SceneManager.LoadScene("Lobby");
    //     
    //     //have them select a character here
    // }
    //
    // public void StartGameButtClicked()
    // {
    //     //PlayerPrefs.SetString("PName", playerName.text);
    //     NetworkManager.Singleton.StartClient();
    //     Debug.Log("Started client");
    //
    //     //wait for the host to select before switching to the new scene
    //     
    //     //have them select a character here
    // }
    //
    // public void ServerButtonClicked()
    // {
    //     NetworkManager.Singleton.StartServer();
    // }
    
    
    

}
