using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player : NetworkBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    public Camera camObj;
    public Transform camT;

    private CharacterController _mpCharacterController;

    private float mouseSensitivity; //move this to player settings later

    private Animator animator;
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
        else
        {
            camObj.enabled = false;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
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
}
