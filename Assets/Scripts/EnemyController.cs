
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //TODO add Health script
    //TODO add Attacker script
    [SerializeField] private float _speed = 2.0f;
    private Vector3 basePosition;

    void Start()
    {
        GameObject baseObject = GameObject.FindWithTag("Base");
        if(baseObject == null)
        {
            Debug.LogError("Base with tag 'Base' not found!");
            enabled = false;
        }
        basePosition = baseObject.transform.position;
    }

    void Update()
    {
        Vector3 direction = (basePosition - transform.position).normalized;
        transform.Translate(direction * (_speed * Time.deltaTime));
    }

    //On Collisionenter(base)
        //Attack()
}