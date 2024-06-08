using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]  private float moveSpeed = 3f;
    [SerializeField]  private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log($"Target set to {target.position}");
    }

    private void Update()
     {
         if (target != null)
         {
             Debug.Log("I am moving ");

             MoveTowardsTarget();
         }
         else
         {
             Debug.LogWarning("Target is null in Enemy script.");
         }
     }
    

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
        Debug.Log($"Enemy moving towards {target.position}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player touched the enemy!");
        }
    }
}
