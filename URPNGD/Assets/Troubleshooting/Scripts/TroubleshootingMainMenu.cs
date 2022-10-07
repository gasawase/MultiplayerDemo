using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


// get an input, save it somewhere, have it display on the next screen
public class TroubleshootingMainMenu : NetworkBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    
    public void OnStartClicked()
    {
        // get string, add string to player info, load next scene
        PlayerPrefs.SetString("PName", playerName.text);
        Debug.Log(PlayerPrefs.GetString("PName"));
        Debug.Log(NetworkManager.LocalClientId);
        NetworkManager.Singleton.StartServer();
        Debug.Log("Started Server");
        //NetworkManager.Singleton.StartHost();
       // Debug.Log("Started Host");
        SceneManager.LoadScene("TestScene2");
    }

    public void OnClientClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        Debug.Log(PlayerPrefs.GetString("PName"));
        Debug.Log(NetworkManager.LocalClientId);
        NetworkManager.Singleton.StartClient();
    }
    
    
}
