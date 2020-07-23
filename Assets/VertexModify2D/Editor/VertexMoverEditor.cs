using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VertexModify2D
{
    [CustomEditor(typeof(VertexMover))]
    public class VertexMoverEditor : Editor
    {
        [MenuItem("GameObject/VertexModify2D/Vertex Mover", false, 10)]
        public static void CreateTextArea()
        {
            GameObject go = new GameObject("Vertex Mover");
            var vertexMover = go.AddComponent<VertexMover>();

            var target = new GameObject("VertexMover Target");
            var transform = target.transform;
            transform.position = go.transform.position;
            transform.parent = go.transform;
            transform.localScale = Vector3.one;
            vertexMover.Target = transform;
        }

        public override void OnInspectorGUI()
        {
            var vertexMover = (VertexMover)target;
            DrawDefaultInspector();
            if(vertexMover.Target == null)
            {
                if (GUILayout.Button("Create Target"))
                {
                    var target = new GameObject("VertexMover Target");
                    var transform = target.transform;
                    transform.position = vertexMover.transform.position;
                    transform.parent = vertexMover.transform;
                    transform.localScale = Vector3.one;
                    vertexMover.Target = transform;
                }
            }
        }
    }
}