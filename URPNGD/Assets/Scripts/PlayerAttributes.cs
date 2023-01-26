using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manages the health and damage of the players
/// the health is over a network variable so when it's changed,
/// everyone on the network knows
/// </summary>
public class PlayerAttributes : NetworkBehaviour
{
    public Slider hpBar;

    private int maxHP = 100;

    private NetworkVariable<int> currentHp = new NetworkVariable<int>();

    private void Awake()
    {
        currentHp.Value = maxHP;
        currentHp.OnValueChanged += ClientOnValueChanged;
    }

    private void ClientOnValueChanged(int previousvalue, int newvalue)
    {
        hpBar.value = currentHp.Value;
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Damager")) // "Damager" should be attached to anything that can damage the player
        {
            Debug.Log($"player hit by {collision.gameObject}");
            ServerDamageHandlerForPlayers(collision.gameObject);
        }
    }
    
    public void ServerDamageHandlerForPlayers(GameObject damagerObject)
    {
        // check if this is an Enemy
        if (damagerObject.GetComponent<EnemyManager>())
        {
            Debug.Log($"this is an enemy hit");
            EnemyManager enemyManager = damagerObject.GetComponent<EnemyManager>();
            int damageVal = enemyManager.regularHitDamage.Value;
            TakeDamageServerRpc(damageVal);
            Debug.Log($"{currentHp.Value}");
            //DisplayHealth();
            //DisplayHealthServerRpc();
            if (currentHp.Value <= 0)
            {
                //play death animation
                //black screen fade in
                RespawnPlayerServerRpc();
                ResetPlayerLocationClientRpc();
            }
            //trigger animation hit
        }
    }

    // tell the server that you've taken damage
    [ServerRpc]
    private void TakeDamageServerRpc(int damage)
    {
        currentHp.Value -= damage;
    }
    
    
    // tell the server that your health is reset
    [ServerRpc]
    void RespawnPlayerServerRpc()
    {
        // set health to 100%
        currentHp.Value = maxHP;
        //hpBar.value = maxHP; //should be covered by ClientOnValueChanged
    }
    
    // tell all the clients that a specific player has a new location
    [ClientRpc]
    void ResetPlayerLocationClientRpc()
    {
        //reset player position to spawn point
        GetComponent<CharacterController>().enabled = false;
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnLocation");
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[index].transform.position;
        GetComponent<CharacterController>().enabled = true;
    }
}
