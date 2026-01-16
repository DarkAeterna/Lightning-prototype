using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    [SerializeField] private float speed = 5.0f;
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        float inputVertical = Input.GetAxis("Vertical");
        float inputHorizontal = Input.GetAxis("Horizontal");
        playerRb.AddForce(Vector2.up * speed * inputVertical, ForceMode2D.Impulse);
        playerRb.AddForce(Vector2.right * speed * inputHorizontal, ForceMode2D.Impulse);
    }
}
