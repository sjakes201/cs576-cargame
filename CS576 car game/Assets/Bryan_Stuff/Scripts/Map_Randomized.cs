using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    FourWayStop,
    FourWaySignal,
    ThreeWayStop,
    ThreeWaySignal,
    Curved,
    Straight,
    DeadEnd
}

public class DynamicTileMapGenerator : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject fourWayStopTile;
    public GameObject fourWaySignalTile;
    public GameObject threeWayStopTile;
    public GameObject threeWaySignalTile;
    public GameObject curvedTile;
    public GameObject straightTile;
    public GameObject deadEndTile;

    [Header("Map Settings")]
    public int maxTiles = 100; // Maximum number of tiles to place

    private Dictionary<Vector2Int, Tile> placedTiles = new Dictionary<Vector2Int, Tile>();
    private List<Vector2Int> invalidPositions = new List<Vector2Int>();
    private Queue<(Vector2Int position, Vector2Int direction)> openConnections = new Queue<(Vector2Int, Vector2Int)>();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        placedTiles.Clear();
        invalidPositions.Clear();
        openConnections.Clear();

        // Initialize start tile
        Vector2Int startPosition = Vector2Int.zero;
        Tile startTile = new Tile
        {
            Type = TileType.FourWayStop,
            Connections = GetBaseConnections(TileType.FourWayStop),
            Rotation = 0
        };

        placedTiles[startPosition] = startTile;

        // Instantiate the start tile in the Scene
        InstantiateTile(startTile, startPosition);

        Debug.Log($"Placed start tile at {startPosition}.");

        foreach (Vector2Int connection in startTile.Connections)
        {
            openConnections.Enqueue((startPosition, connection));
        }

        int tilesPlaced = 1;

        // Generate additional tiles
        while (openConnections.Count > 0 && tilesPlaced < maxTiles)
        {
            var (currentPosition, incomingDirection) = openConnections.Dequeue();
            Vector2Int nextPosition = currentPosition + incomingDirection;

            if (placedTiles.ContainsKey(nextPosition))
            {
                Debug.Log($"Skipped {nextPosition}, already occupied.");
                continue;
            }

            Tile newTile = GetValidTile(nextPosition, -incomingDirection);
            if (newTile != null)
            {
                placedTiles[nextPosition] = newTile;
                tilesPlaced++;

                // Instantiate the new tile in the Scene
                InstantiateTile(newTile, nextPosition);

                foreach (Vector2Int connection in newTile.Connections)
                {
                    if (connection != -incomingDirection)
                    {
                        openConnections.Enqueue((nextPosition, connection));
                    }
                }
            }
            else
            {
                invalidPositions.Add(nextPosition);
                Debug.LogWarning($"No valid tile found for position {nextPosition}.");
            }
        }

        Debug.Log($"Map generation complete with {tilesPlaced} tiles.");

        FillGapsWithDeadEnds();
    }

    void InstantiateTile(Tile tile, Vector2Int position)
    {
        GameObject prefab = GetTilePrefab(tile.Type);
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for tile type {tile.Type}.");
            return;
        }

        // Position and rotation
        Vector3 worldPosition = new Vector3(position.x * 50, 0, position.y * 50);
        Quaternion rotation = Quaternion.Euler(0, tile.Rotation * 90, 0);

        // Instantiate prefab
        Instantiate(prefab, worldPosition, rotation);
    }

    GameObject GetTilePrefab(TileType type)
    {
        switch (type)
        {
            case TileType.FourWayStop: return fourWayStopTile;
            case TileType.FourWaySignal: return fourWaySignalTile;
            case TileType.ThreeWayStop: return threeWayStopTile;
            case TileType.ThreeWaySignal: return threeWaySignalTile;
            case TileType.Curved: return curvedTile;
            case TileType.Straight: return straightTile;
            case TileType.DeadEnd: return deadEndTile;
            default:
                Debug.LogWarning($"No prefab found for tile type {type}.");
                return null;
        }
    }

Tile GetValidTile(Vector2Int position, Vector2Int incomingDirection)
{
    var tileTypes = new List<TileType>
    {
        TileType.FourWayStop,
        TileType.FourWaySignal,
        TileType.ThreeWayStop,
        TileType.ThreeWaySignal,
        TileType.Curved,
        TileType.Straight
    };

    tileTypes.Shuffle();

    foreach (var type in tileTypes)
    {
        Vector2Int[] baseConnections = GetBaseConnections(type);
        for (int rotation = 0; rotation < 4; rotation++)
        {
            Vector2Int[] rotatedConnections = RotateConnections(baseConnections, rotation);
            if (Array.Exists(rotatedConnections, conn => conn == incomingDirection) &&
                AreConnectionsValid(position, rotatedConnections))
            {
                Debug.Log($"Valid tile found: {type} at {position} with rotation {rotation * 90}°.");
                return new Tile
                {
                    Type = type,
                    Connections = rotatedConnections,
                    Rotation = rotation
                };
            }
            else
            {
                Debug.Log($"Tile {type} with rotation {rotation * 90}° at {position} failed validation.");
            }
        }
    }

    Debug.LogWarning($"No valid tile found for position {position} with incoming direction {incomingDirection}.");
    return null; // No valid tile found
}


bool AreConnectionsValid(Vector2Int position, Vector2Int[] connections)
{
    foreach (Vector2Int direction in connections)
    {
        Vector2Int neighborPosition = position + direction;

        // Check if the neighbor exists
        if (placedTiles.TryGetValue(neighborPosition, out var neighborTile))
        {
            // Get neighbor connections rotated to match its placement
            Vector2Int[] neighborConnections = RotateConnections(
                GetBaseConnections(neighborTile.Type),
                neighborTile.Rotation
            );

            // Check if the neighbor connects back properly
            if (!Array.Exists(neighborConnections, conn => conn == -direction))
            {
                Debug.LogWarning($"Invalid connection: Tile at {position} connects to {neighborPosition}, but neighbor does not connect back.");
                return false;
            }
        }
    }

    // Additional check: Ensure no neighbor is blocking a required connection
    foreach (Vector2Int neighborOffset in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
    {
        Vector2Int neighborPosition = position + neighborOffset;

        if (placedTiles.TryGetValue(neighborPosition, out var neighborTile))
        {
            Vector2Int[] neighborConnections = RotateConnections(
                GetBaseConnections(neighborTile.Type),
                neighborTile.Rotation
            );

            // If the neighbor has a connection that points toward this tile,
            // ensure this tile also connects back to the neighbor.
            if (Array.Exists(neighborConnections, conn => conn == -neighborOffset) &&
                !Array.Exists(connections, conn => conn == neighborOffset))
            {
                Debug.LogWarning($"Invalid connection: Neighbor at {neighborPosition} expects a connection to {position}, but this tile does not connect back.");
                return false;
            }
        }
    }

    return true;
}


    Vector2Int[] GetBaseConnections(TileType type)
    {
        switch (type)
        {
            case TileType.FourWayStop:
            case TileType.FourWaySignal:
                return new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            case TileType.ThreeWayStop:
            case TileType.ThreeWaySignal:
                return new[] { Vector2Int.down, Vector2Int.left, Vector2Int.up };

            case TileType.Curved:
                return new[] { Vector2Int.down, Vector2Int.right };

            case TileType.Straight:
                return new[] { Vector2Int.up, Vector2Int.down };






        case TileType.DeadEnd:
            return new[] { Vector2Int.down }; // Dead-end connects in one direction also probably source of error due to how connections work, final process should not rely on connections





            default:
                return new Vector2Int[0];
        }
    }

    Vector2Int[] RotateConnections(Vector2Int[] connections, int steps)
    {
        Vector2Int[] rotatedConnections = new Vector2Int[connections.Length];
        for (int i = 0; i < connections.Length; i++)
        {
            Vector2Int direction = connections[i];
            for (int step = 0; step < steps; step++)
            {
                direction = new Vector2Int(direction.y, -direction.x); // Rotate counterclockwise
            }
            rotatedConnections[i] = direction;
        }
        return rotatedConnections;
    }

void FillGapsWithDeadEnds()
{
    List<Vector2Int> directions = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    HashSet<Vector2Int> processedPositions = new HashSet<Vector2Int>();
    Dictionary<Vector2Int, Tile> newDeadEndTiles = new Dictionary<Vector2Int, Tile>(); // Collect new tiles here

    foreach (var kvp in placedTiles)
    {
        Vector2Int tilePosition = kvp.Key;

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPosition = tilePosition + direction;

            // Skip if the position already has a tile or is already processed
            if (placedTiles.ContainsKey(neighborPosition) || processedPositions.Contains(neighborPosition))
            {
                Debug.Log("Skipped");
                continue;
            }
            Debug.Log("Not Skipped");

            // Determine rotation for the dead-end to face the current tile
            int rotation = GetDeadEndRotation(-direction);

            // Create the dead-end tile
            Tile deadEndTile = new Tile
            {
                Type = TileType.DeadEnd,
                Connections = RotateConnections(GetBaseConnections(TileType.DeadEnd), rotation),
                Rotation = rotation
            };

            // Add the tile to the new tiles dictionary
            newDeadEndTiles[neighborPosition] = deadEndTile;

            // Mark this position as processed
            processedPositions.Add(neighborPosition);

            Debug.Log($"Prepared dead-end tile at {neighborPosition} facing {direction}.");
        }
    }

    // Add all new dead-end tiles to the placedTiles dictionary
    foreach (var kvp in newDeadEndTiles)
    {
        placedTiles[kvp.Key] = kvp.Value;
        InstantiateTile(kvp.Value, kvp.Key);
        Debug.Log($"Placed dead-end tile at {kvp.Key}.");
    }

    Debug.Log("All gaps around placed tiles filled with dead-end tiles.");
}


int GetDeadEndRotation(Vector2Int incomingDirection)
{
    if (incomingDirection == Vector2Int.up) return 2; // 180°
    if (incomingDirection == Vector2Int.down) return 0; // Default
    if (incomingDirection == Vector2Int.left) return 1; // 90°
    if (incomingDirection == Vector2Int.right) return 3; // 270°
    return 0;
}


void OnDrawGizmos()
{
    foreach (var kvp in placedTiles)
    {
        Vector2Int gridPosition = kvp.Key;
        Tile tile = kvp.Value;

        Vector3 worldPosition = new Vector3(gridPosition.x * 50, 0, gridPosition.y * 50);

        // Draw connections
        foreach (Vector2Int connection in tile.Connections)
        {
            Vector3 connectionOffset = new Vector3(connection.x, 0, connection.y) * 25;
            Vector3 connectionWorldPosition = worldPosition + connectionOffset;

            Vector2Int neighborPosition = gridPosition + connection;

            if (placedTiles.TryGetValue(neighborPosition, out var neighborTile))
            {
                // Check if the neighbor connects back
                Vector2Int[] neighborConnections = RotateConnections(
                    GetBaseConnections(neighborTile.Type),
                    neighborTile.Rotation
                );

                if (Array.Exists(neighborConnections, conn => conn == -connection))
                {
                    Gizmos.color = Color.green; // Valid connection
                }
                else
                {
                    Gizmos.color = Color.red; // Invalid connection
                }
            }
            else
            {
                Gizmos.color = Color.red; // No neighbor found
            }

            // Draw the connection line
            Gizmos.DrawLine(worldPosition, connectionWorldPosition);
        }

        // Draw the tile's bounding box
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(worldPosition, new Vector3(50, 1, 50));
    }

    // Draw invalid positions
    foreach (var position in invalidPositions)
    {
        Vector3 worldPosition = new Vector3(position.x * 50, 0, position.y * 50);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(worldPosition, new Vector3(50, 1, 50));
    }
}


    public class Tile
    {
        public TileType Type;
        public Vector2Int[] Connections;
        public int Rotation;
    }
}

// Shuffle extension for randomizing tile types
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    
}
