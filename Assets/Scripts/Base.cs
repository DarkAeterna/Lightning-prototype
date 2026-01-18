using UnityEngine;


[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Base : MonoBehaviour
{
    [SerializeField] private float _baseHp = 100.0f;
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            TakeDamage(10f);
            Destroy(enemy.gameObject);
            Debug.Log($"Enemy destroyed! Base HP: {_baseHp}");
        }
    }
   
    private void TakeDamage(float damage)
    {
        _baseHp -= damage;
        if (_baseHp <= 0)
        {
            Debug.Log("Base destroyed!");
            Destroy(gameObject);
        }
    }
}