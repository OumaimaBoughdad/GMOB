using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveThePlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Speed at which the player moves
    [SerializeField] private Rigidbody2D rb; // Reference to the player's Rigidbody2D component
    private Vector2 movement; // To store player's movement input

    private void Update()
    {
        // Get input for movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize the movement vector to ensure consistent speed in all directions
        movement = movement.normalized;
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Implement behavior when the player collides with an enemy
            Debug.Log("Player collided with enemy!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            // Implement behavior when the player collects a coin
            Debug.Log("Player collected a coin!");
            Destroy(other.gameObject); // Destroy the coin GameObject
        }
    }
}
