using Interfaces;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    private readonly float _maxValue = 100f;
    private float _value;

    private void Awake()
    {
        _value = _maxValue;
    }

    public bool TryTakeDamage(float damage)
    {
        if (damage <= 0f)
        {
            return false;
        }
        
        float newValue = _value - damage;

        if (newValue < 0f)
        {
            Destroy(gameObject);
        }
        else
        {
            _value = newValue;
        }
        
        return true;
    }
}
