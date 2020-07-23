using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertexModify2D
{
    [System.Serializable]
    public class GridDeformationPoint
    {
        public Color Color;
        public Vector3 InitialPosition;
        public Vector3 InitialLocalPosition;
        public Transform Root;
        public GameObject GameObject;
        public Transform Transform;
        public int X;
        public int Y;
        public GridDeformationPointDrawer GridDeformationPointDrawer;

        public GridDeformationPoint(Vector3 position, Transform root, int x, int y, GridDeformer gridDeformer)
        {
            Color = Color.HSVToRGB(Random.value, 1f, 1f);

            var name = $"Grid Deformation Point {x} {y}";
            GameObject = root.Find(name)?.gameObject;
            if (GameObject == null)
                GameObject = new GameObject(name);

            if (GameObject.GetComponent<GridDeformationPointDrawer>() == null)
            {
                GridDeformationPointDrawer = GameObject.AddComponent<GridDeformationPointDrawer>();
                GridDeformationPointDrawer.color = Color;
                GridDeformationPointDrawer.GridDeformer = gridDeformer;
            }
            Transform = GameObject.transform;
            Transform.SetParent(root);
            Transform.position = position;

            InitialPosition = position;
            Root = root;
            InitialLocalPosition = LocalPosition;

            X = x;
            Y = y;
        }

        public Vector3 Position => Transform.position;

        public Vector3 LocalPosition => Position - Root.position;

        public Vector3 Offset => LocalPosition - InitialLocalPosition;
    }
}