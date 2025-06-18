using UnityEngine;

public static class DebugDrawHelper
{
    public static void DrawCube(Vector2 position, Vector2 size, Color color, float duration = 0f)
    {
        Vector2 halfSize = size * 0.5f;

        Vector2 topLeft = position + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = position + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomRight = position + new Vector2(halfSize.x, -halfSize.y);
        Vector2 bottomLeft = position + new Vector2(-halfSize.x, -halfSize.y);

        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
}
