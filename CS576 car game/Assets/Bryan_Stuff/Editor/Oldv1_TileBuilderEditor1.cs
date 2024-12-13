// using UnityEngine;
// using UnityEditor;

// [CustomEditor(typeof(TileBuilder))]
// public class TileBuilderEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         // Draw default inspector UI
//         DrawDefaultInspector();

//         // Get a reference to the TileBuilder instance
//         TileBuilder tileBuilder = (TileBuilder)target;

//         // Generate and save prefab when button is clicked
//         if (GUILayout.Button("Generate and Save Prefab"))
//         {
//             GameObject tile = null;

//             // Determine which tile to generate
//             if (tileBuilder.GenerateStraightTile)
//             {
//                 tile = tileBuilder.BuildStraightRoadTile();
//             }
//             else if (tileBuilder.GenerateCurveTile)
//             {
//                 tile = tileBuilder.BuildCurveTile(tileBuilder.CurveToLeft);
//             }
//             else
//             {
//                 Debug.LogWarning("No tile type selected.");
//                 return;
//             }

//             // Check if the generated tile and path are valid
//             string savePath = tileBuilder.GetPrefabSavePath();
//             if (tile != null && !string.IsNullOrEmpty(savePath))
//             {
//                 try
//                 {
//                     // Save the generated tile as a prefab
//                     PrefabUtility.SaveAsPrefabAsset(tile, savePath);
//                     AssetDatabase.Refresh();
//                     Debug.Log("Prefab saved to: " + savePath);
//                 }
//                 catch (System.Exception e)
//                 {
//                     Debug.LogError("Error saving prefab: " + e.Message);
//                 }

//                 // Do not destroy the generated tile so it remains in the scene
//                 // This allows the user to see and interact with the tile
//             }
//             else
//             {
//                 Debug.LogWarning("Failed to generate or save the prefab. Check prefab path or tile generation.");
//             }

//             // Mark TileBuilder as dirty to ensure Unity updates it properly
//             EditorUtility.SetDirty(tileBuilder);
//         }
//     }
// }
