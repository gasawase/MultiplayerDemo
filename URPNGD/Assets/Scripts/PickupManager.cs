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
    private void OnTriggerEnter(Collider collision)
    {
        ulong colliderClientId = collision.gameObject.GetComponent<Player>().OwnerClientId;

        GameObject collidedGameObject = GameData.Instance.allPlayersSpawned[colliderClientId];
        //TODO switch case for what type of thing was picked up?
        //collidedGameObject.GetComponent<Player>().weaponArrLoc.Value = this.GetComponent<WeaponManager>().activeLoc; 
        //find this client id in the playerinfo
        PlayerInfo playerinfo = GameData.Instance.allPlayers[Convert.ToInt32(colliderClientId)];
        Debug.Log(playerinfo.m_PlayerName);

        WhenPlayerPickupClientRpc(colliderClientId);
        GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    public void WhenPlayerPickupClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        Player[] playerObj = FindObjectsOfType<Player>();
        foreach (Player playerGO in playerObj)
        {
            if (playerGO.OwnerClientId == clientId)
            {
                playerGO.listOfWeapons[GetComponent<WeaponManager>().activeLoc].SetActive(true);
            }
        }
        
        Debug.Log(GameData.Instance.allPlayersSpawned.Count);
        //collidedGameObject.GetComponent<Player>().listOfWeapons[GetComponent<WeaponManager>().activeLoc].SetActive(true);
    }
}
