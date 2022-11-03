using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player : NetworkBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    public GameObject camObj;
    public Transform camT;

    private CharacterController _mpCharacterController;
    [SerializeField] private Mesh[] characterChoicesMesh;

    private float mouseSensitivity; //move this to player settings later

    private Animator animator;
    private NetworkVariable<Byte> charIndex = new NetworkVariable<byte>();


    // Start is called before the first frame update
    void Start()
    {
        _mpCharacterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        
        if(IsLocalPlayer)
        {
            //TODO: let player choose their avatar; have to figure out selecting a mesh and assigning to a specific client id
            //set a mesh
        }

    }

    private void Awake()
    {
        if (IsOwner)
        {
            camObj.GetComponent<Camera>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            Move();
        }
        
    }

    public void Move()
    {
        
        //transform.Rotate(Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime, 0);
        Vector3 moveVect = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        moveVect = Quaternion.AngleAxis(camT.rotation.eulerAngles.y, Vector3.up) * moveVect;
        _mpCharacterController.SimpleMove(moveVect * movementSpeed);

        if (moveVect != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    //setting the player character mesh
    /*private void OnEnable()
    {
        //start listening for the char index being updated
        charIndex.OnValueChanged += OnPlayerTypeSelected;
    }

    private void OnDisable()
    {
        // stop listening for the char index being updated
        charIndex.OnValueChanged -= OnPlayerTypeSelected;
    }*/

    private void OnPlayerTypeSelected(byte oldCharIndex, byte newCharIndex)
    {
        //only clients need to update the renderer
        if (!IsClient)
        {
            return; 
        }
        
        //update the mesh based on player's choice
        GetComponent<SkinnedMeshRenderer>().sharedMesh = characterChoicesMesh[newCharIndex];

    }
    
    
    ///
    /// SERVER RPC
    /// 
    
    [ServerRpc]
    public void SetPlayerServerRpc(byte newPlayerCharIndex)//could use a byte if the network gets slow
    {
        //make sure hte newcharindex is valid
        if (newPlayerCharIndex > 11) { return; }
    
        //update the new charindex networkVariable
        charIndex.Value = newPlayerCharIndex;
    }
    
}
