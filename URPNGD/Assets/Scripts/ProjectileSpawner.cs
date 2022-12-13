using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileSpawner : NetworkBehaviour
{
    public NetworkVariable<int> networkedDamage = new NetworkVariable<int>(5);
    public Rigidbody projectileRB;
    private float weaponSpeed = 20f;
    private int maxDamage = 20;
    
    //TODO do a check for if they have a weapon active. if so, do the thrown weapon vs the magic <-- do this on the player
    
    [ServerRpc]
    public void ThrownWeaponServerRpc(ServerRpcParams rpcParams = default) {
        Rigidbody newWeapon = Instantiate(projectileRB, transform.position, transform.rotation);

        newWeapon.velocity = transform.forward * weaponSpeed;
        newWeapon.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        newWeapon.GetComponent<WeaponInfo>().damageNWVar.Value = networkedDamage.Value;
        Destroy(newWeapon.gameObject, 3);
    }
    
    [ServerRpc]
    public void MagicProjectileServerRpc(ServerRpcParams rpcParams = default) {
        Rigidbody newMagicSpawn = Instantiate(projectileRB, transform.position, transform.rotation);

        newMagicSpawn.velocity = transform.forward * weaponSpeed;
        newMagicSpawn.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        newMagicSpawn.GetComponent<MagicProjectile>().damageNWVar.Value = networkedDamage.Value;
        Destroy(newMagicSpawn.gameObject, 3);
    }
    
}
