using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    public string weaponName = "";

    [SerializeField] private GameObject[] listOfWeapons;
    
    //weapon struct info network variable HERE
    //public NetworkVariable<int> activeLoc = new NetworkVariable<int>();
    public int activeLoc;
    
    private void Start()
    {
        // get whatever child is active and get its attributes
        for (int i = 0; i< gameObject.transform.childCount; i++)
        {
            if(gameObject.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                //this is where you get the information and send it to the client
                weaponName = gameObject.transform.GetChild(i).gameObject.name;
                //activeLoc.Value = i;
                activeLoc = i;
            }
        }
    }
    
}
