using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace VertexModify2D
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteSkin))]
    public class VertexExposer : MonoBehaviour
    {
        public SpriteSkin Target;
        public SpriteRenderer Renderer;
        public bool DrawGizmos;

        private List<VertexData> _vertexData = new List<VertexData>();
        public IReadOnlyList<VertexData> VertexData => _vertexData;

        void Awake()
        {
            if (Target == null)
                Target = GetComponent<SpriteSkin>();
            if (Renderer == null)
                Renderer = GetComponent<SpriteRenderer>();

            if (Target.DeformedVerticesAvailable)
            {
                ref var vertices = ref Target.DeformedVertices;
                Target_OnSkinChanging(ref vertices);
                Target.FinishDeformation(ref vertices);
            }
        }

        private void OnEnable()
        {
            Target.OnSkinChanging += Target_OnSkinChanging;
        }

        private void OnDisable()
        {
            Target.OnSkinChanging -= Target_OnSkinChanging;
        }

        private void LateUpdate()
        {
            if (Application.isPlaying) return;

            if (Target.DeformedVerticesAvailable)
            {
                ref var vertices = ref Target.DeformedVertices;
                Target_OnSkinChanging(ref vertices);
                Target.FinishDeformation(ref vertices);
            }
        }

        private void Target_OnSkinChanging(ref NativeArray<byte> data)
        {
            if (_vertexData.Count != data.Length / 28)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (i % 28 == 0)
                    {
                        _vertexData.Add(new VertexData(i, Target.rootBone, Renderer, ref data));
                    }
                }
            }

            foreach (var vertex in _vertexData)
            {
                vertex.Override(ref data);
            }
        }

        private void OnDrawGizmos()
        {
            if (!DrawGizmos)
                return;

            foreach (var vertex in _vertexData)
            {
                vertex.DrawGizmos();
            }
        }

        public List<VertexData> GetVerticesInRange(Vector3 worldPosition, float distance)
        {
            return _vertexData.Where(x => Vector3.Distance(x.WorldPosition, worldPosition) <= distance).ToList();
        }
    }
}