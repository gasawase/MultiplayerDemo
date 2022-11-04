using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour {


    public NetworkVariable<Vector3> PositionChange = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> RotationChange = new NetworkVariable<Vector3>();
    //public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> pScore = new NetworkVariable<int>(50);
    public TMP_Text txtScoreDisplay;
    
    private GameManager _gameMgr;
    private Camera _camera;
    public float movementSpeed = .5f;
    private float rotationSpeed = 1f;
    //private BulletSpawner _bulletSpawner;
    

    private void Start() {
        //ApplyPlayerColor();
        //PlayerColor.OnValueChanged += OnPlayerColorChanged;
    }
    
    void Update() {
        if (IsOwner) {
            Vector3[] results = CalcMovement();
            RequestPositionForMovementServerRpc(results[0], results[1]);
            // if (Input.GetButtonDown("Fire1")) {
            //     _bulletSpawner.FireServerRpc();
            // }
        }

        if(!IsOwner || IsHost){
            transform.Translate(PositionChange.Value);
            transform.Rotate(RotationChange.Value);
        }
    }

    public override void OnNetworkSpawn() {
        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.enabled = IsOwner;

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
        pScore.Value = value;
    }
    // public void OnPlayerColorChanged(Color previous, Color current) {
    //     ApplyPlayerColor();
    // }

    // public void ApplyPlayerColor() {
    //     GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
    //     //transform.Find("LArm").GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
    //     transform.Find("RArm").GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
    // }


    // horiz changes y rotation or x movement if shift down, vertical moves forward and back.
    private Vector3[] CalcMovement() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");
        float y_rot = 0.0f;

        if (isShiftKeyDown) {
            x_move = Input.GetAxis("Horizontal");
        } else {
            y_rot = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed;

        Vector3 rotVect = new Vector3(0, y_rot, 0);
        rotVect *= rotationSpeed;

        return new[] { moveVect, rotVect };
    }
    

    // public void DisplayScore()
    // {
    //     txtScoreDisplay.text = pScore.Value.ToString();
    // }
}