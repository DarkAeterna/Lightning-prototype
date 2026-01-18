using System;
using UnityEngine;

namespace Logic
{
    public class InputSystem : MonoBehaviour
    {
        [Header("Buttons")] [SerializeField] private int _mouseButton = 0;

        public event Action<Vector3> MouseClicked;

        private void Update()
        {
            if (Input.GetMouseButton(_mouseButton))
            {
                Vector3 mousePosition = Input.mousePosition;
                MouseClicked?.Invoke(mousePosition);
            }
        }
    }
}