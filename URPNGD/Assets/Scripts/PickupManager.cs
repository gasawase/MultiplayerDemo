using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// detects what this object is, what collided with it, and sends a clientrpc 
/// </summary>
public class PickupManager : NetworkBehaviour
{
    private GameManager _gameManager;
    private GameData _gameData;
    private int typeOfWManagerInt;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ulong colliderClientId = collision.gameObject.GetComponent<Player>().OwnerClientId;

            GameObject collidedGameObject = GameData.Instance.allPlayersSpawned[colliderClientId];
            //TODO switch case for what type of thing was picked up?
            //find this client id in the playerinfo
            PlayerInfo playerinfo = GameData.Instance.allPlayers[Convert.ToInt32(colliderClientId)];
            //get the type of weapon manager
            //switch case as to assigning the int for the thingy
            switch (this.name)
            {
                case "TwoHandedWeapons":
                    typeOfWManagerInt = 0;
                    break;
                case "SingleHandedWeapons":
                    typeOfWManagerInt = 1;
                    break;
                case "ThrowableWeapons":
                    typeOfWManagerInt = 2;
                    break;
            }
            WhenPlayerPickupClientRpc(colliderClientId, typeOfWManagerInt);
            GetComponent<NetworkObject>().Despawn();            
        }
    }

    [ClientRpc]
    public void WhenPlayerPickupClientRpc(ulong clientId, int typeOfWManager, ClientRpcParams clientRpcParams = default)
    {
        Player[] playerObj = FindObjectsOfType<Player>();
        foreach (Player playerGO in playerObj)
        {
            if (playerGO.OwnerClientId == clientId)
            {
                switch (typeOfWManager)
                {
                    case 0:
                        playerGO.listOfDoubHandWeapons[GetComponent<WeaponManager>().activeLoc].SetActive(true);
                        break;
                    case 1:
                        playerGO.listOfSingHandWeapons[GetComponent<WeaponManager>().activeLoc].SetActive(true);
                        break;
                    case 2:
                        playerGO.listOfThrownWeapons[GetComponent<WeaponManager>().activeLoc].SetActive(true);
                        break;
                }
            }
        }
    }
}
