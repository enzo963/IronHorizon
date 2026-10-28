using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    [Header("Map")]
    public SpriteRenderer mapRenderer;

    [Header("Hex Grid")]
    [Min(0.05f)]
    public float hexSize = 0.5f;

    public bool showGrid = true;

    public Color gridColor = new Color(0f, 1f, 0.7f, 0.7f);

    void OnDrawGizmos()
    {
        if (!showGrid)
            return;

        if (mapRenderer == null)
            return;

        DrawGrid();
    }

    void DrawGrid()
    {
        Bounds mapBounds = mapRenderer.bounds;

        float hexWidth = hexSize * 2f;
        float hexHeight = Mathf.Sqrt(3f) * hexSize;

        float horizontalSpacing = hexSize * 1.5f;
        float verticalSpacing = hexHeight;

        float startX = mapBounds.min.x + hexSize;
        float startY = mapBounds.min.y + hexHeight / 2f;

        int column = 0;

        for (float x = startX;
             x + hexSize <= mapBounds.max.x;
             x += horizontalSpacing)
        {
            float yOffset =
                (column % 2 == 0)
                ? 0f
                : hexHeight / 2f;

            for (float y = startY + yOffset;
                 y + hexHeight / 2f <= mapBounds.max.y;
                 y += verticalSpacing)
            {
                Vector3 center = new Vector3(
                    x,
                    y,
                    mapRenderer.transform.position.z - 0.1f
                );

                DrawHex(center);
            }

            column++;
        }
    }

    void DrawHex(Vector3 center)
    {
        Gizmos.color = gridColor;

        Vector3[] corners = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            float angle = 60f * i;
            float angleRad = angle * Mathf.Deg2Rad;

            corners[i] = new Vector3(
                center.x + hexSize * Mathf.Cos(angleRad),
                center.y + hexSize * Mathf.Sin(angleRad),
                center.z
            );
        }

        for (int i = 0; i < 6; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[(i + 1) % 6];

            Gizmos.DrawLine(start, end);
        }
    }
}