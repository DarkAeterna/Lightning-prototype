
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float border = -6.0f;
    void Start()
    {
        _speed = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > border)
        {
            transform.Translate(Vector3.left * (_speed * Time.deltaTime));
        }
        
    }
}
