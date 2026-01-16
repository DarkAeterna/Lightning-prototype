
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
