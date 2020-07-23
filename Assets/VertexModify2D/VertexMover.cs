using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertexModify2D
{
    [ExecuteInEditMode]
    public class VertexMover : MonoBehaviour
    {

        public List<VertexExposer> VertexExposers;
        public float EffectDistance = 1f;
        public Transform Target;
        public float Weight = 1f;
        public bool ShowWeight = true;

        private void Update()
        {
            if (Target == null) return;
            if (VertexExposers == null) return;

            var offset = Target.localPosition;

            foreach (var vertexExposer in VertexExposers)
            {
                foreach (var vertex in vertexExposer.VertexData)
                {
                    var initialDistance = Vector3.Distance(
                        vertex.InitialWorldPosition, Target.parent.position);
                    var strength = Mathf.Max(0f, 1f - initialDistance / EffectDistance);
                    strength *= Weight;
                    vertex.SetXY(offset.x * strength, offset.y * strength, GetHashCode());
                }
            }
        }

        private void OnDisable()
        {
            if (VertexExposers == null) return;

            foreach (var vertexExposer in VertexExposers)
            {
                foreach (var vertex in vertexExposer.VertexData)
                {
                    vertex.SetXY(0f, 0f, GetHashCode());
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Target == null) return;
            if (VertexExposers == null) return;

            Gizmos.DrawLine(transform.position, Target.position);
            Gizmos.DrawWireSphere(transform.position, EffectDistance);

            if (ShowWeight)
            {
                foreach (var vertexExposer in VertexExposers)
                {
                    foreach (var vertex in vertexExposer.VertexData)
                    {
                        var initialDistance = Vector3.Distance(
                            vertex.InitialWorldPosition, Target.parent.position);
                        var strength = 1f - initialDistance / EffectDistance;
                        var color = new Color(strength, strength, strength, strength);
                        Gizmos.DrawIcon(vertex.WorldPosition, "blendKey", false, color);
                    }
                }
            }
        }
    }
}