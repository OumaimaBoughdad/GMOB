using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
   // [SerializeField] private GameObject bossPrefab;
    public int maxRegularEnemiesPerRoom = 2;
   // public bool spawnBoss = true;
    public Tilemap floorTile;
    public int borderMargin = 1;

    private List<Vector3Int> validPositions = new List<Vector3Int>();

    //used for what 
    private Transform playerTransform;
    public Transform enemyTarget;

    public void GenerateEnemiesInRoom(BoundsInt roomBounds, HashSet<Vector3Int> coinPositions)
    {
        ClearEnemies();

        HashSet<Vector3Int> generatedPositions = new HashSet<Vector3Int>();
        CollectValidPositions(roomBounds);

        for (int i = 0; i < maxRegularEnemiesPerRoom; i++)
        {
            Vector3Int randomPosition = GetRandomValidPosition(generatedPositions, coinPositions);
            if (randomPosition != Vector3Int.zero)
            {
                InstantiateEnemy(enemyPrefab, randomPosition);
                generatedPositions.Add(randomPosition);
            }
        }
    }

    private void CollectValidPositions(BoundsInt roomBounds)
    {
        validPositions.Clear();
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

        int minDistanceSquared = 15 * 15;

        for (int x = roomBounds.xMin + borderMargin; x <= roomBounds.xMax - borderMargin; x++)
        {
            for (int y = roomBounds.yMin + borderMargin; y <= roomBounds.yMax - borderMargin; y++)
            {
                Vector3Int position = new Vector3Int(x, y, roomBounds.z);

                bool isValidPosition = true;
                for (int dx = -borderMargin; dx <= borderMargin; dx++)
                {
                    for (int dy = -borderMargin; dy <= borderMargin; dy++)
                    {
                        Vector3Int surroundingPosition = new Vector3Int(x + dx, y + dy, roomBounds.z);
                        if (!floorTile.HasTile(surroundingPosition))
                        {
                            isValidPosition = false;
                            break;
                        }
                    }
                    if (!isValidPosition) break;
                }

                bool isFarFromOtherEnemies = true;
                foreach (Vector3Int occupied in occupiedPositions)
                {
                    if ((occupied - position).sqrMagnitude < minDistanceSquared)
                    {
                        isFarFromOtherEnemies = false;
                        break;
                    }
                }

                if (isValidPosition && isFarFromOtherEnemies)
                {
                    validPositions.Add(position);
                    occupiedPositions.Add(position);
                }
            }
        }
    }

    private Vector3Int GetRandomValidPosition(HashSet<Vector3Int> generatedPositions, HashSet<Vector3Int> coinPositions)
    {
        List<Vector3Int> availablePositions = new List<Vector3Int>(validPositions);
        availablePositions.RemoveAll(pos => generatedPositions.Contains(pos) || coinPositions.Contains(pos));

        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("No valid positions available for generating enemies.");
            return Vector3Int.zero;
        }

        return availablePositions[Random.Range(0, availablePositions.Count)];
    }

    private void InstantiateEnemy(GameObject enemyPrefab, Vector3Int position)
    {
        GameObject enemy = Instantiate(enemyPrefab, floorTile.CellToWorld(position) + floorTile.tileAnchor, Quaternion.identity, transform);

        SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
        Vector3 adjustedPosition = floorTile.CellToWorld(position) + floorTile.tileAnchor;

        if (spriteRenderer != null)
        {
            Bounds bounds = spriteRenderer.bounds;
            Vector3 size = bounds.size;
            Vector3 pivot = spriteRenderer.sprite.pivot / spriteRenderer.sprite.pixelsPerUnit;

            adjustedPosition = floorTile.CellToWorld(position) + floorTile.tileAnchor - new Vector3(pivot.x - size.x / 2, pivot.y - size.y / 2, 0);
            enemy.transform.position = adjustedPosition;
        }

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetTarget(enemyTarget);
            Debug.Log($"Enemy spawned at {adjustedPosition} with target {enemyTarget.position}");
        }
    }

    private void ClearEnemies()
    {
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in existingEnemies)
        {
            Destroy(enemy);
        }
    }

   private void Start()
    {
        CoinManager coinManager = FindObjectOfType<CoinManager>();

        if (coinManager != null)
        {
            HashSet<Vector3Int> coinPositions = coinManager.GetCoinPositions();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                BoundsInt roomBounds = floorTile.cellBounds;
                GenerateEnemiesInRoom(roomBounds, coinPositions);
                Debug.Log("YOU ARE RIGHT!");
            }
            else
            {
                Debug.LogError("Player not found in the scene. Enemies cannot be generated.");
            }
        }
        else
        {
            Debug.LogError("CoinManager not found in the scene. Enemies cannot be generated.");
        }
    }
    
}
