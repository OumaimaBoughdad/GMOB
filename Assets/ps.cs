using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ps : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb; // Reference to the Rigidbody2D component
    [SerializeField] private Vector2 initialPosition = new Vector2(2, 3); // Initial position for the Rigidbody2D
    [SerializeField] private float moveSpeed = 5f; // Speed at which the player moves
    [SerializeField] private Vector2 moveDirection = Vector2.right; // Direction of automatic movement
    [SerializeField] private int numberOfSteps = 5; // Number of steps to move

    private int stepsRemaining; // Number of steps remaining

    // Start is called before the first frame update
    void Start()
    {
        if (rb != null)
        {
            rb.position = initialPosition; // Set the position of the Rigidbody2D
        }
        else
        {
            Debug.LogError("Rigidbody2D is not assigned.");
        }

        stepsRemaining = numberOfSteps; // Initialize stepsRemaining
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there are steps remaining
        if (stepsRemaining > 0)
        {
            // Calculate movement vector
            Vector2 movement = moveDirection.normalized * moveSpeed;

            // Apply the movement to the Rigidbody2D
            rb.velocity = movement;

            // Decrement stepsRemaining
            stepsRemaining--;
        }
        else
        {
            // Stop moving if no steps remaining
            rb.velocity = Vector2.zero;
        }
    }
}
