using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manages the display itself of each player panel on the side of the player HUDs
/// </summary>
public class PlayerPanelDisplay : MonoBehaviour
{
    [SerializeField] public TMPro.TMP_Text txtName;
    [SerializeField] public Sprite[] listOfSprites;
    [SerializeField] public GameObject spriteImageGO;
    [SerializeField] public Slider playerHealth;

    public ulong personalClientId;

    public void SetName(string newName)
    {
        txtName.text = newName;
    }

    public void SetSpriteLoc(int i)
    {
        spriteImageGO.GetComponent<Image>().sprite = listOfSprites[i];
    }

    public void RefreshHealth(int playHealth)
    {
        playerHealth.value = playHealth;
    }

    public void SetClientId(ulong clientId)
    {
        personalClientId = clientId;
    }

    public ulong GetClientId()
    {
        return personalClientId;
    }
}
