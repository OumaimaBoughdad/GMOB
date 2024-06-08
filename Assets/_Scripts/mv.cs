using UnityEngine;

public class mv : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
    public Vector2 direction = Vector2.right; // Initial direction of movement
    private Bounds tilemapBounds;

    public GameObject tilemapObject; // Assign your tilemap GameObject in the Inspector

    void Start()
    {
        if (tilemapObject != null)
        {
            tilemapBounds = CalculateTilemapBounds(tilemapObject);
        }
        else
        {
            Debug.LogError("Tilemap GameObject not assigned.");
        }
    }

    void Update()
    {
        if (tilemapBounds.size != Vector3.zero)
        {
            Vector3 newPosition = transform.position + (Vector3)direction * moveSpeed * Time.deltaTime;

            if (IsWithinBounds(newPosition))
            {
                transform.position = newPosition;
            }
            else
            {
                // Reverse direction upon hitting the bounds
                direction = -direction;
            }
        }
    }

    private bool IsWithinBounds(Vector3 position)
    {
        return tilemapBounds.Contains(position);
    }

    private Bounds CalculateTilemapBounds(GameObject tilemap)
    {
        Bounds bounds = new Bounds(tilemap.transform.position, Vector3.zero);

        foreach (Transform child in tilemap.transform)
        {
            bounds.Encapsulate(child.position);
        }

        return bounds;
    }
}

