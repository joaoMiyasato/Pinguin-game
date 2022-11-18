using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;
    public PlayerMovement pm;

    [Header("WallBump")]
    public float maxBumpTime;
    private float bumpTimer;

    public bool bumping = false;
    public bool afterBump = false;

    [Header("WallJumping")]
    public float wallJumpUpForce;
    public float wallJumpBackForce;
    public float wallMaxJumps;
    private float wallJumpCounter;

    public KeyCode jumpKey = KeyCode.Space;
    public bool wallJumping = false;

    [Header("Detection")]
    public float detectionLenght;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;


    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    float timer = 0.2f;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if(bumping)
        {
            WallBumping();
        }
        if(wallJumping)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                EndWallJumping();
            }
        }
        else
        {
            timer = 0.2f;
        }

        if(afterBump)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if(pm.grounded)
            {
                afterBump = false;
            }
        }
    }

    private void StateMachine()
    {
        if(wallFront && wallLookAngle < maxWallLookAngle && !pm.grounded)
        {
            if(!bumping && bumpTimer > 0 && new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > pm.normalSpeed)
            {
                StartWallBump();
            }

            if(bumpTimer > 0 && bumping)
            {
                bumpTimer -= Time.deltaTime;
            }
            if(bumpTimer < 0)
            {
                StopWallBump();
                afterBump = true;
            }
        }
        else
        {
            StopWallBump();
        }

        if(bumping && Input.GetKeyDown(jumpKey) && wallJumpCounter > 0)
        {
            rb.useGravity = true;
            bumping = false;
            WallJumping();
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLenght, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = /*frontWallHit.transform != lastWall || */Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if((wallFront && newWall) || pm.grounded)
        {
            bumpTimer = maxBumpTime;
            wallJumpCounter = wallMaxJumps;
        }
    }

    private void WallJumping()
    {
        wallJumping = false;
        wallJumping = true;
        wallJumpCounter--;
        Vector3 forceToApply = transform.up * wallJumpUpForce + frontWallHit.normal * wallJumpBackForce;

        rb.velocity = Vector3.zero;
        rb.AddForce(forceToApply, ForceMode.Impulse);

        orientation.forward = Vector3.Slerp(orientation.forward, frontWallHit.normal, Time.deltaTime * 300f);
    }

    private void EndWallJumping()
    {
        wallJumping = false;
    }

    private void StartWallBump()
    {
        bumping = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void WallBumping()
    {
        orientation.forward = Vector3.Slerp(orientation.forward, -frontWallHit.normal, Time.deltaTime * 100f);
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private void StopWallBump()
    {
        rb.useGravity = true;
        bumping = false;
    }
}
