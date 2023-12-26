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

    private float horizontal;
    private bool isFacingRight = true;
    private Color defaultColor;
    private float coyoteJumpTimer;
    private float coyoteJumpTimerMax = 0.2f;
    private float jumpBufferingTimer;
    private float jumpBufferingTimerMax = 0.2f;

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
        SetPlayerColor(defaultColor);
        if (IsGrounded())
        {
            SetPlayerColor(Color.white);
        }

        HandleHorizontalMovement();

        HandleJumping();

        Flip();
    }

    private void HandleHorizontalMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void HandleJumping()
    {
        if (IsGrounded())
        {
            coyoteJumpTimer = coyoteJumpTimerMax;
        }
        else
        {
            coyoteJumpTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferingTimer = jumpBufferingTimerMax;
        }
        else
        {
            jumpBufferingTimer -= Time.deltaTime;
        }

        if (jumpBufferingTimer > 0f && coyoteJumpTimer > 0f)
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

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
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
