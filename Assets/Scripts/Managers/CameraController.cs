using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Mouse Drag")]
    public float dragSpeed = 1f;

    [Header("Zoom")]
    public float zoomSpeed = 0.5f;
    public float minZoom = 2.5f;
    public float maxZoom = 5f;

    [Header("Map")]
    public SpriteRenderer mapRenderer;

    private Camera cam;

    private Vector2 lastMousePosition;
    private bool isDragging;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        ClampZoomToMap();
        ClampCamera();
    }

    void Update()
    {
        MoveWithKeyboard();
        MoveWithMouse();
        ZoomCamera();

        ClampZoomToMap();
        ClampCamera();
    }

    // -------------------------
    // WASD Movement
    // -------------------------

    void MoveWithKeyboard()
    {
        if (Keyboard.current == null)
            return;

        Vector2 direction = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            direction.y += 1;

        if (Keyboard.current.sKey.isPressed)
            direction.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            direction.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            direction.x += 1;

        direction = direction.normalized;

        transform.position +=
            new Vector3(direction.x, direction.y, 0)
            * moveSpeed
            * Time.deltaTime;
    }

    // -------------------------
    // Mouse Drag
    // -------------------------

    void MoveWithMouse()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Left Mouse Button
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            lastMousePosition = mousePosition;
            isDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (!isDragging)
            return;

        Vector2 delta = mousePosition - lastMousePosition;

        float worldHeight = cam.orthographicSize * 2f;
        float worldPerPixel = worldHeight / Screen.height;

        Vector3 movement = new Vector3(
            -delta.x * worldPerPixel,
            -delta.y * worldPerPixel,
            0
        );

        transform.position += movement * dragSpeed;

        lastMousePosition = mousePosition;
    }

    // -------------------------
    // Mouse Wheel Zoom
    // -------------------------

    void ZoomCamera()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0)
        {
            cam.orthographicSize -= zoomSpeed;
        }

        if (scroll < 0)
        {
            cam.orthographicSize += zoomSpeed;
        }

        cam.orthographicSize =
            Mathf.Clamp(
                cam.orthographicSize,
                minZoom,
                maxZoom
            );
    }

    // -------------------------
    // Prevent Zoom Outside Map
    // -------------------------

    void ClampZoomToMap()
    {
        if (mapRenderer == null)
            return;

        Bounds bounds = mapRenderer.bounds;

        float maxZoomHeight = bounds.extents.y;

        float maxZoomWidth =
            bounds.extents.x / cam.aspect;

        float mapMaxZoom =
            Mathf.Min(maxZoomHeight, maxZoomWidth);

        float realMaxZoom =
            Mathf.Min(maxZoom, mapMaxZoom);

        cam.orthographicSize =
            Mathf.Clamp(
                cam.orthographicSize,
                minZoom,
                realMaxZoom
            );
    }

    // -------------------------
    // Keep Camera Inside Map
    // -------------------------

    void ClampCamera()
    {
        if (mapRenderer == null)
            return;

        Bounds bounds = mapRenderer.bounds;

        float cameraHeight = cam.orthographicSize;
        float cameraWidth =
            cam.orthographicSize * cam.aspect;

        float minX =
            bounds.min.x + cameraWidth;

        float maxX =
            bounds.max.x - cameraWidth;

        float minY =
            bounds.min.y + cameraHeight;

        float maxY =
            bounds.max.y - cameraHeight;

        Vector3 position = transform.position;

        if (minX < maxX)
        {
            position.x =
                Mathf.Clamp(position.x, minX, maxX);
        }
        else
        {
            position.x = bounds.center.x;
        }

        if (minY < maxY)
        {
            position.y =
                Mathf.Clamp(position.y, minY, maxY);
        }
        else
        {
            position.y = bounds.center.y;
        }

        transform.position = position;
    }
}