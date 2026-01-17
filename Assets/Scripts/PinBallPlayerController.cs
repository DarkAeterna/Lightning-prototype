using UnityEngine;

public class PinBallPlayerController : MonoBehaviour
{
    Rigidbody2D playerRb;
    [SerializeField] float speed = 10.0f;
    public Camera cam;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            Vector2 direction = (mouseWorld - transform.position).normalized;
            playerRb.AddForce(direction * speed, ForceMode2D.Impulse);
        }
    }
}
