using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MPPlayerMovement : NetworkBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    public Transform camT;

    private CharacterController mpCharController;
    // Start is called before the first frame update
    void Start()
    {
        mpCharController = GetComponent<CharacterController>();
        
        if (IsLocalPlayer)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
            camT.GetComponent<Camera>().enabled = false;
            
        }

        /*if (!IsLocalPlayer) //doesn't work and does the same thing if this portion is commented out
        {
            camT.GetComponent<Camera>().enabled = false;
            
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            MPMovePlayer();
        }


        /*if (!IsLocalPlayer)
        {
            camT.GetComponent<Camera>().enabled = false;
        }
        else
        {
            camT.GetComponent<Camera>().enabled = true;
        }*/
        
    }

    void MPMovePlayer()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime, 0);
        Vector3 moveVect = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        mpCharController.SimpleMove(moveVect * movementSpeed);
    }
}
