using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    WallJump wj;

    [Header("Movement")]
    public float acceleration;
    public float normalSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;
    bool sprinting = false;
    public bool sliding = false;

    [Header("Colliders")]
    public Collider foot;
    public PhysicMaterial normalMaterial;
    public PhysicMaterial slipperyMaterial;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode slideKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public float adjust;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;
    
    float horizontaInput;
    float verticalInput;

    Vector3 moveDir;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();
        wj = GetComponent<WallJump>();
    }

    void Update()
    {
        grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y - adjust, transform.position.z), Vector3.down, playerHeight * 0.5f + 0.5f, whatIsGround); 

        MyInput();
        SpeedControl();

        if(grounded)
        {
            if(!sliding)
            {
                rb.drag = groundDrag;
                foot.material = normalMaterial;
            }
            else
            {
                rb.drag = 0;
                foot.material = slipperyMaterial;
            }
        }
        else
        {
            rb.drag = 0;
        }
    }
    void FixedUpdate()
    {
        if(!sliding)
            MovePlayer();
    }

    void MyInput()
    {
        horizontaInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(horizontaInput != 0f || verticalInput != 0f)
        {
            if(grounded && !sliding)
            {
                anim.SetBool("Walk", true);
            }
            else
            {
                anim.SetBool("Walk", false);
            }
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKey(sprintKey))
        {
            sprinting = true;
        }
        else
        {
            sprinting = false;
        }

        if(Input.GetKey(slideKey) && grounded)
        {
            if(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 2f)
            {
                sliding = true;
                anim.SetBool("Slide", true);
            }
        }
        else
        {
            sliding = false;
                anim.SetBool("Slide", false);
        }
    }

    void MovePlayer()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontaInput;

        if(grounded)
        {
            rb.AddForce(moveDir.normalized * acceleration * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDir.normalized * acceleration * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(!sliding)
        {
            if(flatVel.magnitude > normalSpeed && !sprinting && !wj.wallJumping && grounded)
            {
                Vector3 limitedVel = flatVel.normalized * normalSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            else if(flatVel.magnitude > sprintSpeed && sprinting && !wj.wallJumping && grounded)
            {
                Vector3 limitedVel = flatVel.normalized * sprintSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            else if(!grounded && !wj.wallJumping)
            {
                Vector3 limitedVel = flatVel.normalized * sprintSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            else if(wj.wallJumping)
            {
                Vector3 limitedVel = flatVel.normalized * wj.wallJumpBackForce;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
    }

    void Jump()
    {
        // Debug.Log("Jumping");
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + adjust - playerHeight * 0.5f + 0.5f, transform.position.z));
    }
}
