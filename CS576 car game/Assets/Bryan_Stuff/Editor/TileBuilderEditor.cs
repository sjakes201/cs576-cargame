using UnityEngine;
using UnityEditor;

public class TileBuilderEditorWindow : EditorWindow
{
    // Tile Settings
    private Material roadMaterial;
    private Material sidewalkMaterial;
    private Material laneMarkingMaterial;
    private Material midLaneMarkingMaterial;
    private Material curbMaterial;

    // Tile Dimensions
    private float tileWidth = 50f;
    private float tileLength = 50f;
    private float laneWidth = 4f;
    private float curbHeight = 0.2f;
    private float curbWidth = 0.5f;

    // Lane and Sidewalk Settings
    private int numberOfLanes = 4;
    private float sidewalkWidthFactor = 0.5f;

    // Mid Lane Marking Dimensions
    private float midMarkingWidth = 0.3f;
    private float midMarkingHeight = 0.05f;
    private float midMarkingSpacing = 0.75f;

    // Dotted Lane Marking Dimensions
    private float markingWidth = 0.25f;
    private float markingLength = 3.0f;
    private float markingSpacing = 1.5f;

    private float crosswalkLineWidth = 0.3f; // Width of crosswalk lines
    private float crosswalkLineSpacing = 2.0f; // Distance between the two crosswalk lines
    private float stopBarWidth = 0.5f; // Width of the stop bar
    private float crosswalkOffset = 1.0f; // Distance from intersection center to crosswalk
    private float stopBarOffset = 1.0f; // Distance from crosswalk to stop bar



    // Grid Settings
    private int gridWidth = 1;
    private int gridHeight = 1;
    private float tileSpacing = 0f;

    // Prefab Save Path
    private string prefabSavePath = "Assets/Bryan_Stuff/Prefabs/GeneratedTile_unset.prefab";

    // Curve Settings
    private int segmentsPerCurve = 10;
    private bool curveToLeft = true;

    // Tile Generation Type
    private bool generateStraightTile = true;
    private bool generateCurveTile = false;
    private bool generateFourWayIntersectionTile = false;

    // Scroll Position
    private Vector2 scrollPosition;

    [MenuItem("Tools/Tile Builder")]
    public static void ShowWindow()
    {
        GetWindow<TileBuilderEditorWindow>("Tile Builder");
    }

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

        GUILayout.Label("Tile Settings", EditorStyles.boldLabel);

        roadMaterial = (Material)EditorGUILayout.ObjectField("Road Material", roadMaterial, typeof(Material), false);
        sidewalkMaterial = (Material)EditorGUILayout.ObjectField("Sidewalk Material", sidewalkMaterial, typeof(Material), false);
        laneMarkingMaterial = (Material)EditorGUILayout.ObjectField("Lane Marking Material", laneMarkingMaterial, typeof(Material), false);
        midLaneMarkingMaterial = (Material)EditorGUILayout.ObjectField("Mid Lane Marking Material", midLaneMarkingMaterial, typeof(Material), false);
        curbMaterial = (Material)EditorGUILayout.ObjectField("Curb Material", curbMaterial, typeof(Material), false);

        GUILayout.Space(10);
        GUILayout.Label("Tile Dimensions", EditorStyles.boldLabel);
        tileWidth = EditorGUILayout.FloatField("Tile Width", tileWidth);
        tileLength = EditorGUILayout.FloatField("Tile Length", tileLength);
        laneWidth = EditorGUILayout.FloatField("Lane Width", laneWidth);
        curbHeight = EditorGUILayout.FloatField("Curb Height", curbHeight);
        curbWidth = EditorGUILayout.FloatField("Curb Width", curbWidth);

        GUILayout.Space(10);
        GUILayout.Label("Lane and Sidewalk Settings", EditorStyles.boldLabel);
        numberOfLanes = EditorGUILayout.IntField("Number of Lanes", numberOfLanes);
        sidewalkWidthFactor = EditorGUILayout.FloatField("Sidewalk Width Factor", sidewalkWidthFactor);

        GUILayout.Space(10);
        GUILayout.Label("Mid Lane Marking Dimensions", EditorStyles.boldLabel);
        midMarkingWidth = EditorGUILayout.FloatField("Mid Marking Width", midMarkingWidth);
        midMarkingHeight = EditorGUILayout.FloatField("Mid Marking Height", midMarkingHeight);
        midMarkingSpacing = EditorGUILayout.FloatField("Mid Marking Spacing", midMarkingSpacing);

        GUILayout.Space(10);
        GUILayout.Label("Dotted Lane Marking Dimensions", EditorStyles.boldLabel);
        markingWidth = EditorGUILayout.FloatField("Marking Width", markingWidth);
        markingLength = EditorGUILayout.FloatField("Marking Length", markingLength);
        markingSpacing = EditorGUILayout.FloatField("Marking Spacing", markingSpacing);

        GUILayout.Space(10);
        GUILayout.Label("Crosswalk and Stop Bar Settings", EditorStyles.boldLabel);
        crosswalkLineWidth = EditorGUILayout.FloatField("Crosswalk Line Width", crosswalkLineWidth);
        crosswalkLineSpacing = EditorGUILayout.FloatField("Crosswalk Line Spacing", crosswalkLineSpacing);
        stopBarWidth = EditorGUILayout.FloatField("Stop Bar Width", stopBarWidth);
        crosswalkOffset = EditorGUILayout.FloatField("Crosswalk Offset", crosswalkOffset);
        stopBarOffset = EditorGUILayout.FloatField("Stop Bar Offset", stopBarOffset);

        GUILayout.Space(10);
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);
        tileSpacing = EditorGUILayout.FloatField("Tile Spacing", tileSpacing);

        GUILayout.Space(10);
        GUILayout.Label("Prefab Save Path", EditorStyles.boldLabel);
        prefabSavePath = EditorGUILayout.TextField("Prefab Save Path", prefabSavePath);

        GUILayout.Space(10);
        GUILayout.Label("Curve Settings", EditorStyles.boldLabel);
        segmentsPerCurve = EditorGUILayout.IntField("Segments Per Curve", segmentsPerCurve);
        curveToLeft = EditorGUILayout.Toggle("Curve to Left", curveToLeft);

        GUILayout.Space(10);
        GUILayout.Label("Tile Generation Type", EditorStyles.boldLabel);
        generateStraightTile = EditorGUILayout.Toggle("Generate Straight Tile", generateStraightTile);
        generateCurveTile = EditorGUILayout.Toggle("Generate Curve Tile", generateCurveTile);
        generateFourWayIntersectionTile = EditorGUILayout.Toggle("Generate Four-Way Intersection Tile", generateFourWayIntersectionTile);


        GUILayout.Space(20);

        if (GUILayout.Button("Generate and Save Prefab"))
        {
            GameObject tile = GenerateTile();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (tile != null)
            {
                try
                {
                    PrefabUtility.SaveAsPrefabAsset(tile, prefabSavePath);
                    AssetDatabase.Refresh();
                    Debug.Log("Prefab saved to: " + prefabSavePath);
                } 
                catch (System.Exception e)
                {
                    Debug.LogError("Error saving prefab: " + e.Message);
                }
                finally
                {
                    DestroyImmediate(tile);
                }
            } 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Generate Tile in Scene"))
        {
            GenerateTile();
            AssetDatabase.Refresh();
        }

        GUILayout.EndScrollView();
    }
 
    private GameObject GenerateTile()
    {
        GameObject tile = null;

        // Add more statements as more tiles get added here ------------------------------------------------------------------
        if (generateStraightTile)
        {
            tile = BuildStraightRoadTile();
        }
        else if (generateCurveTile)
        {
            tile = BuildCurveTile(curveToLeft);
        }
        else if (generateFourWayIntersectionTile)
        {
            tile = BuildFourWayIntersectionTile();
        }
        else
        {
            Debug.LogWarning("No tile type selected.");
        }

        return tile;
    }


    public GameObject BuildStraightRoadTile()
    {
        // Ensure roadMaterial is not null
        if (roadMaterial == null)
        {
            Debug.LogError("Road Material is not assigned.");
            return null;
        }

        // Calculate sidewalk width based on the number of lanes and road width
        float roadWidth = numberOfLanes * laneWidth;
        float sidewalkWidth = (tileWidth - roadWidth) * sidewalkWidthFactor;

        // Create the parent object
        GameObject tile = new GameObject("StraightRoadTile");

        // Clone the road material to avoid modifying the original
        Material clonedRoadMaterial = new Material(roadMaterial);
        clonedRoadMaterial.mainTextureScale = new Vector2(roadWidth / 10f, tileLength / 10f);

        // Save the cloned material as a new asset
        string materialPath = $"Assets/Bryan_Stuff/Materials/RoadMaterial_{roadWidth}x{tileLength}.mat";
        AssetDatabase.CreateAsset(clonedRoadMaterial, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Create the road
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "RoadSurface"; // Name it for debugging
        road.transform.parent = tile.transform;
        road.transform.localScale = new Vector3(roadWidth, 0.1f, tileLength); // Flat road surface
        road.transform.localPosition = Vector3.zero;

        // Assign the saved material
        road.GetComponent<MeshRenderer>().material = clonedRoadMaterial;

        // Add debug information
        Debug.Log("Road material assigned and saved: " + materialPath);

        // Create lanes with colliders
        for (int i = 0; i < numberOfLanes; i++)
        {
            float lanePositionX = -roadWidth / 2 + laneWidth / 2 + i * laneWidth;
            CreateLaneCollider(tile, "Lane" + i, new Vector3(lanePositionX, 0.1f, 0));
        }

        // Add lane markings
        CreateMidLaneMarking(tile, new Vector3(0, 0.11f, 0), tileLength);
        CreateLaneMarkingsBetweenLanes(tile, roadWidth, tileLength);

        // Add sidewalks
        Vector3 leftSidewalkPosition = new Vector3(-tileWidth / 2 + sidewalkWidth / 2, 0.05f, 0);
        Vector3 rightSidewalkPosition = new Vector3(tileWidth / 2 - sidewalkWidth / 2, 0.05f, 0);
        CreateSidewalk(tile, "LeftSidewalk", leftSidewalkPosition, sidewalkWidth);
        CreateSidewalk(tile, "RightSidewalk", rightSidewalkPosition, sidewalkWidth);

        // Add curbs immediately after sidewalks
        Vector3 leftCurbPosition = new Vector3(leftSidewalkPosition.x + sidewalkWidth / 2 + curbWidth / 2, curbHeight / 2, 0);
        Vector3 rightCurbPosition = new Vector3(rightSidewalkPosition.x - sidewalkWidth / 2 - curbWidth / 2, curbHeight / 2, 0);
        CreateCurb(tile, "LeftCurb", leftCurbPosition);
        CreateCurb(tile, "RightCurb", rightCurbPosition);

        return tile;
    }







    //NEED TO ADJUST CURVED TILE SIDEWALK HEIGHT -------------------------------------------------------------------------------------------------------------








    public GameObject BuildCurveTile(bool curveToLeft)
    {
        float roadRadius = tileLength / 2;
        float sidewalkWidth = (tileWidth - (numberOfLanes * laneWidth)) / 2;

        // Calculate radii for sidewalks and curbs
        float sidewalkRadiusOuter = (sidewalkWidth / 2 + sidewalkWidth) + (numberOfLanes * laneWidth);
        float sidewalkRadiusInner = sidewalkWidth / 2;

        float curbRadiusOuter = sidewalkWidth + (numberOfLanes * laneWidth) - curbWidth / 2;
        float curbRadiusInner = sidewalkWidth + curbWidth / 2;

        // Create the parent object for the curved tile
        GameObject curveTile = new GameObject("CurveTile");

        // Curve direction multiplier (-1 for left curve, 1 for right curve)
        float directionMultiplier = curveToLeft ? -1 : 1;

        // Create curved road
        GameObject curvedRoad = CreateCurvedMesh(curveTile, roadRadius, laneWidth * numberOfLanes, roadMaterial, "CurvedRoad", segmentsPerCurve, directionMultiplier, 0.1f, true);
        SaveMeshAsAsset(curvedRoad.GetComponent<MeshFilter>().sharedMesh, "Assets/Bryan_Stuff/Meshes/CurvedRoad.asset");

        // Create curved sidewalks (outer and inner)
        GameObject curvedSidewalkOuter = CreateCurvedMesh(curveTile, sidewalkRadiusOuter, sidewalkWidth, sidewalkMaterial, "CurvedSidewalkOuter", segmentsPerCurve, directionMultiplier, 0.1f, true);
        SaveMeshAsAsset(curvedSidewalkOuter.GetComponent<MeshFilter>().sharedMesh, "Assets/Bryan_Stuff/Meshes/CurvedSidewalkOuter.asset");

        GameObject curvedSidewalkInner = CreateCurvedMesh(curveTile, sidewalkRadiusInner, sidewalkWidth, sidewalkMaterial, "CurvedSidewalkInner", segmentsPerCurve, directionMultiplier, 0.1f, true);
        SaveMeshAsAsset(curvedSidewalkInner.GetComponent<MeshFilter>().sharedMesh, "Assets/Bryan_Stuff/Meshes/CurvedSidewalkInner.asset");

        // Add curbs along the curved tile (outer and inner)
        CreateCurvedCurb(curveTile, curbRadiusOuter, curbWidth, curbHeight, curbMaterial, "CurvedCurbOuter", segmentsPerCurve, directionMultiplier);
        CreateCurvedCurb(curveTile, curbRadiusInner, curbWidth, curbHeight, curbMaterial, "CurvedCurbInner", segmentsPerCurve, directionMultiplier);

        // Create middle lane markings (two parallel continuous yellow lines)
        float midMarkingRadiusLeft = roadRadius - midMarkingSpacing / 2; // Left yellow line
        float midMarkingRadiusRight = roadRadius + midMarkingSpacing / 2; // Right yellow line
        CreateCurvedContinuousMarkings(curveTile, midMarkingRadiusLeft, midMarkingWidth, midLaneMarkingMaterial, "CurvedMidLaneMarkingLeft", segmentsPerCurve, directionMultiplier);
        CreateCurvedContinuousMarkings(curveTile, midMarkingRadiusRight, midMarkingWidth, midLaneMarkingMaterial, "CurvedMidLaneMarkingRight", segmentsPerCurve, directionMultiplier);

        // Create lane markings between the lanes except for the center
        for (int i = 1; i < numberOfLanes; i++)
        {
            if (i == numberOfLanes / 2) continue; // Skip the middle section (already covered by solid lines)

            float laneMarkingRadius = roadRadius - (numberOfLanes * laneWidth) / 2 + i * laneWidth;
            CreateCurvedLaneMarkings(curveTile, laneMarkingRadius, markingWidth, laneMarkingMaterial, "CurvedLaneMarkingBetweenLanes" + i, segmentsPerCurve, directionMultiplier);
        }

        curveTile.transform.position = new Vector3(25, 0, -25); // Tile got generated off center


        AssetDatabase.Refresh();
 

        return curveTile;
    }


    private void SaveMeshAsAsset(Mesh mesh, string assetPath)
    {
        if (!AssetDatabase.Contains(mesh))
        {
            AssetDatabase.CreateAsset(mesh, assetPath);
            AssetDatabase.SaveAssets(); // Ensure assets are saved
            AssetDatabase.Refresh();    // Force Unity to refresh the Asset Database
            Debug.Log($"Mesh saved as asset: {assetPath}");
        }
    }


    private void CreateCurvedCurb(GameObject parent, float radius, float width, float height, Material material, string name, int segments, float directionMultiplier)
    {
        float segmentAngle = 90f / segments;

        for (int i = 0; i < segments; i++)
        {
            float currentAngle = i * segmentAngle * directionMultiplier;
            float nextAngle = (i + 1) * segmentAngle * directionMultiplier;

            float angleRad = currentAngle * Mathf.Deg2Rad;
            float nextAngleRad = nextAngle * Mathf.Deg2Rad;

            // Calculate the current and next positions
            float x = Mathf.Sin(angleRad) * radius;
            float z = Mathf.Cos(angleRad) * radius;
            float nextX = Mathf.Sin(nextAngleRad) * radius;
            float nextZ = Mathf.Cos(nextAngleRad) * radius;

            // Calculate midpoint for proper alignment
            float midX = (x + nextX) / 2;
            float midZ = (z + nextZ) / 2;

            // Create curb segment
            GameObject curbSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curbSegment.name = $"{name}_Segment{i}";
            curbSegment.transform.parent = parent.transform;
            curbSegment.transform.localPosition = new Vector3(midX, height / 2, midZ);

            // Correctly calculate the tangent rotation for alignment
            float tangentAngle = Mathf.Atan2(nextX - x, nextZ - z) * Mathf.Rad2Deg;
            curbSegment.transform.localRotation = Quaternion.Euler(0, tangentAngle, 0);

            // Set segment size to match the distance between current and next positions
            float segmentLength = Vector3.Distance(new Vector3(x, 0, z), new Vector3(nextX, 0, nextZ));
            curbSegment.transform.localScale = new Vector3(width, height, segmentLength);

            // Assign material
            curbSegment.GetComponent<MeshRenderer>().material = material;
        }
    }


    private void CreateCurvedContinuousMarkings(GameObject parent, float radius, float width, Material material, string name, int segments, float directionMultiplier)
    {
        float segmentAngle = 90f / segments;
        float markingHeight = 0.05f; // Height of the markings above the road surface

        for (int i = 0; i < segments; i++)
        {
            float currentAngle = i * segmentAngle * directionMultiplier;
            float nextAngle = (i + 1) * segmentAngle * directionMultiplier;

            float angleRad = currentAngle * Mathf.Deg2Rad;
            float nextAngleRad = nextAngle * Mathf.Deg2Rad;

            // Calculate the current and next positions
            float x = Mathf.Sin(angleRad) * radius;
            float z = Mathf.Cos(angleRad) * radius;
            float nextX = Mathf.Sin(nextAngleRad) * radius;
            float nextZ = Mathf.Cos(nextAngleRad) * radius;

            // Calculate midpoint for proper alignment
            float midX = (x + nextX) / 2;
            float midZ = (z + nextZ) / 2;

            // Create marking segment
            GameObject markingSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            markingSegment.name = $"{name}_Segment{i}";
            markingSegment.transform.parent = parent.transform;
            markingSegment.transform.localPosition = new Vector3(midX, markingHeight, midZ);

            // Correctly calculate the tangent rotation for alignment
            float tangentAngle = Mathf.Atan2(nextX - x, nextZ - z) * Mathf.Rad2Deg;
            markingSegment.transform.localRotation = Quaternion.Euler(0, tangentAngle, 0);

            // Set segment size to match the distance between current and next positions
            float segmentLength = Vector3.Distance(new Vector3(x, 0, z), new Vector3(nextX, 0, nextZ));
            markingSegment.transform.localScale = new Vector3(width, markingHeight, segmentLength);

            // Assign material
            markingSegment.GetComponent<MeshRenderer>().material = material;
        }
    }


    private void CreateCurvedLaneMarkings(GameObject parent, float radius, float width, Material material, string name, int segments, float directionMultiplier)
    {
        float segmentAngle = 90f / segments;
        float markingHeight = 0.05f; // Set marking height to match road height
        float currentPosition = 0;   // Track position for dashed lane markings
        bool drawMarking = true;     // Toggle for marking segments to create dashes

        for (int i = 0; i < segments; i++)
        {
            float currentAngle = i * segmentAngle * directionMultiplier;
            float nextAngle = (i + 1) * segmentAngle * directionMultiplier;
            float angleRad = currentAngle * Mathf.Deg2Rad;

            // Calculate the current and next positions
            float x = Mathf.Sin(angleRad) * radius;
            float z = Mathf.Cos(angleRad) * radius;

            float nextX = Mathf.Sin(nextAngle * Mathf.Deg2Rad) * radius;
            float nextZ = Mathf.Cos(nextAngle * Mathf.Deg2Rad) * radius;

            if (drawMarking)
            {
                // Calculate midpoint for proper alignment
                float midX = (x + nextX) / 2;
                float midZ = (z + nextZ) / 2;

                // Create marking segment
                GameObject markingSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                markingSegment.name = name + "Segment" + i;
                markingSegment.transform.parent = parent.transform;
                markingSegment.transform.localPosition = new Vector3(midX, markingHeight, midZ);

                // Set the rotation so that the marking segment is tangent to the curve
                float tangentAngle = currentAngle + segmentAngle / 2 * directionMultiplier;
                markingSegment.transform.localRotation = Quaternion.Euler(0, tangentAngle + 90f * directionMultiplier, 0);

                // Set segment size to match the distance between current and next positions
                float segmentLength = Vector3.Distance(new Vector3(x, 0, z), new Vector3(nextX, 0, nextZ));
                markingSegment.transform.localScale = new Vector3(width, 0.05f, segmentLength);
                markingSegment.GetComponent<MeshRenderer>().material = material;

                // Make the collider a trigger
                BoxCollider markingCollider = markingSegment.GetComponent<BoxCollider>();
                markingCollider.isTrigger = true;
            }

            // Update marking position and toggle drawing state
            currentPosition += radius * Mathf.PI * segmentAngle / 180;

            // Toggle between drawing and not drawing if we've reached a full marking length or spacing
            if (currentPosition >= markingLength)
            {
                drawMarking = !drawMarking;
                currentPosition = 0; // Reset position tracker after switching
            }
        }
    }


    private GameObject CreateCurvedMesh(GameObject parent, float radius, float width, Material originalMaterial, string name, int segments, float directionMultiplier, float height, bool addCollider)
    {
        Debug.Log($"[CreateCurvedMesh] Starting for {name} on parent {parent.name}");

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(segments + 1) * 4];
        int[] triangles = new int[segments * 12];
        Vector2[] uvs = new Vector2[(segments + 1) * 4];

        float segmentAngle = 90f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angleRad = i * segmentAngle * Mathf.Deg2Rad * directionMultiplier;
            float u = (float)i / segments; // Calculate the horizontal UV coordinate based on the segment index

            float outerX = Mathf.Sin(angleRad) * (radius + width / 2);
            float outerZ = Mathf.Cos(angleRad) * (radius + width / 2);
            float innerX = Mathf.Sin(angleRad) * (radius - width / 2);
            float innerZ = Mathf.Cos(angleRad) * (radius - width / 2);

            vertices[i * 4] = new Vector3(outerX, height / 2, outerZ); // Top outer
            vertices[i * 4 + 1] = new Vector3(innerX, height / 2, innerZ); // Top inner
            vertices[i * 4 + 2] = new Vector3(outerX, -height / 2, outerZ); // Bottom outer
            vertices[i * 4 + 3] = new Vector3(innerX, -height / 2, innerZ); // Bottom inner

            // Assign UVs for the current segment
            uvs[i * 4] = new Vector2(u, 1); // Top outer
            uvs[i * 4 + 1] = new Vector2(u, 0); // Top inner
            uvs[i * 4 + 2] = new Vector2(u, 1); // Bottom outer
            uvs[i * 4 + 3] = new Vector2(u, 0); // Bottom inner

            if (i < segments)
            {
                int baseIndex = i * 12;

                // Top face
                triangles[baseIndex] = i * 4;
                triangles[baseIndex + 1] = i * 4 + 1;
                triangles[baseIndex + 2] = (i + 1) * 4;

                triangles[baseIndex + 3] = i * 4 + 1;
                triangles[baseIndex + 4] = (i + 1) * 4 + 1;
                triangles[baseIndex + 5] = (i + 1) * 4;

                // Bottom face
                triangles[baseIndex + 6] = i * 4 + 2;
                triangles[baseIndex + 7] = (i + 1) * 4 + 2;
                triangles[baseIndex + 8] = i * 4 + 3;

                triangles[baseIndex + 9] = i * 4 + 3;
                triangles[baseIndex + 10] = (i + 1) * 4 + 2;
                triangles[baseIndex + 11] = (i + 1) * 4 + 3;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        Debug.Log($"[CreateCurvedMesh] Generated Mesh: {mesh.vertexCount} vertices, {mesh.triangles.Length / 3} triangles");

        // Clone material and save it
        Material clonedMaterial = new Material(originalMaterial);
        clonedMaterial.mainTextureScale = new Vector2(width / 10f, radius / 10f);

        string materialPath = $"Assets/Bryan_Stuff/Materials/{name}_Material.mat";
        AssetDatabase.CreateAsset(clonedMaterial, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GameObject curvedSegment = new GameObject(name);
        curvedSegment.transform.parent = parent.transform;
        curvedSegment.AddComponent<MeshFilter>().sharedMesh = mesh;
        curvedSegment.AddComponent<MeshRenderer>().material = clonedMaterial;

        if (addCollider)
        {
            MeshCollider collider = curvedSegment.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

        Debug.Log($"[CreateCurvedMesh] CurvedSegment '{name}' added to parent '{parent.name}'");

        return curvedSegment;
    }

        
    private void CreateLaneCollider(GameObject parent, string name, Vector3 position)
    {

        // In case lane detection type stuff is needed for the straight path
        GameObject lane = new GameObject(name);
        lane.transform.parent = parent.transform;
        lane.transform.localPosition = position;

        BoxCollider collider = lane.AddComponent<BoxCollider>();
        collider.isTrigger = true;  // Set collider as a trigger
        collider.size = new Vector3(laneWidth, 0.1f, tileLength);
        // lane.tag = name; // Assign the tag Old stuff at this point
    }


    private void CreateSidewalk(GameObject parent, string name, Vector3 position, float width)
    {
        GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sidewalk.name = name; // Name it for debugging
        sidewalk.transform.parent = parent.transform;
        sidewalk.transform.localPosition = position;
        sidewalk.transform.localScale = new Vector3(width, 0.1f, tileLength); // Sidewalk width and height
        sidewalk.GetComponent<MeshRenderer>().material = sidewalkMaterial;
        sidewalk.AddComponent<BoxCollider>(); // Add collider to sidewalk
    }


    private void CreateCurb(GameObject parent, string name, Vector3 position)
    {
        GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curb.name = name; // Name it for debugging
        curb.transform.parent = parent.transform;
        curb.transform.localPosition = position;
        curb.transform.localScale = new Vector3(curbWidth, curbHeight, tileLength); // Curb dimensions
        curb.GetComponent<MeshRenderer>().material = curbMaterial; // Same material as sidewalk
    }
    
    
    private void CreateMidLaneMarking(GameObject parent, Vector3 startPosition, float tileLength)
    {
        // Set the marking position to be slightly above or at the road's surface
        float markingHeight = 0.05f;

        // Create the first middle marking line
        GameObject leftMarking = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftMarking.name = "MidLaneMarkingLeft"; // Name for debugging
        leftMarking.transform.parent = parent.transform;
        leftMarking.transform.localPosition = new Vector3(-midMarkingSpacing / 2, markingHeight, 0); 
        leftMarking.transform.localScale = new Vector3(midMarkingWidth, midMarkingHeight, tileLength);
        leftMarking.GetComponent<MeshRenderer>().material = midLaneMarkingMaterial;

        BoxCollider leftCollider = leftMarking.GetComponent<BoxCollider>();
        leftCollider.isTrigger = true; // Set the marking collider as a trigger

        // Create the second middle marking line
        GameObject rightMarking = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightMarking.name = "MidLaneMarkingRight"; // Name for debugging
        rightMarking.transform.parent = parent.transform;
        rightMarking.transform.localPosition = new Vector3(midMarkingSpacing / 2, markingHeight, 0);
        rightMarking.transform.localScale = new Vector3(midMarkingWidth, midMarkingHeight, tileLength);
        rightMarking.GetComponent<MeshRenderer>().material = midLaneMarkingMaterial;

        BoxCollider rightCollider = rightMarking.GetComponent<BoxCollider>();
        rightCollider.isTrigger = true; // Set the marking collider as a trigger
    }



    private void CreateLaneMarkingsBetweenLanes(GameObject parent, float roadWidth, float tileLength)
    {
        float markingHeight = 0.05f;

        for (int i = 1; i < numberOfLanes; i++)
        {
            if (i == numberOfLanes / 2) continue; // Skip the middle marking

            float lanePositionX = -roadWidth / 2 + i * laneWidth;
            float currentPosition = -tileLength / 2 + markingSpacing / 2 + markingLength / 2; // Start position for markings

            while (currentPosition + markingLength / 2 <= tileLength / 2)
            {
                GameObject marking = GameObject.CreatePrimitive(PrimitiveType.Cube);
                marking.name = "LaneMarkingBetweenLanes"; // Name for debugging
                marking.transform.parent = parent.transform;
                marking.transform.localPosition = new Vector3(lanePositionX, markingHeight, currentPosition);
                marking.transform.localScale = new Vector3(markingWidth, 0.05f, markingLength);
                marking.GetComponent<MeshRenderer>().material = laneMarkingMaterial;

                BoxCollider markingCollider = marking.GetComponent<BoxCollider>();
                markingCollider.isTrigger = true; // Set the marking collider as a trigger

                currentPosition += markingLength + markingSpacing; // Move to the next position
            }
        }
    }


public GameObject BuildFourWayIntersectionTile()
{
    // Create the parent object for the intersection tile
    GameObject intersectionTile = new GameObject("FourWayIntersectionTile");

    float roadWidth = numberOfLanes * laneWidth;

    // Create horizontal and vertical roads
    CreateIntersectionRoad(intersectionTile, "HorizontalRoad", new Vector3(0, 0, 0), new Vector3(tileWidth, 0.1f, roadWidth));
    CreateIntersectionRoad(intersectionTile, "VerticalRoad", new Vector3(0, 0, 0), new Vector3(roadWidth, 0.1f, tileWidth));

    // Add sidewalks at corners
    float sidewalkSize = (tileWidth - roadWidth) / 2; // Size of each sidewalk square

    Vector3 topLeftCorner = new Vector3(-tileWidth / 2 + sidewalkSize / 2, 0.05f, tileWidth / 2 - sidewalkSize / 2);
    Vector3 topRightCorner = new Vector3(tileWidth / 2 - sidewalkSize / 2, 0.05f, tileWidth / 2 - sidewalkSize / 2);
    Vector3 bottomLeftCorner = new Vector3(-tileWidth / 2 + sidewalkSize / 2, 0.05f, -tileWidth / 2 + sidewalkSize / 2);
    Vector3 bottomRightCorner = new Vector3(tileWidth / 2 - sidewalkSize / 2, 0.05f, -tileWidth / 2 + sidewalkSize / 2);

    CreateSidewalk(intersectionTile, "TopLeftSidewalk", topLeftCorner, new Vector3(sidewalkSize, 0.1f, sidewalkSize));
    CreateSidewalk(intersectionTile, "TopRightSidewalk", topRightCorner, new Vector3(sidewalkSize, 0.1f, sidewalkSize));
    CreateSidewalk(intersectionTile, "BottomLeftSidewalk", bottomLeftCorner, new Vector3(sidewalkSize, 0.1f, sidewalkSize));
    CreateSidewalk(intersectionTile, "BottomRightSidewalk", bottomRightCorner, new Vector3(sidewalkSize, 0.1f, sidewalkSize));

    // Add curbs
    CreateIntersectionCurbs(intersectionTile);

    // Add lane markings (e.g., stop lines, crosswalks)
    CreateIntersectionMarkings(intersectionTile, roadWidth, roadWidth);

    return intersectionTile;
}



    // Create curbs along the road edges
private void CreateIntersectionCurbs(GameObject parent)
{
    float roadWidth = numberOfLanes * laneWidth;
    float sidewalkSize = (tileWidth - roadWidth) / 2;

    float curbHalfHeight = curbHeight / 2;

    // Top-left curb
    Vector3 topLeftHorizontalCurb = new Vector3(-tileWidth / 2 + sidewalkSize / 2, curbHalfHeight, tileWidth / 2 - sidewalkSize - curbWidth / 2);
    Vector3 topLeftVerticalCurb = new Vector3(-tileWidth / 2 + sidewalkSize + curbWidth / 2, curbHalfHeight, tileWidth / 2 - sidewalkSize / 2);

    CreateCurb(parent, "TopLeftHorizontalCurb", topLeftHorizontalCurb, new Vector3(sidewalkSize, curbHeight, curbWidth));
    CreateCurb(parent, "TopLeftVerticalCurb", topLeftVerticalCurb, new Vector3(curbWidth, curbHeight, sidewalkSize));

    // Top-right curb
    Vector3 topRightHorizontalCurb = new Vector3(tileWidth / 2 - sidewalkSize / 2, curbHalfHeight, tileWidth / 2 - sidewalkSize - curbWidth / 2);
    Vector3 topRightVerticalCurb = new Vector3(tileWidth / 2 - sidewalkSize - curbWidth / 2, curbHalfHeight, tileWidth / 2 - sidewalkSize / 2);

    CreateCurb(parent, "TopRightHorizontalCurb", topRightHorizontalCurb, new Vector3(sidewalkSize, curbHeight, curbWidth));
    CreateCurb(parent, "TopRightVerticalCurb", topRightVerticalCurb, new Vector3(curbWidth, curbHeight, sidewalkSize));

    // Bottom-left curb
    Vector3 bottomLeftHorizontalCurb = new Vector3(-tileWidth / 2 + sidewalkSize / 2, curbHalfHeight, -tileWidth / 2 + sidewalkSize + curbWidth / 2);
    Vector3 bottomLeftVerticalCurb = new Vector3(-tileWidth / 2 + sidewalkSize + curbWidth / 2, curbHalfHeight, -tileWidth / 2 + sidewalkSize / 2);

    CreateCurb(parent, "BottomLeftHorizontalCurb", bottomLeftHorizontalCurb, new Vector3(sidewalkSize, curbHeight, curbWidth));
    CreateCurb(parent, "BottomLeftVerticalCurb", bottomLeftVerticalCurb, new Vector3(curbWidth, curbHeight, sidewalkSize));

    // Bottom-right curb
    Vector3 bottomRightHorizontalCurb = new Vector3(tileWidth / 2 - sidewalkSize / 2, curbHalfHeight, -tileWidth / 2 + sidewalkSize + curbWidth / 2);
    Vector3 bottomRightVerticalCurb = new Vector3(tileWidth / 2 - sidewalkSize - curbWidth / 2, curbHalfHeight, -tileWidth / 2 + sidewalkSize / 2);

    CreateCurb(parent, "BottomRightHorizontalCurb", bottomRightHorizontalCurb, new Vector3(sidewalkSize, curbHeight, curbWidth));
    CreateCurb(parent, "BottomRightVerticalCurb", bottomRightVerticalCurb, new Vector3(curbWidth, curbHeight, sidewalkSize));
}

// Generic curb creation function
void CreateCurb(GameObject parent, string name, Vector3 position, Vector3 scale)
{
    GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
    curb.name = name;
    curb.transform.parent = parent.transform;
    curb.transform.localPosition = position;
    curb.transform.localScale = scale;
    curb.GetComponent<MeshRenderer>().material = curbMaterial; // Use predefined curb material
}

    // Helper method to create the roads
    private void CreateIntersectionRoad(GameObject parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = name;
        road.transform.parent = parent.transform;
        road.transform.localPosition = position;
        road.transform.localScale = scale;
        road.GetComponent<MeshRenderer>().material = roadMaterial;
        road.AddComponent<BoxCollider>(); // Optional collider
    }

    // Helper method to create the sidewalks
    private void CreateSidewalk(GameObject parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sidewalk.name = name;
        sidewalk.transform.parent = parent.transform;
        sidewalk.transform.localPosition = position;
        sidewalk.transform.localScale = scale;
        sidewalk.GetComponent<MeshRenderer>().material = sidewalkMaterial;
        sidewalk.AddComponent<BoxCollider>(); // Optional collider
    }


private void CreateIntersectionMarkings(GameObject parent, float roadWidth, float clearWidth)
{
    float markingHeight = 0.05f; // Slightly above the road surface
    float solidLineThickness = midMarkingWidth; // Thickness of the center solid lines
    float dashedLineThickness = markingWidth; // Thickness of dashed markings
    float dashLength = markingLength; // Length of each dashed marking
    float dashSpacing = markingSpacing; // Spacing between dashed markings

    float sidewalkWidth = (tileWidth - roadWidth) / 2; // Sidewalk width
    float laneOffset = laneWidth / 2; // Offset to center lanes
    float centerMarkingOffset = midMarkingSpacing / 2; // Distance between center solid lines

    // ** Crosswalks and Stop Bars **
    // Take absolute values for dynamic clearWidth calculation but preserve original values for positions
    float crosswalkOffsetAbs = Mathf.Abs(crosswalkOffset);
    float stopBarOffsetAbs = Mathf.Abs(stopBarOffset);

    // Adjust clearWidth dynamically to include crosswalk and stop bar offsets
    clearWidth = roadWidth + (crosswalkOffsetAbs + crosswalkLineSpacing + stopBarOffsetAbs * 2 - stopBarWidth - 2 * crosswalkLineWidth);

    // Adjust stop bar position relative to crosswalk
    float stopBarAdjustment = stopBarOffset + crosswalkLineSpacing / 2;

    // Horizontal Crosswalks and Stop Bars
    float crosswalkPositionTop = tileWidth / 2 - sidewalkWidth - crosswalkOffset;
    float stopBarPositionTop = crosswalkPositionTop - stopBarAdjustment;

    CreateCrosswalk(parent, "HorizontalCrosswalkTop", 
        new Vector3(0, markingHeight, crosswalkPositionTop), roadWidth, true);
    CreateStopBar(parent, "HorizontalStopBarTop", 
        new Vector3(0, markingHeight, stopBarPositionTop), roadWidth, true);

    float crosswalkPositionBottom = -tileWidth / 2 + sidewalkWidth + crosswalkOffset;
    float stopBarPositionBottom = crosswalkPositionBottom + stopBarAdjustment;

    CreateCrosswalk(parent, "HorizontalCrosswalkBottom", 
        new Vector3(0, markingHeight, crosswalkPositionBottom), roadWidth, true);
    CreateStopBar(parent, "HorizontalStopBarBottom", 
        new Vector3(0, markingHeight, stopBarPositionBottom), roadWidth, true);



    float crosswalkPositionRight = tileWidth / 2 - sidewalkWidth - crosswalkOffset;
    float stopBarPositionRight = crosswalkPositionRight - stopBarAdjustment;

    CreateCrosswalk(parent, "VerticalCrosswalkRight", 
        new Vector3(crosswalkPositionRight, markingHeight, 0), roadWidth, false);
    CreateStopBar(parent, "VerticalStopBarRight", 
        new Vector3(stopBarPositionRight, markingHeight, 0), roadWidth, false);

    // Vertical Crosswalks and Stop Bars
    float crosswalkPositionLeft = -tileWidth / 2 + sidewalkWidth + crosswalkOffset;
    float stopBarPositionLeft = crosswalkPositionLeft + stopBarAdjustment;


    CreateCrosswalk(parent, "VerticalCrosswalkLeft", 
        new Vector3(crosswalkPositionLeft, markingHeight, 0), roadWidth, false);
    CreateStopBar(parent, "VerticalStopBarLeft", 
        new Vector3(stopBarPositionLeft, markingHeight, 0), roadWidth, false);

    // ** Solid Lines in the Center **
    // Adjust markings to avoid overlap with clearWidth
    CreateSolidMarking(parent, "CenterLeftMarkingVertical", 
        new Vector3(-centerMarkingOffset, markingHeight, 0), tileWidth, solidLineThickness, true, clearWidth);
    CreateSolidMarking(parent, "CenterRightMarkingVertical", 
        new Vector3(centerMarkingOffset, markingHeight, 0), tileWidth, solidLineThickness, true, clearWidth);

    CreateSolidMarking(parent, "CenterLeftMarkingHorizontal", 
        new Vector3(0, markingHeight, -centerMarkingOffset), tileWidth, solidLineThickness, false, clearWidth);
    CreateSolidMarking(parent, "CenterRightMarkingHorizontal", 
        new Vector3(0, markingHeight, centerMarkingOffset), tileWidth, solidLineThickness, false, clearWidth);

    // ** Dashed Lines Between Lanes **
    CreateDashedMarking(parent, $"DashedMarkingHorizontalLane1", 
        new Vector3(-laneWidth, markingHeight, 0), tileWidth, dashLength, dashSpacing, dashedLineThickness, true, clearWidth);
    CreateDashedMarking(parent, $"DashedMarkingHorizontalLane2", 
        new Vector3(laneWidth, markingHeight, 0), tileWidth, dashLength, dashSpacing, dashedLineThickness, true, clearWidth);

    CreateDashedMarking(parent, $"DashedMarkingVerticalLane1", 
        new Vector3(0, markingHeight, -laneWidth), tileWidth, dashLength, dashSpacing, dashedLineThickness, false, clearWidth);
    CreateDashedMarking(parent, $"DashedMarkingVerticalLane2", 
        new Vector3(0, markingHeight, laneWidth), tileWidth, dashLength, dashSpacing, dashedLineThickness, false, clearWidth);
}


// Updated solid marking method
private void CreateSolidMarking(GameObject parent, string name, Vector3 position, float length, float thickness, bool isHorizontal, float clearWidth)
{
    // Adjust the length to be split into two segments on either side of the clear area
    float segmentLength = (length - clearWidth) / 2;

    // First segment (before the clear area)
    GameObject marking1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    marking1.name = $"{name}_Segment1";
    marking1.transform.parent = parent.transform;

    if (isHorizontal)
    {
        marking1.transform.localPosition = new Vector3(position.x, position.y, position.z - clearWidth / 2 - segmentLength / 2);
        marking1.transform.localScale = new Vector3(thickness, 0.01f, segmentLength);
    }
    else
    {
        marking1.transform.localPosition = new Vector3(position.x - clearWidth / 2 - segmentLength / 2, position.y, position.z);
        marking1.transform.localScale = new Vector3(segmentLength, 0.01f, thickness);
    }

    marking1.GetComponent<MeshRenderer>().material = midLaneMarkingMaterial;

    // Second segment (after the clear area)
    GameObject marking2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    marking2.name = $"{name}_Segment2";
    marking2.transform.parent = parent.transform;

    if (isHorizontal)
    {
        marking2.transform.localPosition = new Vector3(position.x, position.y, position.z + clearWidth / 2 + segmentLength / 2);
        marking2.transform.localScale = new Vector3(thickness, 0.01f, segmentLength);
    }
    else
    {
        marking2.transform.localPosition = new Vector3(position.x + clearWidth / 2 + segmentLength / 2, position.y, position.z);
        marking2.transform.localScale = new Vector3(segmentLength, 0.01f, thickness);
    }

    marking2.GetComponent<MeshRenderer>().material = midLaneMarkingMaterial;
}

// Updated dashed marking method
private void CreateDashedMarking(GameObject parent, string name, Vector3 startPosition, float length, float dashLength, float dashSpacing, float thickness, bool isHorizontal, float clearWidth)
{
    float currentPosition = -length / 2 + dashSpacing / 2 + dashLength / 2;

    while (currentPosition + dashLength / 2 <= length / 2)
    {
        // Skip markings in the clear area
        if (Mathf.Abs(currentPosition) < clearWidth / 2)
        {
            currentPosition += dashLength + dashSpacing;
            continue;
        }

        GameObject marking = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marking.name = $"{name}_Segment";
        marking.transform.parent = parent.transform;

        if (isHorizontal)
        {
            marking.transform.localPosition = new Vector3(startPosition.x, startPosition.y, currentPosition);
            marking.transform.localScale = new Vector3(thickness, 0.01f, dashLength);
        }
        else
        {
            marking.transform.localPosition = new Vector3(currentPosition, startPosition.y, startPosition.z);
            marking.transform.localScale = new Vector3(dashLength, 0.01f, thickness);
        }

        marking.GetComponent<MeshRenderer>().material = laneMarkingMaterial;

        currentPosition += dashLength + dashSpacing; // Move to the next dashed marking position
    }
}

private void CreateCrosswalk(GameObject parent, string name, Vector3 position, float roadWidth, bool isHorizontal)
{
    // First crosswalk line
    GameObject crosswalkLine1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    crosswalkLine1.name = $"{name}_Line1";
    crosswalkLine1.transform.parent = parent.transform;

    if (isHorizontal)
    {
        crosswalkLine1.transform.localPosition = new Vector3(position.x, position.y, position.z - crosswalkLineSpacing / 2);
        crosswalkLine1.transform.localScale = new Vector3(roadWidth, 0.01f, crosswalkLineWidth);
    }
    else
    {
        crosswalkLine1.transform.localPosition = new Vector3(position.x - crosswalkLineSpacing / 2, position.y, position.z);
        crosswalkLine1.transform.localScale = new Vector3(crosswalkLineWidth, 0.01f, roadWidth);
    }

    crosswalkLine1.GetComponent<MeshRenderer>().material = laneMarkingMaterial;

    // Second crosswalk line
    GameObject crosswalkLine2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    crosswalkLine2.name = $"{name}_Line2";
    crosswalkLine2.transform.parent = parent.transform;

    if (isHorizontal)
    {
        crosswalkLine2.transform.localPosition = new Vector3(position.x, position.y, position.z + crosswalkLineSpacing / 2);
        crosswalkLine2.transform.localScale = new Vector3(roadWidth, 0.01f, crosswalkLineWidth);
    }
    else
    {
        crosswalkLine2.transform.localPosition = new Vector3(position.x + crosswalkLineSpacing / 2, position.y, position.z);
        crosswalkLine2.transform.localScale = new Vector3(crosswalkLineWidth, 0.01f, roadWidth);
    }

    crosswalkLine2.GetComponent<MeshRenderer>().material = laneMarkingMaterial;
}

private void CreateStopBar(GameObject parent, string name, Vector3 position, float roadWidth, bool isHorizontal)
{
    GameObject stopBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
    stopBar.name = name;
    stopBar.transform.parent = parent.transform;

    if (isHorizontal)
    {
        stopBar.transform.localPosition = new Vector3(position.x, position.y, position.z);
        stopBar.transform.localScale = new Vector3(roadWidth, 0.01f, stopBarWidth);
    }
    else
    {
        stopBar.transform.localPosition = new Vector3(position.x, position.y, position.z);
        stopBar.transform.localScale = new Vector3(stopBarWidth, 0.01f, roadWidth);
    }

    stopBar.GetComponent<MeshRenderer>().material = laneMarkingMaterial;
}

 
}