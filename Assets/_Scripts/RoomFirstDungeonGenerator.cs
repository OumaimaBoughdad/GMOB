using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenrator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField][Range(0, 10)] public int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    private List<BoundsInt> roomOrigins = new List<BoundsInt>();

    private CoinManager coinManager; // Reference to CoinManager
    private EnemyManager enemyManager; // Reference to EnemyManager
    public GameObject firePrefab; // Define firePrefab variable

    // Enemy generation before the game starts :
    private Transform playerTransform;
    [SerializeField] private GameObject enemyPrefab;
    public int maxRegularEnemiesPerRoom = 2;
     public Tilemap floorTilemap;
    public int borderMargin = 1;
    public Transform enemyTarget;

    private List<Vector3Int> validPositions = new List<Vector3Int>();

    private void InstantiateEnemy(GameObject enemyPrefab, Vector3Int position)
    {
        GameObject enemy = Instantiate(enemyPrefab, floorTilemap.CellToWorld(position) + floorTilemap.tileAnchor, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
        Vector3 adjustedPosition = floorTilemap.CellToWorld(position) + floorTilemap.tileAnchor;

        if (spriteRenderer != null)
        {
            Bounds bounds = spriteRenderer.bounds;
            Vector3 size = bounds.size;
            Vector3 pivot = spriteRenderer.sprite.pivot / spriteRenderer.sprite.pixelsPerUnit;

            adjustedPosition = floorTilemap.CellToWorld(position) + floorTilemap.tileAnchor - new Vector3(pivot.x - size.x / 2, pivot.y - size.y / 2, 0);
            enemy.transform.position = adjustedPosition;
        }

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetTarget(enemyTarget);
            Debug.Log($"Enemy spawned at {adjustedPosition} with target {enemyTarget.position}");
        }
    }

    public void GenerateEnemiesInRoom(BoundsInt roomBounds, HashSet<Vector3Int> coinPositions)
    {
        ClearEnemies();// Ensure enemies are cleared before generating new ones
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
                        if (!floorTilemap.HasTile(surroundingPosition))
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

    private void Awake()
    {
        coinManager = FindObjectOfType<CoinManager>(); // Find the CoinManager object in the scene
        enemyManager = FindObjectOfType<EnemyManager>(); // Find the EnemyManager object in the scene
    }

    public void PlayRunProceduralGeneration()
    {
        tileMapVisulizer.Clear();
        RunProceduralGenartion();
    }

    protected override void RunProceduralGenartion()
    {
        
        CreateRooms();
        CoinManager coinManager = FindObjectOfType<CoinManager>();

        if (coinManager != null)
        {
            HashSet<Vector3Int> coinPositions = coinManager.GetCoinPositions();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                BoundsInt roomBounds = floorTilemap.cellBounds;
                GenerateEnemiesInRoom(roomBounds, coinPositions);
                Debug.Log("Enemies generated in the room.");
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
        Debug.Log("i was runned");
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
            minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = randomWalkRooms ? CreateRoomsRandomly(roomsList) : CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        corridors = IncreaseCorridorSize(corridors);
        floor.UnionWith(corridors);
        

        tileMapVisulizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tileMapVisulizer);
        roomOrigins = roomsList; // Store room origins for enemy and coin generation


    }
    private HashSet<Vector2Int> IncreaseCorridorSize(HashSet<Vector2Int> corridors)
    {
        HashSet<Vector2Int> enlargedCorridors = new HashSet<Vector2Int>();
        foreach (var corridorTile in corridors)
        {
            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    enlargedCorridors.Add(corridorTile + new Vector2Int(x, y));
                }
            }
        }
        return enlargedCorridors;
    }

    private void GenerateCoinsInDungeon()
    {
        if (coinManager != null)
        {
            foreach (var room in roomOrigins)
            {
                coinManager.GenerateCoinsInRoom(room); // Generate coins in each room
            }
        }
        else
        {
            Debug.LogWarning("CoinManager not found in the scene.");
        }
    }

    private void GenerateEnemiesInDungeon()
    {
        if (enemyManager != null)
        {
            HashSet<Vector3Int> coinPositions = new HashSet<Vector3Int>();

            // Collect all coin positions
            foreach (var room in roomOrigins)
            {
                for (int x = room.xMin; x < room.xMax; x++)
                {
                    for (int y = room.yMin; y < room.yMax; y++)
                    {
                        Vector3Int position = new Vector3Int(x, y, room.z);
                        if (coinManager.floorTile.HasTile(position))
                        {
                            coinPositions.Add(position);
                        }
                    }
                }
            }

            foreach (var room in roomOrigins)
            {
                enemyManager.GenerateEnemiesInRoom(room, coinPositions); // Generate enemies in each room
            }
        }
        else
        {
            Debug.LogWarning("EnemyManager not found in the scene.");
        }
    }

    private void GenerateFiresInDungeon()
    {
        foreach (var room in roomOrigins)
        {
            PlaceFiresInRoom(room);
        }
    }

    private void PlaceFiresInRoom(BoundsInt roomBounds)
    {
        // Get the corner positions of the room
        List<Vector3Int> cornerPositions = new List<Vector3Int>
        {
            new Vector3Int(roomBounds.min.x, roomBounds.min.y, 0), // Bottom-left corner
            new Vector3Int(roomBounds.min.x, roomBounds.max.y, 0), // Top-left corner
            new Vector3Int(roomBounds.max.x, roomBounds.min.y, 0), // Bottom-right corner
            new Vector3Int(roomBounds.max.x, roomBounds.max.y, 0)  // Top-right corner
        };

        // Place fires at the corner positions
        foreach (var cornerPos in cornerPositions)
        {
            Vector3 worldPosition = floorTilemap.CellToWorld(cornerPos) + floorTilemap.tileAnchor;
            Instantiate(firePrefab, worldPosition, Quaternion.identity, transform);
        }
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset)
                    && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            position.y += (destination.y > position.y) ? 1 : -1;
            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            position.x += (destination.x > position.x) ? 1 : -1;
            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;

        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    floor.Add((Vector2Int)room.min + new Vector2Int(col, row));
                }
            }
        }

        return floor;
    }

    private void ClearEnemies()
    {
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"Clearing {existingEnemies.Length} enemies.");
        foreach (GameObject enemy in existingEnemies)
        {
            Destroy(enemy);
            Debug.Log("One of the enemies cleared.");
        }
    }
}
