using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public bool canMove = true;

    private Rigidbody2D rb;
    private Vector2 input;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        rb.gravityScale = 0;               
        rb.freezeRotation = true;         
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
    }

    void Update()
    {
        if (!canMove)
        {
            input = Vector2.zero;
            ResetDirectionBools();
            animator.SetBool("IsMoving", false);
            return;
        }

       
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        
        if (input.x != 0)
            input.y = 0;

        
        if (input.x > 0)
            spriteRenderer.flipX = false;
        else if (input.x < 0)
            spriteRenderer.flipX = true;

        UpdateAnimationBools();
    }

    void FixedUpdate()
    {
        
        rb.velocity = input.normalized * moveSpeed;
    }

    void UpdateAnimationBools()
    {
        ResetDirectionBools();

        if (input == Vector2.zero)
        {
            animator.SetBool("IsMoving", false);
            return;
        }
        else
        {
            animator.SetBool("IsMoving", true);
        }

        if (input.y > 0.3f)
            animator.SetBool("MovingUp", true);
        else if (input.y < -0.3f)
            animator.SetBool("MovingDown", true);
        else if (input.x > 0.3f)
            animator.SetBool("MovingRight", true);
        else if (input.x < -0.3f)
            animator.SetBool("MovingLeft", true);
    }

    void ResetDirectionBools()
    {
        animator.SetBool("MovingUp", false);
        animator.SetBool("MovingDown", false);
        animator.SetBool("MovingLeft", false);
        animator.SetBool("MovingRight", false);
    }
}