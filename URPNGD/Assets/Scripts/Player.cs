using System;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour {
    public NetworkVariable<Vector3> PositionChange = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> RotationChange = new NetworkVariable<Vector3>();
    //public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> PlayerMeshInt = new NetworkVariable<int>();
    public NetworkVariable<int> pScore = new NetworkVariable<int>(50);
    public NetworkVariable<int> weaponArrLoc = new NetworkVariable<int>();

    //public NetworkVariable<int> weaponObjArray = new NetworkVariable<int>();

    public TMP_Text txtScoreDisplay;
    [SerializeField] public Mesh[] listOfMeshes;
    [SerializeField] public Sprite[] listOfSprites;
    [SerializeField] public GameObject[] listOfWeapons;
    [SerializeField] public GameObject meshHolder;
    [SerializeField] public GameObject cameraGameObject;
    [SerializeField] public CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] public TMP_Text playerNameTxt;
    //[SerializeField] public CharacterController mpCharController;
    
    private GameManager _gameMgr;
    //private CharacterController mpCharController;
    //private BulletSpawner _bulletSpawner;

    public override void OnNetworkSpawn() {
        cameraGameObject.GetComponent<Camera>().enabled = IsOwner;
        cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = IsOwner;
        //pScore.OnValueChanged += ClientOnScoreChanged;
        //Make a more effecient way to find the spawner. Player>PlayerMeshes>Root>Hips>Spine_01>Spine_02>Spine_03>Clavicle_R>Shoulder_R>Elbow_R>Hand_R>ItemSpawningLocation
        //_bulletSpawner = transform.Find("RArm").transform.Find("BulletSpawner").GetComponent<BulletSpawner>();
        // if (IsHost)
        // {
        //     _bulletSpawner.bulletDamage.Value = 1;
        // }

        //DisplayScore();
    }

    private void HostHandleBulletCollision(GameObject bullet)
    {
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        pScore.Value -= bulletScript.damage.Value;

        ulong ownerClientId = bullet.GetComponent<NetworkObject>().OwnerClientId;
        Player otherPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject.GetComponent<Player>();
        otherPlayer.pScore.Value += 1;
        
        Destroy(bullet);          
    }

    private void HostHandleDamageBoostPickup(Collider collision)
    {
        // if (!_bulletSpawner.isAtMaxDamage())
        // {
        //     _bulletSpawner.IncreaseDamage();
        //     collision.GetComponent<NetworkObject>().Despawn();
        // }

    }

    // private void ClientOnScoreChanged(int previous, int current)
    // {
    //     DisplayScore();
    // }

    public void OnCollisionEnter(Collision collision)
    {
        if (IsHost)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                HostHandleBulletCollision(collision.gameObject);
            }

        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (IsHost)
        {
            if (collision.gameObject.CompareTag("DamageBoost"))
            {
                HostHandleDamageBoostPickup(collision);
            }

            if (collision.gameObject.CompareTag("Weapon"))
            {
                //TODO in the code it will run a switch case or checking and getting the weapon class info or something to handle to weapon and which is enabled
                //ActivateWeaponOnPlayerClientRpc(collision);
            }
        }
    }
    
    private void HostHandleWeaponPickup(Collider collision)
    {
        WeaponManager weaponScript = collision.GetComponent<WeaponManager>();
        weaponArrLoc.Value = weaponScript.activeLoc;
        listOfWeapons[weaponArrLoc.Value].SetActive(true);
    }

    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 posChange, Vector3 rotChange) {
        if (!IsServer && !IsHost) return;

        PositionChange.Value = posChange;
        RotationChange.Value = rotChange;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSetScoreServerRpc(int value)
    {
        pScore.Value = value;
    }

    // horiz changes y rotation or x movement if shift down, vertical moves forward and back.


    // public void DisplayScore()
    // {
    //     txtScoreDisplay.text = pScore.Value.ToString();
    // }
}