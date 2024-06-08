using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Reference to the player prefab
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 0, 0); // Default spawn position

    void Start()
    {
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player prefab is not assigned.");
        }
    }
}
