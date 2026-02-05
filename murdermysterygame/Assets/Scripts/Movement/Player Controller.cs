using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public bool canMove = true; 

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lastMoveDir = Vector2.down;

    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            input = Vector2.zero;
            return;
        }

       
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        
        if (input.x != 0)
            input.y = 0;

        if (input != Vector2.zero)
            lastMoveDir = input.normalized;
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = input.normalized * moveSpeed;
    }

    public Vector2 GetFacingDirection()
    {
        return lastMoveDir;
    }
}