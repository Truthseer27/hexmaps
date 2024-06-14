using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexagonGeometry
{
    public static class HexagonMetrics
    {
        public const int NUM_VERTICES = 6;
        public const float INTERIOR_ANGLE_DEG = 360f / NUM_VERTICES;

        public static float OuterRadius(float hexSize)
        {
            return hexSize;
        }

        public static float InnerRadius(float hexSize)
        {
            return hexSize * Mathf.Sqrt(3f) / 2;
        }

        public static Vector3[] GetVertices
        (
            float hexSize,
            float cellHeight = 0f,
            HexCellOrientation orientation = HexCellOrientation.Pointy,
            HexCellDepthAxis depthAxis = HexCellDepthAxis.Z
        )
        {
            Vector3[] vertices = new Vector3[NUM_VERTICES];
            for (int i = 0; i < NUM_VERTICES; i++)
            {
                vertices[i] = GetVertex(hexSize, i, cellHeight, orientation, depthAxis);
            }
            return vertices;
        }

        public static Vector3 GetVertex
        (
            float hexSize,
            int index,
            float cellHeight = 0f,
            HexCellOrientation orientation = HexCellOrientation.Pointy,
            HexCellDepthAxis depthAxis = HexCellDepthAxis.Z
        )
        {
            float angle = INTERIOR_ANGLE_DEG * index;
            if (orientation == HexCellOrientation.Pointy)
            {
                angle += 30f;
            }

            float xCoord = hexSize * Mathf.Cos(angle * Mathf.Deg2Rad);
            float yCoord = hexSize * Mathf.Sin(angle * Mathf.Deg2Rad);
            float zCoord = cellHeight;
            
            if (depthAxis == HexCellDepthAxis.Y)
            {
                // swap the x- and y-coordinates with tuples
                (yCoord, zCoord) = (zCoord, yCoord);
            }

            return new Vector3(xCoord, yCoord, zCoord);
        }

        public static Matrix4x4 GetAxialRotationMatrix(Axis axis, float vertexAngle)
        {
            Vector4 xVector, yVector, zVector, wVector;
            wVector = new Vector4(0f, 0f, 0f, 1f);

            if (axis == Axis.X)
            {
                xVector = new Vector4(1f, 0f, 0f, 0f);
                yVector = new Vector4(Mathf.Cos(vertexAngle), 0f, Mathf.Sin(vertexAngle), 0f);
                zVector = new Vector4(-1f * Mathf.Sin(vertexAngle), 0f, Mathf.Cos(vertexAngle), 0f);
            }
            else if (axis == Axis.Y)
            {
                xVector = new Vector4(Mathf.Cos(vertexAngle), 0f, -1f * Mathf.Sin(vertexAngle), 0f);
                yVector = new Vector4(0f, 1f, 0f, 0f);
                zVector = new Vector4(Mathf.Sin(vertexAngle), 0f, Mathf.Cos(vertexAngle), 0f);
            }
            else
            {
                xVector = new Vector4(Mathf.Cos(vertexAngle), 0f, Mathf.Sin(vertexAngle), 0f);
                yVector = new Vector4(-1f * Mathf.Sin(vertexAngle), 0f, Mathf.Cos(vertexAngle), 0f);
                zVector = new Vector4(0f, 0f, 1f, 0f);
            }
            return new Matrix4x4(xVector, yVector, zVector, wVector);
        }

        public enum Axis
        {
            X,
            Y,
            Z
        }
    }
}
