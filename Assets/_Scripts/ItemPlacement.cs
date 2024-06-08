using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPlacement
{
    private Dictionary<PlacementType, HashSet<Vector2Int>> tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();
    private HashSet<Vector2Int> roomFloorCorridor;

    public ItemPlacement(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorCorridor)
    {
        Graph graph = new Graph(roomFloor);
        this.roomFloorCorridor = roomFloorCorridor;
        foreach (var position in roomFloorCorridor)
        {
            int neighboursCount8dir = graph.GetNeighbours8Directions(position).Count;
            PlacementType type = neighboursCount8dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace;

            if (!tileByType.ContainsKey(type))
                tileByType[type] = new HashSet<Vector2Int>();

            if (type == PlacementType.NearWall && graph.GetNeighbours4Directions(position).Count > 0)
                continue;

            tileByType[type].Add(position);
        }
    }

    public Vector2? GetItemPlacementPosition(PlacementType placementType, int iterationMax, Vector2Int size)
    {
        int itemArea = size.x * size.y;
        if (tileByType[placementType].Count < itemArea)
            return null;

        int iteration = 0;
        while (iteration < iterationMax)
        {
            iteration++;
            int index = UnityEngine.Random.Range(0, tileByType[placementType].Count);
            Vector2Int position = tileByType[placementType].ElementAt(index);

            if (itemArea > 1)
            {
                var (result, placementPositions) = PlaceBigItem(position, size, false);
                if (!result)
                    continue;

                tileByType[placementType].ExceptWith(placementPositions);
                if (tileByType.ContainsKey(PlacementType.NearWall))
                    tileByType[PlacementType.NearWall].ExceptWith(placementPositions);
            }
            else
            {
                tileByType[placementType].Remove(position);
            }
            return position;
        }
        return null;
    }

    private (bool result, List<Vector2Int> positions) PlaceBigItem(Vector2Int originPosition, Vector2Int size, bool addOffset)
    {
        List<Vector2Int> positions = new List<Vector2Int> { originPosition };
        int maxX = addOffset ? size.x + 1 : size.x;
        int maxY = addOffset ? size.y + 1 : size.y;
        int minX = addOffset ? -1 : 0;
        int minY = addOffset ? -1 : 0;

        for (int row = minX; row < maxX; row++)
        {
            for (int col = minY; col < maxY; col++)
            {
                if (col == 0 && row == 0)
                    continue;

                Vector2Int newPosToCheck = new Vector2Int(originPosition.x + row, originPosition.y + col);
                if (!roomFloorCorridor.Contains(newPosToCheck))
                    return (false, positions);

                positions.Add(newPosToCheck);
            }
        }
        return (true, positions);
    }

    public enum PlacementType
    {
        OpenSpace,
        NearWall
    }
}
