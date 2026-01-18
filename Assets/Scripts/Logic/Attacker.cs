using Interfaces;
using UnityEngine;

namespace Logic
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Attacker : MonoBehaviour, IAttackable
    {
        [SerializeField, Min(1)] private float _damage;
        [SerializeField, Min(1)] private float _damageRadius;

        private CircleCollider2D _damageCollider;
        
        private void Awake()
        {
            _damageCollider = GetComponent<CircleCollider2D>();
            _damageCollider.radius = _damageRadius;
            _damageCollider.isTrigger = true;
        }

        public void Attack(IDamagable target)
        {
            target.TryTakeDamage(_damage);
        }
    }
}
