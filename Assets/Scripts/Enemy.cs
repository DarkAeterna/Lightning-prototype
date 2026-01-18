
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private Base _base;
    private Vector3 _basePosition;

    private void Start()
    {
        if(_base == null)
        {
            Debug.LogError("Base не назначен в Inspector!");
            enabled = false;
        }
        _basePosition = _base.transform.position;
    }

    private void Update()
    {
        Vector3 direction = (_basePosition - transform.position).normalized;
        transform.Translate(direction * (_speed * Time.deltaTime));
    }
}