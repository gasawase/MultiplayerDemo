using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class StartMenu : NetworkBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    

    private void Start()
    {

    }

    public void HostButtClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started");
        SceneManager.LoadScene("Lobby");
        
        //have them select a character here
    }

    public void StartGameButtClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client Started");
        
        //wait for the host to select before switching to the new scene
        
        //have them select a character here
    }
}
