using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Range(1, 100)] [SerializeField] private float movementSpeed = 1;
    [Range(1, 100)] [SerializeField] private float jumpForce = 1;

    [Range(1,3)][SerializeField] private float fallMultiplier = 1;

    private Rigidbody2D rb;

    private float xInput; // toma valor de 1 si me muevo a derecha, -1 a izq
    private bool jumpButton; // chequea si toqué el salto

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Time.timeScale = .5f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckXYAxis();
    }

    private void FixedUpdate()
    {
        HorizontalMovement();
        JumpMovement();
    }

    private void CheckXYAxis()
    {
        xInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump")  &&  Mathf.Abs(rb.velocity.y) < .001f ) // Si quiero saltar y estoy en el piso
        {
            jumpButton = true;
        }
    }

    private void HorizontalMovement()
    {
        transform.position += movementSpeed * Time.deltaTime * new Vector3 ( xInput , 0 , 0 );
    }

    private void JumpMovement()
    {
        if (jumpButton)
        {
            jumpButton = false;
            rb.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        if (rb.velocity.y < -0.001f)
        {
            rb.AddForce(fallMultiplier * jumpForce * Vector2.down , ForceMode2D.Force);
        }
    }
}
