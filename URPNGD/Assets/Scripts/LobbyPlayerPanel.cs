using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] public TMP_Text playerName;
    //[SerializeField] public GameObject playerModel; 
    [SerializeField] public Toggle readyIcon;
    [SerializeField] public GameObject waitingPanel;
    [SerializeField] public GameObject playerInfoPanel;


    internal void UpdatePlayerName(TMP_Text playerNameIn)
    {
        playerName = playerNameIn;
    }
}
