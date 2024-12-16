// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public enum TileType
// {
//     FourWayStop,
//     FourWaySignal,
//     ThreeWayStop,
//     ThreeWaySignal,
//     Curved,
//     Straight
// }

// public class DynamicTileMapGenerator : MonoBehaviour
// {
//     [Header("Tile Prefabs")]
//     public GameObject fourWayStopTile;
//     public GameObject fourWaySignalTile;
//     public GameObject threeWayStopTile;
//     public GameObject threeWaySignalTile;
//     public GameObject curvedTile;
//     public GameObject straightTile;

//     [Header("Map Settings")]
//     public int maxTiles = 100; // Maximum number of tiles to place

//     private Dictionary<Vector2Int, Tile> placedTiles = new Dictionary<Vector2Int, Tile>();
//     private List<Vector2Int> invalidPositions = new List<Vector2Int>();
//     private Queue<(Vector2Int position, Vector2Int direction)> openConnections = new Queue<(Vector2Int, Vector2Int)>();

//     void Start()
//     {
//         GenerateMap();
//     }

//     void GenerateMap()
//     {
//         placedTiles.Clear();
//         invalidPositions.Clear();
//         openConnections.Clear();

//         // Initialize start tile
//         Vector2Int startPosition = Vector2Int.zero;
//         Tile startTile = new Tile
//         {
//             Type = TileType.FourWayStop,
//             Connections = GetBaseConnections(TileType.FourWayStop),
//             Rotation = 0
//         };

//         placedTiles[startPosition] = startTile;

//         // Instantiate the start tile in the Scene
//         InstantiateTile(startTile, startPosition);

//         Debug.Log($"Placed start tile at {startPosition}.");

//         foreach (Vector2Int connection in startTile.Connections)
//         {
//             openConnections.Enqueue((startPosition, connection));
//         }

//         int tilesPlaced = 1;

//         // Generate additional tiles
//         while (openConnections.Count > 0 && tilesPlaced < maxTiles)
//         {
//             var (currentPosition, incomingDirection) = openConnections.Dequeue();
//             Vector2Int nextPosition = currentPosition + incomingDirection;

//             if (placedTiles.ContainsKey(nextPosition))
//             {
//                 Debug.Log($"Skipped {nextPosition}, already occupied.");
//                 continue;
//             }

//             Tile newTile = GetValidTile(nextPosition, -incomingDirection);
//             if (newTile != null)
//             {
//                 placedTiles[nextPosition] = newTile;
//                 tilesPlaced++;

//                 // Instantiate the new tile in the Scene
//                 InstantiateTile(newTile, nextPosition);

//                 foreach (Vector2Int connection in newTile.Connections)
//                 {
//                     if (connection != -incomingDirection)
//                     {
//                         openConnections.Enqueue((nextPosition, connection));
//                     }
//                 }
//             }
//             else
//             {
//                 invalidPositions.Add(nextPosition);
//                 Debug.LogWarning($"No valid tile found for position {nextPosition}.");
//             }
//         }

//         Debug.Log($"Map generation complete with {tilesPlaced} tiles.");
//     }

//     void InstantiateTile(Tile tile, Vector2Int position)
//     {
//         GameObject prefab = GetTilePrefab(tile.Type);
//         if (prefab == null)
//         {
//             Debug.LogWarning($"No prefab found for tile type {tile.Type}.");
//             return;
//         }

//         // Position and rotation
//         Vector3 worldPosition = new Vector3(position.x * 50, 0, position.y * 50);
//         Quaternion rotation = Quaternion.Euler(0, tile.Rotation * 90, 0);

//         // Instantiate prefab
//         Instantiate(prefab, worldPosition, rotation);
//     }

//     GameObject GetTilePrefab(TileType type)
//     {
//         switch (type)
//         {
//             case TileType.FourWayStop: return fourWayStopTile;
//             case TileType.FourWaySignal: return fourWaySignalTile;
//             case TileType.ThreeWayStop: return threeWayStopTile;
//             case TileType.ThreeWaySignal: return threeWaySignalTile;
//             case TileType.Curved: return curvedTile;
//             case TileType.Straight: return straightTile;
//             default:
//                 Debug.LogWarning($"No prefab found for tile type {type}.");
//                 return null;
//         }
//     }

//     Tile GetValidTile(Vector2Int position, Vector2Int incomingDirection)
//     {
//         var tileTypes = new List<TileType>
//         {
//             TileType.FourWayStop,
//             TileType.FourWaySignal,
//             TileType.ThreeWayStop,
//             TileType.ThreeWaySignal,
//             TileType.Curved,
//             TileType.Straight
//         };

//         // Shuffle the tile types for randomness
//         tileTypes.Shuffle();

//         foreach (var type in tileTypes)
//         {
//             Vector2Int[] baseConnections = GetBaseConnections(type);
//             for (int rotation = 0; rotation < 4; rotation++)
//             {
//                 Vector2Int[] rotatedConnections = RotateConnections(baseConnections, rotation);
//                 if (Array.Exists(rotatedConnections, conn => conn == incomingDirection) &&
//                     AreConnectionsValid(position, rotatedConnections))
//                 {
//                     Debug.Log($"Found valid tile: {type} at {position} with rotation {rotation * 90}°.");
//                     return new Tile
//                     {
//                         Type = type,
//                         Connections = rotatedConnections,
//                         Rotation = rotation
//                     };
//                 }
//             }
//         }

//         Debug.LogWarning($"No valid tile found for position {position} with incoming direction {incomingDirection}.");
//         return null; // No valid tile found
//     }

//     bool AreConnectionsValid(Vector2Int position, Vector2Int[] connections)
//     {
//         foreach (Vector2Int direction in connections)
//         {
//             Vector2Int neighborPosition = position + direction;

//             if (placedTiles.TryGetValue(neighborPosition, out var neighborTile))
//             {
//                 if (!Array.Exists(neighborTile.Connections, conn => conn == -direction))
//                 {
//                     Debug.LogWarning($"Invalid connection at {position}: Neighbor at {neighborPosition} does not connect back.");
//                     return false; // Neighbor does not connect back
//                 }
//             }
//         }
//         return true;
//     }

//     Vector2Int[] GetBaseConnections(TileType type)
//     {
//         switch (type)
//         {
//             case TileType.FourWayStop:
//             case TileType.FourWaySignal:
//                 return new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

//             case TileType.ThreeWayStop:
//             case TileType.ThreeWaySignal:
//                 return new[] { Vector2Int.down, Vector2Int.left, Vector2Int.up };

//             case TileType.Curved:
//                 return new[] { Vector2Int.down, Vector2Int.right };

//             case TileType.Straight:
//                 return new[] { Vector2Int.up, Vector2Int.down };

//             default:
//                 return new Vector2Int[0];
//         }
//     }

//     Vector2Int[] RotateConnections(Vector2Int[] connections, int steps)
//     {
//         Vector2Int[] rotatedConnections = new Vector2Int[connections.Length];
//         for (int i = 0; i < connections.Length; i++)
//         {
//             Vector2Int direction = connections[i];
//             for (int step = 0; step < steps; step++)
//             {
//                 direction = new Vector2Int(direction.y, -direction.x); // Rotate counterclockwise
//             }
//             rotatedConnections[i] = direction;
//         }
//         return rotatedConnections;
//     }

// void OnDrawGizmos()
// {
//     foreach (var kvp in placedTiles)
//     {
//         Vector2Int gridPosition = kvp.Key;
//         Tile tile = kvp.Value;

//         Vector3 worldPosition = new Vector3(gridPosition.x * 50, 0, gridPosition.y * 50);

//         // Draw connections
//         foreach (Vector2Int connection in tile.Connections)
//         {
//             Vector3 connectionOffset = new Vector3(connection.x, 0, connection.y) * 25;
//             Vector3 connectionWorldPosition = worldPosition + connectionOffset;

//             Vector2Int neighborPosition = gridPosition + connection;

//             if (placedTiles.TryGetValue(neighborPosition, out var neighborTile))
//             {
//                 // Check if the neighbor connects back
//                 Vector2Int[] neighborConnections = RotateConnections(
//                     GetBaseConnections(neighborTile.Type),
//                     neighborTile.Rotation
//                 );

//                 if (Array.Exists(neighborConnections, conn => conn == -connection))
//                 {
//                     Gizmos.color = Color.green; // Valid connection
//                 }
//                 else
//                 {
//                     Gizmos.color = Color.red; // Invalid connection
//                 }
//             }
//             else
//             {
//                 Gizmos.color = Color.red; // No neighbor found
//             }

//             // Draw the connection line
//             Gizmos.DrawLine(worldPosition, connectionWorldPosition);
//         }

//         // Draw the tile's bounding box
//         Gizmos.color = Color.blue;
//         Gizmos.DrawWireCube(worldPosition, new Vector3(50, 1, 50));
//     }

//     // Draw invalid positions
//     foreach (var position in invalidPositions)
//     {
//         Vector3 worldPosition = new Vector3(position.x * 50, 0, position.y * 50);
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireCube(worldPosition, new Vector3(50, 1, 50));
//     }
// }


//     public class Tile
//     {
//         public TileType Type;
//         public Vector2Int[] Connections;
//         public int Rotation;
//     }
// }

// // Shuffle extension for randomizing tile types
// public static class ListExtensions
// {
//     private static System.Random rng = new System.Random();

//     public static void Shuffle<T>(this IList<T> list)
//     {
//         int n = list.Count;
//         while (n > 1)
//         {
//             n--;
//             int k = rng.Next(n + 1);
//             T value = list[k];
//             list[k] = list[n];
//             list[n] = value;
//         }
//     }


    
// }
