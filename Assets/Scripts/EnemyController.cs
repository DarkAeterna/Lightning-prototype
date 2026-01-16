
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float border = -6.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > border)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        
    }
}
