using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float groundSpeed;
    public float jumpSpeed;
    public float acceleration;
    [Range(0f, 1f)]
    public float groundDecay;
    public Rigidbody2D body;
    [Header("Groundcheck")]
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    public bool grounded;
    float xInput;
    float yInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetIput();
        HandleJump();
    }
    void FixedUpdate()
    {
        CheckGround();
        ApplyFriction();
        MoveWithInput();


    }


    void GetIput()
    {
        xInput = Input.GetAxis("Horizontal");
        // yInput = Input.GetAxis("Vertical");
    }

    void MoveWithInput()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(body.velocity.x + increment, -groundSpeed, groundSpeed);
            body.velocity = new Vector2(newSpeed, body.velocity.y);
            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        }
    }
    void CheckGround()
    {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }
    void ApplyFriction()
    {
        if (grounded && xInput == 0 && body.velocity.y <= 0)
        {
            body.velocity *= groundDecay;
        }

    }
}