using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace VertexModify2D
{
    [Serializable]
    public class VertexData
    {
        public float ActualX { get; private set; }
        public float ActualY { get; private set; }

        private Dictionary<int, Vector2> _offsets = new Dictionary<int, Vector2>();

        private int _byteOffset;
        private Transform _root;
        public SpriteRenderer SpriteRenderer;
        public Transform RendererTransform;

        public Vector2 WorldPosition { get; private set; }
        public Vector2 InitialWorldPosition { get; private set; }
        public Vector2 InitialLocalPosition { get; private set; }
        public Vector2 ActualPosition => new Vector2(ActualX, ActualY);

        public VertexData(int offset, Transform root, SpriteRenderer spriteRenderer, ref NativeArray<byte> data)
        {
            _byteOffset = offset;
            _root = root;
            SpriteRenderer = spriteRenderer;
            RendererTransform = SpriteRenderer.transform;

            ActualX = GetX(ref data);
            ActualY = GetY(ref data);

            InitialLocalPosition = new Vector2(ActualX, ActualY);
            StoreWorldAndLocalPosition(ref data, true);
        }

        private float GetX(ref NativeArray<byte> data)
        {
            return BitConverter.ToSingle(new byte[4]
            {
            data[_byteOffset], data[_byteOffset + 1], data[_byteOffset + 2], data[_byteOffset + 3]
            }, 0);
        }

        private void SetX(ref NativeArray<byte> data, float value)
        {
            var bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < 4; i++)
            {
                data[i + _byteOffset] = bytes[i];
            }
        }

        private float GetY(ref NativeArray<byte> data)
        {
            return BitConverter.ToSingle(new byte[4]
            {
            data[_byteOffset + 4], data[_byteOffset + 5], data[_byteOffset + 6], data[_byteOffset + 7]
            }, 0);
        }

        private void SetY(ref NativeArray<byte> data, float value)
        {
            var bytes = BitConverter.GetBytes(value);
            for (int i = 4; i < 8; i++)
            {
                data[i + _byteOffset] = bytes[i - 4];
            }
        }

        private void StoreWorldAndLocalPosition(ref NativeArray<byte> data, bool setInitial)
        {
            var scaledX = ActualX * _root.lossyScale.x / _root.localScale.x;
            var scaledY = ActualY * _root.lossyScale.y / _root.localScale.y;
            WorldPosition = RendererTransform.position + new Vector3(scaledX, scaledY);

            if (setInitial)
                InitialWorldPosition = WorldPosition;
        }

        public void Override(ref NativeArray<byte> data)
        {
            float x = 0f, y = 0f;
            if (_offsets == null) _offsets = new Dictionary<int, Vector2>(); 
            foreach (var offset in _offsets)
            {
                x += offset.Value.x;
                y += offset.Value.y;
            }

            ActualX = GetX(ref data) + x;
            SetX(ref data, ActualX);

            ActualY = GetY(ref data) + y;
            SetY(ref data, ActualY);

            StoreWorldAndLocalPosition(ref data, false);
        }

        public void SetXY(float x, float y, int Identifier)
        {
            if(_offsets == null) _offsets = new Dictionary<int, Vector2>(); 

            if (!_offsets.ContainsKey(Identifier))
            {
                _offsets.Add(Identifier, new Vector2(x, y));
            }
            else
            {
                var vector = _offsets[Identifier];
                vector.x = x;
                vector.y = y;
                _offsets[Identifier] = vector;
            }
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(WorldPosition, 0.025f);
        }
    }
}