using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class  CircleScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpForce=4;
    public float speed = 0f;

    // Becomes true once the speed has been chosen via the SpeedSlider
    private bool speedSet = false;
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ReloadScene();
        }    
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // rb.velocity = Vector2.zero;
        // rb.angularVelocity = 0f;
        // rb.simulated = false;


        // Freeze physics until the speed is chosen
        if (rb != null)
        {
            rb.simulated = false;
        }
    }

    // Called by SpeedSlider once the player locks in a speed
    public void StartMovement(float moveSpeed)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;  
            rb.simulated = true;
        }
        speed = moveSpeed;
        speedSet = true;

    }

    void FixedUpdate()
    {
        if (!speedSet || rb == null)
            return;

        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }

    void Update()
    {
        if (!speedSet || rb == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

}