using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Coin : MonoBehaviour
{
    [SerializeField] private float collectRadius = 1f;

    private void Update()
    {
        // Using distance check in Update (optional)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= collectRadius)
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        Score scoreManager = FindObjectOfType<Score>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(1); // Assuming each coin adds 1 to the score
        }

        Destroy(gameObject); // Destroy the coin object after collection
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin(); // Collect coin when player enters trigger
        }
    }
}
