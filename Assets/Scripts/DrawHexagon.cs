using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHexagon : MonoBehaviour
{
    private const int NUM_VERTICES = 6;
    private const float INTERIOR_ANGLE = 2 * Mathf.PI / NUM_VERTICES;
    private LineRenderer lineRenderer;
    [SerializeField]
    private float lineStartWidth = 0.1f;
    [SerializeField]
    private float lineEndWidth = 0.1f;

    enum Axis
    {
        X,
        Y,
        Z
    }

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawHex(1f, Vector3.zero, lineStartWidth, lineEndWidth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawHex(float outerRadius, Vector3 centerCoord, float lineStartWidth, float lineEndWidth)
    {
        lineRenderer.startWidth = lineStartWidth;
        lineRenderer.endWidth = lineEndWidth;
        lineRenderer.loop = true;
        lineRenderer.positionCount = NUM_VERTICES;

        for (int i = 0; i < NUM_VERTICES; i++)
        {
            float vertexAngle = INTERIOR_ANGLE * i;
            Matrix4x4 rotationMatrix = GetAxialRotationMatrix(Axis.Y, vertexAngle);
            Vector3 initialRelativePosition = new Vector3(0f, 0f, outerRadius);
            lineRenderer.SetPosition(i, centerCoord + rotationMatrix.MultiplyPoint(initialRelativePosition));
        }
    }

    Matrix4x4 GetAxialRotationMatrix(Axis axis, float vertexAngle)
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
}
