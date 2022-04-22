using AI.Pathfinding;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGraph))]
public class GraphInspector : Editor
{
    private int generationGridColumns;
    private int generationGridRows;
    private float generationGridCellSize;
    private float generationObstacleAvoidance;

    public override void OnInspectorGUI()
    {
        DrawSerializedProperties();

        var graph = (GridGraph)target;

        if (graph.Count > 0)
        {
            if (GUILayout.Button("Clear Graph"))
                graph.Clear();
        }

        DrawGenerationControls();
    }

    private void DrawSerializedProperties()
    {
        var begin = serializedObject.GetIterator();
        if (begin != null)
        {
            var it = begin.Copy();
            if (it.NextVisible(true))
            {
                do EditorGUILayout.PropertyField(it);
                while (it.NextVisible(false));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawHorizontalLine(int height = 1)
    {
        var rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1f));
    }

    private void DrawGenerationControls()
    {
        var graph = (GridGraph)target;

        EditorGUI.BeginChangeCheck();
        {
            if (graph.Count == 0)
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("Generation Options", EditorStyles.boldLabel);

                    generationGridColumns = EditorGUILayout.IntField("Number of Columns", graph.generationGridColumns);
                    generationGridColumns = generationGridColumns < 0 ? 0 : generationGridColumns;
                    generationGridRows = EditorGUILayout.IntField("Number of Rows", graph.generationGridRows);
                    generationGridRows = generationGridRows < 0 ? 0 : generationGridRows;
                    generationGridCellSize = EditorGUILayout.FloatField("Grid Cell Size", graph.generationGridCellSize);
                    generationGridCellSize = generationGridCellSize < 0 ? 0 : generationGridCellSize;
                    generationObstacleAvoidance = EditorGUILayout.FloatField("Obstacle Avoidance", graph.generationObstacleAvoidance);
                    generationObstacleAvoidance = generationObstacleAvoidance < 0 ? 0 : generationObstacleAvoidance;

                    EditorGUILayout.Space(10);
                    if (GUILayout.Button("Generate Graph"))
                    {
                        graph.GenerateGrid();
                        EditorUtility.SetDirty(graph);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        if (!EditorGUI.EndChangeCheck()) return;
        
        Undo.RecordObject(target, "Graph 'Generation Options' inspector changes");
        graph.generationGridColumns = generationGridColumns;
        graph.generationGridRows = generationGridRows;
        graph.generationGridCellSize = generationGridCellSize;
        graph.generationObstacleAvoidance = generationObstacleAvoidance;
    }
}
