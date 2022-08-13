using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementController : MonoBehaviour
{
    
    [Header("Parameters")]
    public float WalkSpeed = 4;
    public float SprintSpeed = 8;
    public float GravityPower = 0.5F;
    public float FloorGroundedDist = 1.2F;

    [Header( "Runtime" )]
    public bool CanMove = true;
    public bool Grounded = false;
    public bool Sprinting = false;
    
    
    [Header("References")]
    public FPSMouseLook MouseLook;
    public CharacterController ch;
    public Animator CamAnim;
    public LayerMask FloorLayer;
    
    
    private float currentSpeed = 1;
    private float animMovement;
    private Vector3 gravityVec;
    private bool WasInAir = false;



    public static PlayerMovementController instance;

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        MouseLook.Init(transform);    
    }

    void Update()
    {
        ProcessMovement();
        MouseLook.LookRotation(transform);
    }

    void ProcessMovement()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        bool sprintInput = Input.GetButton("Sprint");

        if( !CanMove )
        {
            verticalInput = 0;
            horizontalInput = 0;
            sprintInput = false;
        }
        

        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
        if (inputVector.sqrMagnitude > 1)
            inputVector.Normalize();


        Sprinting = sprintInput;


        if (Mathf.Abs(verticalInput) <= 0.01F && Mathf.Abs(horizontalInput) <= 0.01F)
        {
            currentSpeed = Mathf.Lerp(currentSpeed,0F,Time.deltaTime);
        }
        else
        {
            if (sprintInput)
            {
                currentSpeed = Mathf.Lerp(currentSpeed,SprintSpeed,Time.deltaTime * 8F);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed,WalkSpeed,Time.deltaTime * 8F);
            }
        }

        Vector3 desiredMove = transform.forward * inputVector.y  + transform.right * inputVector.x;
        desiredMove *= currentSpeed * Time.deltaTime;


        ProcessGravity();
        

        Vector3 movementVec = desiredMove + gravityVec;
        ch.Move(movementVec);


        
        // ANIM MOVEMENT
        float targetAnimMovement = 0;
        if (inputVector.sqrMagnitude > 0)
        {
            if (sprintInput)
            {
                animMovement = Mathf.Lerp(animMovement, 1F, Time.deltaTime * 6F);
            }
            else
            {
                animMovement = Mathf.Lerp(animMovement, 0.5F, Time.deltaTime * 4F);
            }
        }
        else
        {
            animMovement = Mathf.Lerp(animMovement, 0F, Time.deltaTime * 2F);
            if( animMovement < 0.05F )
                animMovement = 0F;
        }

        if(CamAnim != null)
            CamAnim.SetFloat("Movement",animMovement);

    }

    void ProcessGravity()
    {
        //Gravity
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down,out hit, FloorGroundedDist, FloorLayer);

        Grounded = (hit.collider != null);
        

        
        if (Grounded == false)
        {
            WasInAir = true;
            gravityVec += Vector3.down * GravityPower * Time.deltaTime;
        }
        else
        {
            gravityVec = Vector3.zero;
            if (WasInAir)
            {
                if(CamAnim != null)
                    CamAnim.Play("CamLand",-1,0);
                
                WasInAir = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Grounded)
        {
            Gizmos.color = new Color(1f, 0.44f, 0.38f);
            Gizmos.DrawLine(transform.position, transform.position+Vector3.down*FloorGroundedDist);
        }
        else
        {
            Gizmos.color = new Color(0.62f, 1f, 0.59f);
            Gizmos.DrawLine(transform.position, transform.position+Vector3.down*FloorGroundedDist);
        }
#endif
    }
}
