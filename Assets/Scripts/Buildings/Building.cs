using System.Collections;
using Interfaces;
using Logic;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Attacker))]
    public class Building : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _attackCooldown;
        
        private int _perTouchChargePercent;
        private float _maxCharge;
        private float _charge;
        
        private bool _isTouched;
        
        private Attacker _attacker;
        private WaitForSeconds _waitForSeconds;
        
        private IDamagable _target;

        private void Awake()
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0.0f;

            _perTouchChargePercent = 25;
            _maxCharge = 100f;
            
            _attacker = GetComponent<Attacker>();
            _waitForSeconds = new WaitForSeconds(_attackCooldown);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_target == null)
            {
                if (other.TryGetComponent(out IDamagable target))
                {
                    _target = target;
                    StartCoroutine(Attack());
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_target == null)
            {
                return;
            }

            if (other.TryGetComponent<IDamagable>(out IDamagable exitedTarget) == false)
            {
                return;
            }

            if (ReferenceEquals(_target, exitedTarget) == false)
            {
                return;
            }

            _target = null;
            StopCoroutine(Attack());
        }

        public void Touch()
        {
            _isTouched = true;

            float chargeAmount = _maxCharge / 100f * _perTouchChargePercent;
            _charge = Mathf.Min(_charge + chargeAmount, _maxCharge);

            Debug.Log($"Зарядил до {_charge}");
        }

        private IEnumerator Attack()
        {
            while (_target != null)
            {
                _attacker.Attack(_target);
                yield return _waitForSeconds;
            }
        }
    }
}
