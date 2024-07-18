using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7.5f;
    public ParticleSystem trailFX;


    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpTime = 0.5f;

    [Header("Turn Check")]
    [SerializeField] private GameObject DirL;
    [SerializeField] private GameObject DirR;

    [Header("Ground Check")]
    [SerializeField] private float extraHeight = 0.25f;
    [SerializeField] private LayerMask whatIsGround;

    [HideInInspector] public bool IsFacingRight;
    private Rigidbody2D rb;
    private Collider2D coll;

    private Animator anim;
    private float moveInput;

    private bool IsJumping;
    private bool IsFalling;
    private float JumpTimeCounter;
    private RaycastHit2D groundHit;
    private Coroutine resetTriggerCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();


        StartDirectionCheck();

    }

    private void Update()
    {

        Move();
        Jump();


    }

    #region Movement
    private void Move()
    {

        moveInput = UserInput.instance.moveInput.x;

        if (moveInput > 0 || moveInput < 0)
        {
            anim.SetBool("IsWalking", true);
            TurnCheck();
            Dust();

        }

        else
        {
            anim.SetBool("IsWalking", false);

        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

    }

    private void Dust()
{       if (IsGrounded())
        {
            trailFX.Play();
        }
        else
        {
            trailFX.Stop();
        }
}

    private void Jump()
    {
        //button was pushed this frame
        if (UserInput.instance.controls.Jumping.Jump.WasPressedThisFrame() && IsGrounded())
        {
            IsJumping = true;
            JumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            anim.SetTrigger("jump");
            trailFX.Stop();

        }

        //button is held
        if (UserInput.instance.controls.Jumping.Jump.IsPressed())
        {
            if (JumpTimeCounter > 0 && IsJumping)
            {

                // rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                rb.velocity = new Vector2(0, jumpForce);
                JumpTimeCounter -= Time.deltaTime;
            }
            else if(JumpTimeCounter == 0) 
            {
                IsFalling = true;
                IsJumping = false;
            }
            else
            {
                IsJumping = false;
                anim.ResetTrigger("jump");
                
           }
        }


        //button was released this frame    
        if (UserInput.instance.controls.Jumping.Jump.WasReleasedThisFrame())
        {
            IsJumping = false;
            IsFalling = true;
        }

        if(!IsJumping && CheckForLand())
        {
            anim.SetTrigger("land");
            resetTriggerCoroutine = StartCoroutine(Reset());

        }

        DrawGroundCheck();

    }

    #endregion
    #region Ground/Landed Check

    private bool IsGrounded()
    {
        groundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, extraHeight, whatIsGround);

        if (groundHit.collider != null)
        {
            return true;

        }
        else
        {
            return false;
        }
    }
    private bool CheckForLand()
    {
        if (IsFalling)
        {
            if (IsGrounded())
            {
                //player has landed
                IsFalling = false;

                return true;
            }

            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }
        
    }
private IEnumerator Reset()
    {
        yield return null;

        anim.ResetTrigger("land");

    }

#endregion
#region Turn Checks
    private void StartDirectionCheck()
    {

        if (DirR.transform.position.x > DirL.transform.position.x)
        {
            IsFacingRight = true;
        }

        else
        {
            IsFacingRight = false;
        }

    }

    private void TurnCheck()
    {
        if (UserInput.instance.moveInput.x > 0 && !IsFacingRight)
        {
            Turn();
        }

        else if (UserInput.instance.moveInput.x < 0 && IsFacingRight)
        {
            Turn();
        }
    }

    private void Turn()
    {
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }

        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
    }
    #endregion
#region Debug Functions
    private void DrawGroundCheck()
    {   
        Color rayColor;
        if (IsGrounded())
        {
            rayColor = Color.green;
        }
        else
        {
        rayColor = Color.red;
        }
        
    Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
    Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
    Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + extraHeight), Vector2.right * (coll.bounds.extents.x * 2), rayColor);
    }
#endregion
}