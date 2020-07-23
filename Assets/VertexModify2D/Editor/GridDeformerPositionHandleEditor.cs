using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VertexModify2D
{
    [CustomEditor(typeof(GridDeformer))]
    public class GridDeformerPositionHandleEditor : Editor
    {
        [MenuItem("GameObject/VertexModify2D/Grid Deformer", false, 10)]
        public static void CreateTextArea()
        {
            GameObject go = new GameObject("Grid Deformer");
            var gridDeformer = go.AddComponent<GridDeformer>();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Reset"))
            {
                var gridDeformer = (GridDeformer)target;
                gridDeformer.ResetPoints();
                SceneView.RepaintAll();
            }
        }
    }
}