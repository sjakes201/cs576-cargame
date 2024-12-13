// using UnityEngine;

// public class TileBuilder : MonoBehaviour
// {
//     [Header("Materials")]
//     [SerializeField] private Material roadMaterial;        // Assign in the Inspector
//     [SerializeField] private Material sidewalkMaterial;    // Assign in the Inspector
//     [SerializeField] private Material laneMarkingMaterial; // Material for lane markings
//     [SerializeField] private Material midLaneMarkingMaterial; // Material for the middle yellow lane marking
//     [SerializeField] private Material curbMaterial; // Material to distinguish curbs from rest

//     [Header("Tile Dimensions")]
//     [SerializeField] private float tileWidth = 50f;        // Total width of the tile
//     [SerializeField] private float tileLength = 50f;       // Total length of the tile
//     [SerializeField] private float laneWidth = 10f;        // Width of each lane
//     [SerializeField] private float curbHeight = 0.2f;      // Height of the curb
//     [SerializeField] private float curbWidth = 0.5f;       // Width of the curb

//     [Header("Lane and Sidewalk Settings")]
//     [SerializeField] private int numberOfLanes = 4;        // Number of lanes (hardcoded to 4 for now)
//     [SerializeField] private float sidewalkWidthFactor = 0.5f; // Sidewalk width as a factor of total road width

//     [Header("Mid Lane Marking Dimensions")]
//     [SerializeField] private float midMarkingWidth = 0.3f;   // Width of the middle yellow marking
//     [SerializeField] private float midMarkingHeight = 0.05f; // Height of the middle yellow marking
//     [SerializeField] private float midMarkingSpacing = 0.5f; // Spacing between the two middle yellow lines

//     [Header("Dotted Lane Marking Dimensions")]
//     [SerializeField] private float markingWidth = 0.25f;    // Width of the lane marking
//     [SerializeField] private float markingLength = 3.0f;    // Length of individual markings
//     [SerializeField] private float markingSpacing = 1.5f;   // Spacing between markings

//     [Header("Grid Settings")]
//     [SerializeField] private int gridWidth = 1;            // Number of tiles horizontally
//     [SerializeField] private int gridHeight = 1;           // Number of tiles vertically
//     [SerializeField] private float tileSpacing = 0f;       // Gap between tiles

//     [SerializeField] private string prefabSavePath = "Assets/Prefabs/GeneratedTile.prefab"; // Path to save the prefab


//     [Header("Curve Settings")]
//     [SerializeField] private int segmentsPerCurve = 10;    // Number of segments to divide the curve for smoothing
//     [SerializeField] private bool curveToLeft = true; // Determine the direction of the curve




//     [Header("Tile Generation Type")]
//     [SerializeField] private bool generateStraightTile = true; // Toggle for generating a straight road tile
//     [SerializeField] private bool generateCurveTile = false; // Toggle for generating a curved tile


//     public bool GenerateStraightTile
//     {
//         get => generateStraightTile;
//         set => generateStraightTile = value;
//     }

//     public bool GenerateCurveTile
//     {
//         get => generateCurveTile;
//         set => generateCurveTile = value;
//     }

//     public bool CurveToLeft
//     {
//         get => curveToLeft;
//         set => curveToLeft = value;
//     }

//     public string GetPrefabSavePath()
//     {
//         return prefabSavePath;
//     }


//     private void Start()
//     {
//         GenerateTiles(); // Automatically generate tiles on start
//     }

//     private void GenerateTiles()
//     {
//         for (int x = 0; x < gridWidth; x++)
//         {
//             for (int z = 0; z < gridHeight; z++)
//             {
//                 Vector3 position = Vector3.zero;
//                 GameObject tile;

//                 if (generateStraightTile)
//                 {
//                     tile = BuildStraightRoadTile();
//                 }
//                 else if (generateCurveTile)
//                 {
//                     tile = BuildCurveTile(curveToLeft);
//                 }
//                 else
//                 {
//                     Debug.LogWarning("No tile type selected. Defaulting to straight road tile.");
//                     tile = BuildStraightRoadTile();
//                 }

//                 tile.transform.position = position;
//                 tile.transform.parent = this.transform;

//             }
//         }
//     }


//     public GameObject BuildStraightRoadTile()
//     {
//         // Calculate sidewalk width based on the number of lanes and road width
//         float roadWidth = numberOfLanes * laneWidth;
//         float sidewalkWidth = (tileWidth - roadWidth) * sidewalkWidthFactor;

//         // Create the parent object
//         GameObject tile = new GameObject("StraightRoadTile");

//         // Create the road
//         GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         road.name = "RoadSurface"; // Name it for debugging
//         road.transform.parent = tile.transform;
//         road.transform.localScale = new Vector3(roadWidth, 0.1f, tileLength); // Flat road surface
//         road.transform.localPosition = Vector3.zero;
//         road.GetComponent<MeshRenderer>().material = roadMaterial;
//         road.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(roadWidth / 10f, tileLength / 10f);

//         // Create lanes with colliders and tags
//         for (int i = 0; i < numberOfLanes; i++)
//         {
//             float lanePositionX = -roadWidth / 2 + laneWidth / 2 + i * laneWidth;
//             string laneName = (i < numberOfLanes / 2) ? "NorthboundLane" + i : "SouthboundLane" + (i - numberOfLanes / 2);
//             CreateLaneCollider(tile, laneName, new Vector3(lanePositionX, 0.1f, 0));
//         }

//         // Add lane markings
//         CreateMidLaneMarking(tile, new Vector3(0, 0.11f, 0), tileLength);
//         CreateLaneMarkingsBetweenLanes(tile, roadWidth, tileLength);

//         // Add sidewalks
//         Vector3 leftSidewalkPosition = new Vector3(-tileWidth / 2 + sidewalkWidth / 2, 0.05f, 0);
//         Vector3 rightSidewalkPosition = new Vector3(tileWidth / 2 - sidewalkWidth / 2, 0.05f, 0);
//         CreateSidewalk(tile, "LeftSidewalk", leftSidewalkPosition, sidewalkWidth);
//         CreateSidewalk(tile, "RightSidewalk", rightSidewalkPosition, sidewalkWidth);

//         // Add curbs immediately after sidewalks
//         Vector3 leftCurbPosition = new Vector3(leftSidewalkPosition.x + sidewalkWidth / 2 + curbWidth / 2, curbHeight / 2, 0);
//         Vector3 rightCurbPosition = new Vector3(rightSidewalkPosition.x - sidewalkWidth / 2 - curbWidth / 2, curbHeight / 2, 0);
//         CreateCurb(tile, "LeftCurb", leftCurbPosition);
//         CreateCurb(tile, "RightCurb", rightCurbPosition);


//         return tile;
//     }

    
// public GameObject BuildCurveTile(bool curveToLeft)
// {
//     float roadRadius = tileLength / 2;
//     float sidewalkWidth = (tileWidth - (numberOfLanes * laneWidth)) / 2;

//     // Calculate radii for sidewalks and curbs
//     float sidewalkRadiusOuter = (sidewalkWidth / 2 + sidewalkWidth) + (numberOfLanes * laneWidth);
//     float sidewalkRadiusInner = sidewalkWidth / 2;

//     float curbRadiusOuter = sidewalkWidth + (numberOfLanes * laneWidth) - curbWidth / 2;
//     float curbRadiusInner = sidewalkWidth + curbWidth / 2;

//     // Create the parent object for the curved tile
//     GameObject curveTile = new GameObject("CurveTile");

//     // Curve direction multiplier (-1 for left curve, 1 for right curve)
//     float directionMultiplier = curveToLeft ? -1 : 1;

// // Create curved road with consistent height
//     CreateCurvedMesh(curveTile, roadRadius, laneWidth * numberOfLanes, roadMaterial, "CurvedRoad", segmentsPerCurve, directionMultiplier, 0.1f, true);

//     // Create curved sidewalks (outer and inner) with consistent height and add colliders
//     CreateCurvedMesh(curveTile, sidewalkRadiusOuter, sidewalkWidth, sidewalkMaterial, "CurvedSidewalkOuter", segmentsPerCurve, directionMultiplier, 0.1f, true);
//     CreateCurvedMesh(curveTile, sidewalkRadiusInner, sidewalkWidth, sidewalkMaterial, "CurvedSidewalkInner", segmentsPerCurve, directionMultiplier, 0.1f, true);

//     // Create curbs along the curved tile (outer and inner) directly on the edge of the road, raised above it
//     CreateCurvedCurb(curveTile, curbRadiusOuter, curbWidth, curbHeight, curbMaterial, "CurvedCurbOuter", segmentsPerCurve, directionMultiplier);
//     CreateCurvedCurb(curveTile, curbRadiusInner, curbWidth, curbHeight, curbMaterial, "CurvedCurbInner", segmentsPerCurve, directionMultiplier);

//     // Create middle lane markings (two parallel continuous yellow lines)
//     float midMarkingRadiusLeft = roadRadius - midMarkingSpacing / 2; // Left yellow line
//     float midMarkingRadiusRight = roadRadius + midMarkingSpacing / 2; // Right yellow line
//     CreateCurvedContinuousMarkings(curveTile, midMarkingRadiusLeft, midMarkingWidth, midLaneMarkingMaterial, "CurvedMidLaneMarkingLeft", segmentsPerCurve, directionMultiplier);
//     CreateCurvedContinuousMarkings(curveTile, midMarkingRadiusRight, midMarkingWidth, midLaneMarkingMaterial, "CurvedMidLaneMarkingRight", segmentsPerCurve, directionMultiplier);

//     // Create lane markings between the lanes except for the center
//     for (int i = 1; i < numberOfLanes; i++)
//     {
//         if (i == numberOfLanes / 2) continue; // Skip the middle section (already covered by solid lines)

//         float laneMarkingRadius = roadRadius - (numberOfLanes * laneWidth) / 2 + i * laneWidth;
//         CreateCurvedLaneMarkings(curveTile, laneMarkingRadius, markingWidth, laneMarkingMaterial, "CurvedLaneMarkingBetweenLanes" + i, segmentsPerCurve, directionMultiplier);
//     }

//     return curveTile;
// }


// private void CreateCurvedCurb(GameObject parent, float radius, float width, float height, Material material, string name, int segments, float directionMultiplier)
// {
//     float segmentAngle = 90f / segments;

//     for (int i = 0; i < segments; i++)
//     {
//         float currentAngle = i * segmentAngle * directionMultiplier;
//         float nextAngle = (i + 1) * segmentAngle * directionMultiplier;
//         float angleRad = currentAngle * Mathf.Deg2Rad;

//         // Calculate the current and next positions
//         float x = Mathf.Sin(angleRad) * radius;
//         float z = Mathf.Cos(angleRad) * radius;

//         float nextX = Mathf.Sin(nextAngle * Mathf.Deg2Rad) * radius;
//         float nextZ = Mathf.Cos(nextAngle * Mathf.Deg2Rad) * radius;

//         // Calculate midpoint for proper alignment
//         float midX = (x + nextX) / 2;
//         float midZ = (z + nextZ) / 2;

//         // Create curb segment
//         GameObject curbSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         curbSegment.name = name + "Segment" + i;
//         curbSegment.transform.parent = parent.transform;
//         curbSegment.transform.localPosition = new Vector3(midX, curbHeight / 2, midZ); // Place curb at the correct height above the road

//         // Set the rotation so that the curb segment is tangent to the curve
//         float tangentAngle = currentAngle + segmentAngle / 2 * directionMultiplier;
//         curbSegment.transform.localRotation = Quaternion.Euler(0, tangentAngle + 90f * directionMultiplier, 0);

//         // Set segment size to match the distance between current and next positions
//         float segmentLength = Vector3.Distance(new Vector3(x, 0, z), new Vector3(nextX, 0, nextZ));
//         curbSegment.transform.localScale = new Vector3(width, height, segmentLength);
//         curbSegment.GetComponent<MeshRenderer>().material = material;
//     }
// }


// private void CreateCurvedContinuousMarkings(GameObject parent, float radius, float width, Material material, string name, int segments, float directionMultiplier)
// {
//     float segmentAngle = 90f / segments;
//     float markingHeight = 0.05f; // Set marking height to match road height

//     for (int i = 0; i < segments; i++)
//     {
//         float currentAngle = i * segmentAngle * directionMultiplier;
//         float nextAngle = (i + 1) * segmentAngle * directionMultiplier;
//         float angleRad = currentAngle * Mathf.Deg2Rad;

//         // Calculate the current and next positions
//         float x = Mathf.Sin(angleRad) * radius;
//         float z = Mathf.Cos(angleRad) * radius;

//         float nextX = Mathf.Sin(nextAngle * Mathf.Deg2Rad) * radius;
//         float nextZ = Mathf.Cos(nextAngle * Mathf.Deg2Rad) * radius;

//         // Calculate midpoint for proper alignment
//         float midX = (x + nextX) / 2;
//         float midZ = (z + nextZ) / 2;

//         // Create marking segment
//         GameObject markingSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         markingSegment.name = name + "Segment" + i;
//         markingSegment.transform.parent = parent.transform;
//         markingSegment.transform.localPosition = new Vector3(midX, markingHeight, midZ);

//         // Set the rotation so that the marking segment is tangent to the curve
//         float tangentAngle = currentAngle + segmentAngle / 2 * directionMultiplier;
//         markingSegment.transform.localRotation = Quaternion.Euler(0, tangentAngle + 90f * directionMultiplier, 0);

//         // Set segment size to match the distance between current and next positions
//         float segmentLength = Vector3.Distance(new Vector3(x, 0, z), new Vector3(nextX, 0, nextZ));
//         markingSegment.transform.localScale = new Vector3(width, 0.05f, segmentLength);
//         markingSegment.GetComponent<MeshRenderer>().material = material;

//         // Make the collider a trigger
//         BoxCollider markingCollider = markingSegment.GetComponent<BoxCollider>();
//         markingCollider.isTrigger = true;
//     }
// }

// private void CreateCurvedLaneMarkings(GameObject parent, float radius, float width, Material material, string name, int segments, float directionMultiplier)
// {
//     float segmentAngle = 90f / segments;
//     float markingHeight = 0.05f; // Set marking height to match road height
//     float currentPosition = 0;   // Track position for dashed lane markings
//     bool drawMarking = true;     // Toggle for marking segments to create dashes

//     for (int i = 0; i < segments; i++)
//     {
//         float currentAngle = i * segmentAngle * directionMultiplier;
//         float nextAngle = (i + 1) * segmentAngle * directionMultiplier;
//         float angleRad = currentAngle * Mathf.Deg2Rad;

//         // Calculate the current and next positions
//         float x = Mathf.Sin(angleRad) * radius;
//         float z = Mathf.Cos(angleRad) * radius;

//         float nextX = Mathf.Sin(nextAngle * Mathf.Deg2Rad) * radius;
//         float nextZ = Mathf.Cos(nextAngle * Mathf.Deg2Rad) * radius;

//         if (drawMarking)
//         {
//             // Calculate midpoint for proper alignment
//             float midX = (x + nextX) / 2;
//             float midZ = (z + nextZ) / 2;

//             // Create marking segment
//             GameObject markingSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
//             markingSegment.name = name + "Segment" + i;
//             markingSegment.transform.parent = parent.transform;
//             markingSegment.transform.localPosition = new Vector3(midX, markingHeight, midZ);

//             // Set the rotation so that the marking segment is tangent to the curve
//             float tangentAngle = currentAngle + segmentAngle / 2 * directionMultiplier;
//             markingSegment.transform.localRotation = Quaternion.Euler(0, tangentAngle + 90f * directionMultiplier, 0);

//             // Set segment size to match the distance between current and next positions
//             float segmentLength = Vector3.Distance(new Vector3(x, 0, z), new Vector3(nextX, 0, nextZ));
//             markingSegment.transform.localScale = new Vector3(width, 0.05f, segmentLength);
//             markingSegment.GetComponent<MeshRenderer>().material = material;

//             // Make the collider a trigger
//             BoxCollider markingCollider = markingSegment.GetComponent<BoxCollider>();
//             markingCollider.isTrigger = true;
//         }

//         // Update marking position and toggle drawing state
//         currentPosition += radius * Mathf.PI * segmentAngle / 180;

//         // Toggle between drawing and not drawing if we've reached a full marking length or spacing
//         if (currentPosition >= markingLength)
//         {
//             drawMarking = !drawMarking;
//             currentPosition = 0; // Reset position tracker after switching
//         }
//     }
// }


// private void CreateCurvedMesh(GameObject parent, float radius, float width, Material material, string name, int segments, float directionMultiplier, float height, bool addCollider)
// {
//     Mesh mesh = new Mesh();
//     Vector3[] vertices = new Vector3[(segments + 1) * 4]; // Now we have twice as many vertices for top and bottom
//     int[] triangles = new int[segments * 12]; // More triangles to create both top and bottom surfaces and the sides
//     Vector2[] uvs = new Vector2[(segments + 1) * 4]; // UVs for texture mapping

//     float segmentAngle = 90f / segments;

//     for (int i = 0; i <= segments; i++)
//     {
//         // Adjusted angle calculation to incorporate curve direction (left/right)
//         float angleRad = i * segmentAngle * Mathf.Deg2Rad * directionMultiplier;

//         // Calculating the position of the outer and inner points of the curved segment for top face
//         float outerX = Mathf.Sin(angleRad) * (radius + width / 2);
//         float outerZ = Mathf.Cos(angleRad) * (radius + width / 2);
//         float innerX = Mathf.Sin(angleRad) * (radius - width / 2);
//         float innerZ = Mathf.Cos(angleRad) * (radius - width / 2);

//         // Setting the vertex positions for the top face
//         vertices[i * 4] = new Vector3(outerX, height / 2, outerZ); // Top outer
//         vertices[i * 4 + 1] = new Vector3(innerX, height / 2, innerZ); // Top inner

//         // Setting the vertex positions for the bottom face
//         vertices[i * 4 + 2] = new Vector3(outerX, -height / 2, outerZ); // Bottom outer
//         vertices[i * 4 + 3] = new Vector3(innerX, -height / 2, innerZ); // Bottom inner

//         // Set UV coordinates for texture mapping
//         float uvX = (float)i / segments;
//         uvs[i * 4] = new Vector2(uvX, 1); // Top outer
//         uvs[i * 4 + 1] = new Vector2(uvX, 0); // Top inner
//         uvs[i * 4 + 2] = new Vector2(uvX, 1); // Bottom outer
//         uvs[i * 4 + 3] = new Vector2(uvX, 0); // Bottom inner

//         // Setting up the triangle indices for the mesh
//         if (i < segments)
//         {
//             int baseIndex = i * 12;

//             // Top face triangles
//             triangles[baseIndex] = i * 4;
//             triangles[baseIndex + 1] = i * 4 + 1;
//             triangles[baseIndex + 2] = (i + 1) * 4;

//             triangles[baseIndex + 3] = i * 4 + 1;
//             triangles[baseIndex + 4] = (i + 1) * 4 + 1;
//             triangles[baseIndex + 5] = (i + 1) * 4;

//             // Bottom face triangles (flipped winding order)
//             triangles[baseIndex + 6] = i * 4 + 2;
//             triangles[baseIndex + 7] = (i + 1) * 4 + 2;
//             triangles[baseIndex + 8] = i * 4 + 3;

//             triangles[baseIndex + 9] = i * 4 + 3;
//             triangles[baseIndex + 10] = (i + 1) * 4 + 2;
//             triangles[baseIndex + 11] = (i + 1) * 4 + 3;
//         }
//     }

//     // Assigning the calculated vertices and triangles to the mesh
//     mesh.vertices = vertices;
//     mesh.triangles = triangles;
//     mesh.uv = uvs; // Assign the UVs to the mesh
//     mesh.RecalculateNormals();

//     // Creating the GameObject for the curved mesh segment
//     GameObject curvedSegment = new GameObject(name);
//     curvedSegment.transform.parent = parent.transform;

//     // Assigning MeshFilter and MeshRenderer components
//     MeshFilter mf = curvedSegment.AddComponent<MeshFilter>();
//     mf.mesh = mesh;

//     MeshRenderer mr = curvedSegment.AddComponent<MeshRenderer>();
//     mr.material = material;
//     mr.material.mainTextureScale = new Vector2(3, 1);



//     // Add a MeshCollider for physics interactions if required
//     if (addCollider)
//     {
//         MeshCollider collider = curvedSegment.AddComponent<MeshCollider>();
//         collider.sharedMesh = mesh; // Use the generated mesh as the collider
//     }
// }

    
//     private void CreateLaneCollider(GameObject parent, string name, Vector3 position)
//     {

//         // In case lane detection type stuff is needed for the straight path
//         GameObject lane = new GameObject(name);
//         lane.transform.parent = parent.transform;
//         lane.transform.localPosition = position;

//         BoxCollider collider = lane.AddComponent<BoxCollider>();
//         collider.isTrigger = true;  // Set collider as a trigger
//         collider.size = new Vector3(laneWidth, 0.1f, tileLength);
//         lane.tag = name; // Assign the tag
//     }


// private void CreateSidewalk(GameObject parent, string name, Vector3 position, float width)
// {
//     GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
//     sidewalk.name = name; // Name it for debugging
//     sidewalk.transform.parent = parent.transform;
//     sidewalk.transform.localPosition = position;
//     sidewalk.transform.localScale = new Vector3(width, 0.1f, tileLength); // Sidewalk width and height
//     sidewalk.GetComponent<MeshRenderer>().material = sidewalkMaterial;
//     sidewalk.AddComponent<BoxCollider>(); // Add collider to sidewalk
// }


//     private void CreateCurb(GameObject parent, string name, Vector3 position)
//     {
//         GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         curb.name = name; // Name it for debugging
//         curb.transform.parent = parent.transform;
//         curb.transform.localPosition = position;
//         curb.transform.localScale = new Vector3(curbWidth, curbHeight, tileLength); // Curb dimensions
//         curb.GetComponent<MeshRenderer>().material = curbMaterial; // Same material as sidewalk
//     }
    
    
//     private void CreateMidLaneMarking(GameObject parent, Vector3 startPosition, float tileLength)
//     {
//         // Set the marking position to be slightly above or at the road's surface
//         float markingHeight = 0.05f;

//         // Create the first middle marking line
//         GameObject leftMarking = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         leftMarking.name = "MidLaneMarkingLeft"; // Name for debugging
//         leftMarking.transform.parent = parent.transform;
//         leftMarking.transform.localPosition = new Vector3(-midMarkingSpacing / 2, markingHeight, 0);
//         leftMarking.transform.localScale = new Vector3(midMarkingWidth, midMarkingHeight, tileLength);
//         leftMarking.GetComponent<MeshRenderer>().material = midLaneMarkingMaterial;

//         BoxCollider leftCollider = leftMarking.GetComponent<BoxCollider>();
//         leftCollider.isTrigger = true; // Set the marking collider as a trigger

//         // Create the second middle marking line
//         GameObject rightMarking = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         rightMarking.name = "MidLaneMarkingRight"; // Name for debugging
//         rightMarking.transform.parent = parent.transform;
//         rightMarking.transform.localPosition = new Vector3(midMarkingSpacing / 2, markingHeight, 0);
//         rightMarking.transform.localScale = new Vector3(midMarkingWidth, midMarkingHeight, tileLength);
//         rightMarking.GetComponent<MeshRenderer>().material = midLaneMarkingMaterial;

//         BoxCollider rightCollider = rightMarking.GetComponent<BoxCollider>();
//         rightCollider.isTrigger = true; // Set the marking collider as a trigger
//     }



// private void CreateLaneMarkingsBetweenLanes(GameObject parent, float roadWidth, float tileLength)
// {
//     float markingHeight = 0.05f;

//     for (int i = 1; i < numberOfLanes; i++)
//     {
//         if (i == numberOfLanes / 2) continue; // Skip the middle marking

//         float lanePositionX = -roadWidth / 2 + i * laneWidth;
//         float currentPosition = -tileLength / 2 + markingSpacing / 2 + markingLength / 2; // Start position for markings

//         while (currentPosition + markingLength / 2 <= tileLength / 2)
//         {
//             GameObject marking = GameObject.CreatePrimitive(PrimitiveType.Cube);
//             marking.name = "LaneMarkingBetweenLanes"; // Name for debugging
//             marking.transform.parent = parent.transform;
//             marking.transform.localPosition = new Vector3(lanePositionX, markingHeight, currentPosition);
//             marking.transform.localScale = new Vector3(markingWidth, 0.05f, markingLength);
//             marking.GetComponent<MeshRenderer>().material = laneMarkingMaterial;

//             BoxCollider markingCollider = marking.GetComponent<BoxCollider>();
//             markingCollider.isTrigger = true; // Set the marking collider as a trigger

//             currentPosition += markingLength + markingSpacing; // Move to the next position
//         }
//     }
// }



// }