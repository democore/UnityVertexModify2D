using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertexModify2D
{
    [System.Serializable]
    public class GridDeformationVertexData
    {
        public GridDeformationVertexData(VertexData vertexData)
        {
            VertexData = vertexData;
        }

        public VertexData VertexData;
        public Color Color;
        public Vector2 NormalizedInitialPosition;
        public float[] XFFD;
        public float[] YFFD;

        public void DrawWeightedGizmo()
        {
            Gizmos.color = Color;
            Gizmos.DrawIcon(VertexData.WorldPosition, "blendKey", false, Gizmos.color);
        }
    }
}