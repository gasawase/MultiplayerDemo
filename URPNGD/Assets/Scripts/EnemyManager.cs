using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : NetworkBehaviour
{
    [SerializeField] private Slider refToEnHealthBar;
    public NetworkVariable<int> enHealth;
    public NetworkVariable<int> regularHitDamage = new NetworkVariable<int>();
    void Start()
    {
        refToEnHealthBar.value = enHealth.Value;
    }

    public override void OnNetworkSpawn()
    {
        enHealth.OnValueChanged += ClientViewOnHealthChanged;
        DisplayHealthBar();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //checking if ishost so it only does this on the server; handling for clients is done later
        if (IsHost)
        {
            if (collision.gameObject.CompareTag("Projectile"))
            {
                HostHandleProjectileCollision(collision.gameObject);
            }
        }
    }

    public void DisplayHealthBar()
    {
        refToEnHealthBar.value = enHealth.Value;
    }

    private void HostHandleProjectileCollision(GameObject projectile)
    {
        MagicProjectile projectileScript = projectile.GetComponent<MagicProjectile>();
        enHealth.Value -= projectileScript.damageNWVar.Value;
        
        Destroy(projectile);
    }

    private void ClientViewOnHealthChanged(int previousvalue, int newvalue)
    {
        DisplayHealthBar();
    }
    
}
