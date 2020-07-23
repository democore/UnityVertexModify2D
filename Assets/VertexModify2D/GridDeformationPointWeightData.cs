using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertexModify2D
{
    [System.Serializable]
    public class GridDeformationPointWeightData
    {
        public GridDeformationPointWeightData(GridDeformationPoint point)
        {
            Point = point;
        }
        public GridDeformationPoint Point;
        public float Weight;
    }
}