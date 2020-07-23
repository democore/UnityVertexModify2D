using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VertexModify2D
{
    [ExecuteInEditMode]
    public class GridDeformationPointDrawer : MonoBehaviour
    {
        private Transform _transform;
        [HideInInspector]
        public GridDeformer GridDeformer;
        public Color color;

        private void Awake()
        {
            _transform = transform;
        }

        public void OnDrawGizmos()
        {
            if (GridDeformer == null) return;
            if (!GridDeformer.DrawGizmos) return;

            Gizmos.color = color;

#if UNITY_EDITOR
            if (UnityEditor.Selection.gameObjects.Any(x => x == gameObject))
            {
                Gizmos.color = Color.white;
            }
            Gizmos.DrawIcon(_transform.position, "blendSampler", false, Gizmos.color);
#endif

        }
    }
}
