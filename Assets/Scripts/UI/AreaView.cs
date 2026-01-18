using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class AreaView : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Collider2D _areaCollider;

    [Header("Line")]
    [SerializeField, Min(0.001f)]
    private float _lineWidth = 0.06f;

    [SerializeField] private Color _color = new Color(0.2f, 0.9f, 1.0f, 1.0f);

    [SerializeField] private string _sortingLayerName = "Default";
    [SerializeField] private int _sortingOrder = 300;

    [SerializeField] private bool _rebuildEveryFrame = false;

    private LineRenderer _lineRenderer;
    private Material _material;

    private void Awake()
    {
        if (_areaCollider == null)
        {
            _areaCollider = GetComponent<Collider2D>();
        }

        _material = CreateLineMaterial();

        _lineRenderer = CreateLineRenderer();
        Rebuild();
    }

    private void LateUpdate()
    {
        if (_rebuildEveryFrame)
        {
            Rebuild();
        }
    }

    public void Rebuild()
    {
        if (_areaCollider == null || _lineRenderer == null)
        {
            return;
        }

        if (_areaCollider is BoxCollider2D box)
        {
            DrawBox(box);
            return;
        }

        if (_areaCollider is PolygonCollider2D poly)
        {
            DrawPolygon(poly);
            return;
        }

        DrawBounds(_areaCollider.bounds);
    }

    private void DrawBox(BoxCollider2D box)
    {
        Transform boxTransform = box.transform;

        Vector2 half = box.size * 0.5f;

        Vector3 localBL = new Vector3(box.offset.x - half.x, box.offset.y - half.y, 0.0f);
        Vector3 localBR = new Vector3(box.offset.x + half.x, box.offset.y - half.y, 0.0f);
        Vector3 localTR = new Vector3(box.offset.x + half.x, box.offset.y + half.y, 0.0f);
        Vector3 localTL = new Vector3(box.offset.x - half.x, box.offset.y + half.y, 0.0f);

        Vector3 bl = boxTransform.TransformPoint(localBL);
        Vector3 br = boxTransform.TransformPoint(localBR);
        Vector3 tr = boxTransform.TransformPoint(localTR);
        Vector3 tl = boxTransform.TransformPoint(localTL);

        Vector3[] points = new Vector3[5];
        points[0] = bl;
        points[1] = br;
        points[2] = tr;
        points[3] = tl;
        points[4] = bl;

        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }

    private void DrawPolygon(PolygonCollider2D poly)
    {
        int pathIndex = 0;
        Vector2[] localPoints = poly.GetPath(pathIndex);

        if (localPoints == null || localPoints.Length < 2)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        Transform polyTransform = poly.transform;

        Vector3[] points = new Vector3[localPoints.Length + 1];
        for (int i = 0; i < localPoints.Length; i++)
        {
            Vector3 world = polyTransform.TransformPoint(localPoints[i]);
            points[i] = world;
        }

        points[points.Length - 1] = points[0];

        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }

    private void DrawBounds(Bounds b)
    {
        Vector3 bl = new Vector3(b.min.x, b.min.y, 0.0f);
        Vector3 br = new Vector3(b.max.x, b.min.y, 0.0f);
        Vector3 tr = new Vector3(b.max.x, b.max.y, 0.0f);
        Vector3 tl = new Vector3(b.min.x, b.max.y, 0.0f);

        Vector3[] points = new Vector3[5];
        points[0] = bl;
        points[1] = br;
        points[2] = tr;
        points[3] = tl;
        points[4] = bl;

        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }

    private LineRenderer CreateLineRenderer()
    {
        GameObject areaOutLine = new GameObject("AreaOutline");
        areaOutLine.transform.SetParent(transform, false);

        LineRenderer lineRenderer = areaOutLine.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = _material;

        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;

        lineRenderer.startColor = _color;
        lineRenderer.endColor = _color;

        lineRenderer.numCornerVertices = 2;
        lineRenderer.numCapVertices = 2;

        lineRenderer.sortingLayerName = _sortingLayerName;
        lineRenderer.sortingOrder = _sortingOrder;

        lineRenderer.positionCount = 0;

        return lineRenderer;
    }

    private Material CreateLineMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        return new Material(shader);
    }
}