using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpingPower = 20f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private float horizontal;
    private bool isFacingRight = true;
    private Color defaultColor;

    private bool isJumping;
    private float coyoteJumpTimer;
    private float coyoteJumpTimerMax = 0.2f;
    private float jumpBufferingTimer;
    private float jumpBufferingTimerMax = 0.2f;

    private int maxJumpNumber = 5;
    private int jumpNumber;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTimer;
    private float wallJumpingTimerMax = 0.2f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Square")
            {
                defaultColor = child.GetComponent<SpriteRenderer>().color;
            }
        }
    }

    private void Update()
    {
        ChangePlayerColor();

        if (!isWallJumping)
        {
            HorizontalMovement();
        }


        Jumping();

        WallSlide();

        WallJumping();
    }

    private void ChangePlayerColor()
    {
        SetPlayerColor(defaultColor);
        if (IsGrounded())
        {
            SetPlayerColor(Color.white);
        }

        if (isWallJumping)
        {
            SetPlayerColor(Color.red);
        }

        if (isWallSliding)
        {
            SetPlayerColor(Color.cyan);
        }
    }

    private void HorizontalMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        // Flip
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void Jumping()
    {
        if (IsGrounded())
        {
            coyoteJumpTimer = coyoteJumpTimerMax;
            jumpNumber = maxJumpNumber;
            isJumping = false;
        }
        else
        {
            coyoteJumpTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferingTimer = jumpBufferingTimerMax;
            jumpNumber--;
        }
        else
        {
            jumpBufferingTimer -= Time.deltaTime;
        }

        // Ground Jump
        if (jumpBufferingTimer > 0f && coyoteJumpTimer > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            isJumping = true;
        }

        // Air Jump
        if (isJumping && jumpBufferingTimer > 0f && jumpNumber > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }


        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteJumpTimer = 0f; // avoid multi-jumping
            jumpBufferingTimer = 0f;
        }
    }

    private bool IsGrounded()
    {
        float playerRadius = 0.5f;
        return Physics2D.OverlapArea(new Vector2(groundCheck.position.x - playerRadius, groundCheck.position.y), new Vector2(groundCheck.position.x + playerRadius, groundCheck.position.y), groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapArea(new Vector2(wallCheck.position.x, wallCheck.position.y - 0.5f), new Vector2(wallCheck.position.x, wallCheck.position.y + 0.5f), wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(0f, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJumping()
    {
        if (isWallSliding)
        {
            isWallSliding = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingTimer = wallJumpingTimerMax;
        }
        else
        {
            wallJumpingTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingTimer > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingTimer = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }

        if (IsGrounded() || IsWalled())
        {
            isWallJumping = false;
        }
    }


    private void SetPlayerColor(Color color)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Square")
            {
                child.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }
}
