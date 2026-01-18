using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(Collider2D))]
    public class Building : MonoBehaviour
    {
        private int _perTouchChargePercent;
        private float _maxCharge;
        private float _charge;
        
        private bool _isTouched;

        private void Awake()
        {
            _perTouchChargePercent = 25;
            _maxCharge = 100f;
        }

        public void Touch()
        {
            _isTouched = true;

            float chargeAmount = _maxCharge / 100f * _perTouchChargePercent;
            _charge = Mathf.Min(_charge + chargeAmount, _maxCharge);

            Debug.Log($"Зарядил до {_charge}");
        }
    }
}
