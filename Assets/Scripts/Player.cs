using Buildings;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
public class Player : MonoBehaviour
{
    [Header("Charging")]
    [SerializeField, Min(0.01f)]
    private float _maxDragDistance = 3.0f;

    [SerializeField, Min(0.0f)] private float _maxLaunchSpeed = 12.0f;
    [SerializeField, Min(0.0f)] private float _minLaunchSpeed = 0.0f;

    [Tooltip("Если включено - направление выстрела будет противоположно вектору к мыши (как в слингшоте).")]
    [SerializeField]
    private bool _invertDirection = false;

    [Header("Path prediction")]
    [SerializeField]
    private LayerMask _collisionMask = ~0;

    [SerializeField, Min(1)] private int _maxBounces = 32;
    [SerializeField, Min(2)] private int _maxPoints = 128;
    [SerializeField, Min(0.0001f)] private float _skin = 0.01f;

    [Tooltip("Сколько скорости теряем при ударе (приближение). 1 = не теряем, 0.9 = теряем 10% каждый отскок.")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float _bounceSpeedMultiplier = 0.95f;

    [Header("Line renderer")]
    [SerializeField, Min(0.01f)]
    private float _lineWidth = 0.06f;

    [Header("Stop settings")]
    [SerializeField, Min(0.05f)]
    private float _stopTimeSeconds = 3.0f;

    [SerializeField, Min(0.0f)] private float _sleepSpeedEpsilon = 0.03f;

    [Header("Bounce / friction (PhysicsMaterial2D)")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float _bounciness = 0.9f;

    [SerializeField, Min(0.0f)] private float _friction = 0.0f;

    [Header("Advanced")] [SerializeField] private bool _useImpulseLaunch = false;

    private LineRenderer _lineRenderer;
    private Camera _camera;

    private bool _isCharging;
    private Vector2 _aimDirection;
    private float _launchSpeed;

    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;

    private void Reset()
    {
        ApplyOrUpdatePhysicsMaterial2D();
        RecalculateDamping();
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();

        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;

        ApplyOrUpdatePhysicsMaterial2D();
        RecalculateDamping();

        _lineRenderer = GetComponent<LineRenderer>();
        _camera = Camera.main;

        _lineRenderer.enabled = false;
        _lineRenderer.positionCount = 0;
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;
        _lineRenderer.useWorldSpace = true;
    }

    private void FixedUpdate()
    {
        if (_rigidbody2D == null)
        {
            return;
        }

        float speed = _rigidbody2D.linearVelocity.magnitude;

        if (speed <= _sleepSpeedEpsilon)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0.0f;
            _rigidbody2D.Sleep();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Building building))
        {
            building.Touch();
        }
    }

    public void Launch(Vector2 worldDirection, float speed)
    {
        if (speed <= 0.0f)
        {
            return;
        }

        Vector2 direction = worldDirection;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        direction.Normalize();

        _rigidbody2D.WakeUp();

        if (_useImpulseLaunch)
        {
            float impulse = speed * _rigidbody2D.mass;
            _rigidbody2D.AddForce(direction * impulse, ForceMode2D.Impulse);
        }
        else
        {
            _rigidbody2D.linearVelocity = direction * speed;
        }
    }

    public float GetStopTimeSeconds()
    {
        return _stopTimeSeconds;
    }

    public float GetSleepSpeedEpsilon()
    {
        return _sleepSpeedEpsilon;
    }

    private void RecalculateDamping()
    {
        if (_rigidbody2D == null)
        {
            return;
        }

        float k = Mathf.Log(100.0f) / Mathf.Max(0.05f, _stopTimeSeconds);

        _rigidbody2D.linearDamping = k;
        _rigidbody2D.angularDamping = k;
    }

    private void ApplyOrUpdatePhysicsMaterial2D()
    {
        if (_collider2D == null)
        {
            return;
        }

        PhysicsMaterial2D material = _collider2D.sharedMaterial;
        if (material == null)
        {
            material = new PhysicsMaterial2D("BilliardBall2DMaterial");
            _collider2D.sharedMaterial = material;
        }

        material.bounciness = _bounciness;
        material.friction = _friction;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, transform.position + Vector3.right * 5.0f);
        }

        if (_camera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            StartCharging();
        }

        if (_isCharging)
        {
            UpdateChargingAndPreview();

            if (Input.GetMouseButtonUp(1))
            {
                ReleaseShot();
            }
        }
    }

    private void StartCharging()
    {
        _isCharging = true;
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 0;
    }

    private void UpdateChargingAndPreview()
    {
        Vector2 ballPosition = transform.position;
        Vector2 mouseWorld = _camera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 rawVector = mouseWorld - ballPosition;

        float distance = rawVector.magnitude;
        float clampedDistance = Mathf.Min(distance, _maxDragDistance);

        Vector2 direction = distance > 0.0001f ? rawVector / distance : Vector2.right;

        if (_invertDirection)
        {
            direction = -direction;
        }

        float t = _maxDragDistance <= 0.0001f ? 0.0f : clampedDistance / _maxDragDistance;
        float speed = Mathf.Lerp(_minLaunchSpeed, _maxLaunchSpeed, t);

        _aimDirection = direction;
        _launchSpeed = speed;

        BuildPredictedPath(ballPosition, _aimDirection, _launchSpeed);
    }

    private void ReleaseShot()
    {
        _isCharging = false;
        _lineRenderer.enabled = false;
        _lineRenderer.positionCount = 0;

        if (_launchSpeed <= 0.0001f)
        {
            return;
        }

        Launch(_aimDirection, _launchSpeed);
        _launchSpeed = 0.0f;
    }

    private void BuildPredictedPath(Vector2 startPosition, Vector2 startDirection, float startSpeed)
    {
        if (_maxPoints < 2)
        {
            return;
        }

        Vector2 position = startPosition;
        Vector2 direction = startDirection.normalized;
        float speed = startSpeed;

        float stopTimeSeconds = GetStopTimeSeconds();
        float epsilon = GetSleepSpeedEpsilon();

        float k = Mathf.Log(100.0f) / Mathf.Max(0.05f, stopTimeSeconds);

        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = _collisionMask;
        filter.useTriggers = false;

        _lineRenderer.positionCount = 0;
        int pointsCount = 0;

        AddPoint(position, ref pointsCount);

        int bounces = 0;

        while (pointsCount < _maxPoints)
        {
            if (speed <= epsilon)
            {
                break;
            }

            float remainingDistance = (speed - epsilon) / k;

            if (remainingDistance <= 0.0001f)
            {
                break;
            }

            RaycastHit2D hit = Physics2D.Raycast(position, direction, remainingDistance, _collisionMask);

            if (!hit)
            {
                Vector2 endPoint = position + direction * remainingDistance;
                AddPoint(endPoint, ref pointsCount);
                break;
            }

            float traveled = hit.distance;

            Vector2 hitPoint = hit.point;
            AddPoint(hitPoint, ref pointsCount);

            speed = Mathf.Max(epsilon, speed - k * traveled);

            speed *= _bounceSpeedMultiplier;

            direction = Vector2.Reflect(direction, hit.normal).normalized;

            position = hitPoint + direction * _skin;

            bounces++;

            if (bounces >= _maxBounces)
            {
                break;
            }
        }

        if (pointsCount < 2)
        {
            _lineRenderer.positionCount = 1;
            _lineRenderer.SetPosition(0, startPosition);
        }
    }

    private void AddPoint(Vector2 point, ref int pointsCount)
    {
        int newCount = pointsCount + 1;
        _lineRenderer.positionCount = newCount;
        _lineRenderer.SetPosition(pointsCount, point);
        pointsCount = newCount;
    }
}