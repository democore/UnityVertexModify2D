using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace VertexModify2D
{
    [ExecuteInEditMode]
    public class GridDeformer : MonoBehaviour
    {
        public List<VertexExposer> VertexExposers;
        public bool DrawGizmos = true;

        private Vector2 _size = new Vector2(1f, 1f);
        private bool _showWeights;
        private List<GridDeformationVertexData> _vertexData = new List<GridDeformationVertexData>();
        private GridDeformationPoint[,] _sortedDeformationPoints =
            new GridDeformationPoint[_ControlPoints, _ControlPoints];
        private float _previousSubdevisionsCount;
        // Currently only works with exactly 3 Points because of GetSurfaceFFDAlterator
        private const int _ControlPoints = 3;

        private void Start()
        {
            ResetPoints();
        }

        private void Update()
        {
            if ((_vertexData == null || _vertexData.Count == 0 )
                && 
                (VertexExposers != null && VertexExposers.Count > 0))
                ResetVertexData();
            ApplyWeights();
        }

        private void OnDisable()
        {
            foreach (var vertexExposer in VertexExposers)
            {
                foreach (var vertex in vertexExposer.VertexData)
                {
                    vertex.SetXY(0f, 0f, GetHashCode());
                }
            }
        }

        private void ApplyWeights()
        {
            foreach (var vertex in _vertexData)
            {
                var localScale = vertex.VertexData.RendererTransform.lossyScale;

                UnityEngine.Profiling.Profiler.BeginSample("Deform");
                var offset = new Vector3();
                var color = new Color();
                foreach (var deformationPoint in _sortedDeformationPoints)
                {
                    // After reloading the assembly, the _sortedDeformationPoints array seems to be nulled..?
                    // So we reload the whole thing in that case... 
                    if (deformationPoint == null)
                    {
                        ResetPoints();
                        return;
                    }

                    var FFDMultiplier = vertex.XFFD[deformationPoint.X] * vertex.YFFD[deformationPoint.Y];

                    var pointOffset = deformationPoint.Offset;

                    offset += new Vector3(pointOffset.x / localScale.x,
                        pointOffset.y / localScale.y) * FFDMultiplier;

                    color += deformationPoint.Color * FFDMultiplier;
                }
                color.a = 1f;
                vertex.Color = color;
                vertex.VertexData.SetXY(offset.x, offset.y, GetHashCode());
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        /// <summary>
        /// Gets the global minimum and maximum of the vertex positions.
        /// </summary>
        /// <returns></returns>
        private MinMax FindMinMax()
        {
            var minMax = new MinMax();
            _vertexData.ForEach(x => minMax.CheckXY(x.VertexData.WorldPosition));
            return minMax;
        }

        // From https://github.com/Niklaren/FFD/blob/master/Assets/FFD2D.cs
        private float[] GetSurfaceFFDAlterator(float coordinate)
        {
            var returner = new float[3];

            returner[0] = (1.0f - coordinate) * (1.0f - coordinate);
            returner[1] = 2.0f * coordinate * (1.0f - coordinate);
            returner[2] = coordinate * coordinate;

            return returner;
        }

        /// <summary>
        /// Initialize GridDeformationVertexData by grabbing all the vertices from all the VertexExposers
        /// and creating their normalized position.
        /// </summary>
        private void ResetVertexData()
        {
            if (_vertexData == null)
                _vertexData = new List<GridDeformationVertexData>();
            else
                _vertexData.Clear();

            _vertexData
                .AddRange(
                    VertexExposers.Where(x => x.VertexData != null)
                    .SelectMany(x => x.VertexData)
                    .Select(x => new GridDeformationVertexData(x)));

            var minMax = FindMinMax();
            foreach (var vertex in _vertexData)
            {
                var initialPos = vertex.VertexData.WorldPosition;
                vertex.NormalizedInitialPosition =
                    new Vector2(minMax.NormalizeX(initialPos.x), minMax.NormalizeY(initialPos.y));

                vertex.XFFD = GetSurfaceFFDAlterator(vertex.NormalizedInitialPosition.x);
                vertex.YFFD = GetSurfaceFFDAlterator(vertex.NormalizedInitialPosition.y);
            }

        }

        public void OnDrawGizmos()
        {
            if (!DrawGizmos) return;

            // Draw Lines
            for (int x = 0; x < _ControlPoints; x++)
            {
                for (int y = 0; y < _ControlPoints; y++)
                {
                    var point = _sortedDeformationPoints[x, y];
                    if (point == null) continue;

                    if (x < _ControlPoints - 1)
                    {
                        var rightPoint = _sortedDeformationPoints[x + 1, y];
                        Gizmos.DrawLine(point.Position, rightPoint.Position);
                    }
                    if (y < _ControlPoints - 1)
                    {
                        var bottomPoint = _sortedDeformationPoints[x, y + 1];
                        Gizmos.DrawLine(point.Position, bottomPoint.Position);
                    }
                }
            }

            // Draw vertex positions if desired
            if (_showWeights)
            {
                _vertexData.ForEach(vert => vert.DrawWeightedGizmo());
            }
        }

        private void SetSizeAndPositionFromSpriteRendererBounds()
        {
            var minMax = FindMinMax();
            transform.position = minMax.Center;
            _size = minMax.Bounds;
        }

        public void ResetPoints()
        {
            if (VertexExposers == null || VertexExposers.Count == 0)
            {
                return;
            }

            ResetVertexData();
            SetSizeAndPositionFromSpriteRendererBounds();

            var pos = transform.position;

            var halfXSize = _size.x / 2f;
            var halfYSize = _size.y / 2f;

            var partX = _size.x / (float)(_ControlPoints - 1);
            var partY = _size.y / (float)(_ControlPoints - 1);
            var topLeft = pos + new Vector3(-halfXSize, halfYSize);
            for (int x = 0; x < _ControlPoints; x++)
            {
                for (int y = 0; y < _ControlPoints; y++)
                {
                    var position = topLeft + new Vector3(x * partX, -y * partY);
                    _sortedDeformationPoints[x, y] =
                        new GridDeformationPoint(position, transform, x, (_ControlPoints - 1) - y, this);
                }
            }
        }
    }
}