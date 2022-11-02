using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] public TMP_Text txtName;
    [SerializeField] public TMP_Text txtReady;
    //[SerializeField] public GameObject playerModel; 
    //[SerializeField] public Toggle readyIcon;
    //[SerializeField] public GameObject waitingPanel;
    //[SerializeField] public GameObject playerInfoPanel;

    private bool isReady = false;

    private void Start()
    {
        SetReady(isReady);
    }

    public void SetName(string newName)
    {
        txtName.text = newName;
    }

    public string GetName()
    {
        return txtName.text;
    }

    public void SetReady(bool ready)
    {
        isReady = ready;
        if (isReady)
        {
            txtReady.text = "Ready";
        }
        else
        {
            txtReady.text = "Not Ready";
        }
    }


    // internal void UpdatePlayerName(TMP_Text playerNameIn)
    // {
    //     playerName = playerNameIn;
    // }
}
