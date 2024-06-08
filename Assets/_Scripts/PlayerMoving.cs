using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMoving : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 3f;
    [SerializeField]
    private Rigidbody2D playerBody;
    [SerializeField]
    private Animator animator;
    private Vector2 movement;
    //generate randomly the player positon on our floorTileMap
    //has relation with the health bar 
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthbar;


    // Reference to the Tilemap containing floor tiles
    public Tilemap floorTilemap;

    // Reference to the Tilemap containing walls
    public Tilemap wallTilemap;

    // Size of the tiles (assuming they are square)
    private float tileSize;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthbar.setMaxHealth(maxHealth);

        BoundsInt bounds = floorTilemap.cellBounds;

        List<Vector3Int> validFloorPositions = new List<Vector3Int>();

        // Iterate through all cells in the bounds
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                // Get current position
                Vector3Int currentPos = new Vector3Int(x, y, 0);

                // Check if current position has a floor tile
                if (floorTilemap.HasTile(currentPos))
                {
                    validFloorPositions.Add(currentPos);
                }
            }
        }
        // Select a random floor position
        Vector3Int randomFloorPosition = validFloorPositions[Random.Range(0, validFloorPositions.Count)];

        // Place the player at the random floor position
        transform.position = floorTilemap.CellToWorld(randomFloorPosition) + new Vector3(0.5f, 0.5f, 0f); // Assuming tiles are centered


        playerBody.position = new Vector2(2, 2);
        tileSize = wallTilemap.cellSize.x; // Assuming tiles are square
    }


    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Set animator parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("playerSpeed", movement.sqrMagnitude);

        //health of the player 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);

        }
     }
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
    }

    private void FixedUpdate()
    {
        // Movement
        // Movement
        Vector2 newPosition = playerBody.position + movement * playerSpeed * Time.fixedDeltaTime;

        // Check if the next position collides with a wall tile
        if (!IsCollidingWithWall(newPosition))
        {
            playerBody.MovePosition(newPosition);
        }
    }

    private bool IsCollidingWithWall(Vector2 position)
    {
        Vector3Int cellPosition = wallTilemap.WorldToCell(position);

        // Check if there's a tile at the given position
        return wallTilemap.HasTile(cellPosition);
    }
}