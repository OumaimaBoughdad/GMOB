using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossSpawner : MonoBehaviour
{
    public Tilemap floorTilemap; // Reference to the tilemap containing floor tiles

    // Start is called before the first frame update
    void Start()
    {
        SpawnBossAtRandomPosition();
    }

    void SpawnBossAtRandomPosition()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        List<Vector3Int> validFloorPositions = new List<Vector3Int>();

        // Iterate through all cells in the bounds to find floor tiles
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                if (floorTilemap.HasTile(currentPos))
                {
                    validFloorPositions.Add(currentPos);
                }
            }
        }

        // Select a random floor position
        if (validFloorPositions.Count > 0)
        {
            Vector3Int randomFloorPosition = validFloorPositions[Random.Range(0, validFloorPositions.Count)];
            Vector3 spawnPosition = floorTilemap.CellToWorld(randomFloorPosition) + new Vector3(0.5f, 0.5f, 0f); // Adjust for tile center

            // Move the boss to the selected position
            transform.position = spawnPosition;
        }
        else
        {
            Debug.LogWarning("No valid floor positions found for boss spawn.");
        }
    }
}
