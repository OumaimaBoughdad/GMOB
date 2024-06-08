using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private CoinManager coinManager;

    // Set reference to the CoinManager
    public void SetCoinManager(CoinManager manager)
    {
        coinManager = manager;
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Destroy the coin
            Destroy(gameObject);

            // Remove coin position from the CoinManager
            Vector3Int coinPosition = coinManager.floorTile.WorldToCell(transform.position);
            coinManager.RemoveCoinPosition(coinPosition);
        }
    }
}
