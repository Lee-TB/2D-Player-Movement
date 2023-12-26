using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;
    private Color defaultColor;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpingPower = 20f;
    [SerializeField] private float speed = 10f;

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

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        Flip();

        if (IsGrounded())
        {
            SetPlayerColor(Color.white);
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void FixedUpdate()
    {
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapArea(groundCheck.position - (groundCheck.localScale * 0.48f), groundCheck.position + groundCheck.localScale * 0.48f, groundLayer);
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
