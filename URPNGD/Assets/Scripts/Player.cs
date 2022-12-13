using System;
using Cinemachine;
using StarterAssets;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif


public class Player : NetworkBehaviour {
    public NetworkVariable<Vector3> PositionChange = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> RotationChange = new NetworkVariable<Vector3>();
    //public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> PlayerMeshInt = new NetworkVariable<int>();
    public NetworkVariable<int> playerHealth = new NetworkVariable<int>(100);
    public NetworkVariable<int> weaponArrLoc = new NetworkVariable<int>();
    

    //UI
    public Slider healthSlider;
    [SerializeField] public Mesh[] listOfMeshes;
    [SerializeField] public Sprite[] listOfSprites;
    [SerializeField] public TMP_Text playerNameTxt;
    
    [SerializeField] public GameObject[] listOfDoubHandWeapons;
    [SerializeField] public GameObject[] listOfSingHandWeapons;
    [SerializeField] public GameObject[] listOfThrownWeapons;
    [SerializeField] public GameObject meshHolder;
    [SerializeField] public GameObject cameraGameObject;
    [SerializeField] public CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] public ProjectileSpawner _projectileSpawner;
    
    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animWeaponType;
    private int _animIsMagic;

    private Animator _animator;
    private bool _hasAnimator;
    private GameManager _gameMgr;
    //private CharacterController mpCharController;
    //private BulletSpawner _bulletSpawner;

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();
    }
    
    public override void OnNetworkSpawn() {
        _hasAnimator = TryGetComponent(out _animator);
        cameraGameObject.GetComponent<Camera>().enabled = IsOwner;
        cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = IsOwner;
        playerHealth.OnValueChanged += ClientOnScoreChanged;
        foreach (PlayerInfo player in GameData.Instance.allPlayers)
        {
            if (player.playMeshSelect == 5 || player.playMeshSelect == 10 || player.playMeshSelect == 11)
            {
                _animator.SetBool(_animIsMagic, true);
            }
            _animator.SetBool(_animIsMagic, false);
        }
        //Make a more effecient way to find the spawner. Player>PlayerMeshes>Root>Hips>Spine_01>Spine_02>Spine_03>Clavicle_R>Shoulder_R>Elbow_R>Hand_R>ItemSpawningLocation
        //_bulletSpawner = transform.Find("RArm").transform.Find("BulletSpawner").GetComponent<BulletSpawner>();
        // if (IsHost)
        // {
        //     _bulletSpawner.bulletDamage.Value = 1;
        // }

        DisplayScore();
    }

    private void HostHandleBulletCollision(GameObject bullet)
    {
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        playerHealth.Value -= bulletScript.damage.Value;

        ulong ownerClientId = bullet.GetComponent<NetworkObject>().OwnerClientId;
        Player otherPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject.GetComponent<Player>();
        otherPlayer.playerHealth.Value += 1;
        
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

    private void ClientOnScoreChanged(int previous, int current)
    {
        DisplayScore();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (IsHost)
        {
            // if (collision.gameObject.CompareTag("Bullet"))
            // {
            //     HostHandleBulletCollision(collision.gameObject);
            // }
            if (collision.gameObject.CompareTag("Projectile"))
            {
                //this currently won't work for thrown objects because it will conflict and send the wrong message if the player runs over the object
            }
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (IsHost)
        {
            if (collision.gameObject.CompareTag("Weapon"))
            {
                //TODO in the code it will run a switch case or checking and getting the weapon class info or something to handle to weapon and which is enabled
                //ActivateWeaponOnPlayerClientRpc(collision);
            }
        }
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
        playerHealth.Value = value;
    }

    // horiz changes y rotation or x movement if shift down, vertical moves forward and back.


    public void DisplayScore()
    {
        healthSlider.value = playerHealth.Value;
    }
    
    //////// ANIMATION CONTROLLERS ////////
    
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animWeaponType = Animator.StringToHash("weaponType");
        _animIsMagic = Animator.StringToHash("isMagic");
        //_animIDGrounded = Animator.StringToHash("Grounded");
        //_animIDJump = Animator.StringToHash("Jump");
        //_animIDFreeFall = Animator.StringToHash("FreeFall");
        //_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}