using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 patrolCenter;
    private float patrolRadius = 1.0f; // Reduced radius for a smaller patrol area
    private Vector3 targetPosition;
    private float speed = 2.0f;
    public Tilemap floorTilemap; // Reference to the tilemap

    // Method to set the patrol area
    public void SetPatrolArea(Vector3 center, float radius, Tilemap tilemap)
    {
        patrolCenter = center;
        patrolRadius = radius; // Use the provided radius if needed, otherwise default to the smaller radius
        floorTilemap = tilemap; // Assign the tilemap reference
        SetNewTargetPosition();
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        if ((targetPosition - transform.position).sqrMagnitude < 0.1f)
        {
            SetNewTargetPosition();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    private void SetNewTargetPosition()
    {
        Vector3Int randomTilePosition;
        TileBase tile;

        do
        {
            Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
            randomTilePosition = floorTilemap.WorldToCell(patrolCenter + new Vector3(randomDirection.x, randomDirection.y, 0));

            // Ensure that the random tile position is within the bounds of the tilemap
            randomTilePosition.x = Mathf.Clamp(randomTilePosition.x, floorTilemap.cellBounds.xMin + 1, floorTilemap.cellBounds.xMax - 1);
            randomTilePosition.y = Mathf.Clamp(randomTilePosition.y, floorTilemap.cellBounds.yMin + 1, floorTilemap.cellBounds.yMax - 1);

            tile = floorTilemap.GetTile(randomTilePosition);

        } while (tile == null || !IsTileFarFromWalls(randomTilePosition)); // Keep looking for a new target position until a valid walkable tile is found

        targetPosition = floorTilemap.GetCellCenterWorld(randomTilePosition);
    }

    private bool IsTileFarFromWalls(Vector3Int tilePosition)
    {
        // Check all 8 surrounding tiles to ensure they are walkable
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the center tile

                Vector3Int neighborPosition = new Vector3Int(tilePosition.x + x, tilePosition.y + y, tilePosition.z);
                TileBase neighborTile = floorTilemap.GetTile(neighborPosition);

                if (neighborTile == null)
                {
                    return false; // If any neighboring tile is not walkable, the tile is too close to walls
                }
            }
        }

        return true; // All surrounding tiles are walkable
    }
}
