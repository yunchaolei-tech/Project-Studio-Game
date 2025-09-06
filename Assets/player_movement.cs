using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpState
{
    Grounded,
    PrepareToJump,
    Jumping,
    InFlight,
    Landed
}

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float maxSpeed = 7f;
    public float jumpTakeOffSpeed = 10f;
    public float gravityScale = 3f; // 重力缩放（控制下落速度）

    [Header("地面检测")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("音效")]
    public AudioClip jumpAudio;
    public AudioClip ouchAudio;

    private Rigidbody2D rb;
    private Collider2D collider2d;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public JumpState jumpState = JumpState.Grounded;
    private bool stopJump;
    private bool controlEnabled = true;
    private Vector2 move;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 初始化重力设置
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
        }
    }

    void Update()
    {
        if (controlEnabled)
        {
            move.x = Input.GetAxis("Horizontal");

            if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                jumpState = JumpState.PrepareToJump;
            else if (Input.GetButtonUp("Jump"))
            {
                stopJump = true;
            }
        }
        else
        {
            move.x = 0;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        UpdateJumpState();
    }

    void FixedUpdate()
    {
        ComputeVelocity();
    }

    void UpdateJumpState()
    {
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                break;
            case JumpState.Jumping:
                if (!isGrounded)
                {
                    jumpState = JumpState.InFlight;
                    if (jumpAudio != null)
                        audioSource.PlayOneShot(jumpAudio);
                }
                break;
            case JumpState.InFlight:
                if (isGrounded)
                {
                    jumpState = JumpState.Landed;
                }
                break;
            case JumpState.Landed:
                jumpState = JumpState.Grounded;
                break;
        }
    }

    void ComputeVelocity()
    {
        // 处理跳跃 - 只在跳跃状态且在地面时设置向上速度
        if (jumpState == JumpState.Jumping && isGrounded)
        {
            // 保留水平速度，只修改垂直速度
            rb.velocity = new Vector2(rb.velocity.x, jumpTakeOffSpeed);
            jumpState = JumpState.InFlight; // 立即切换到飞行状态，防止重复跳跃
        }
        else if (stopJump)
        {
            stopJump = false;
            if (rb.velocity.y > 0)
            {
                // 松开跳跃键后立即减小上升速度
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        // 处理水平移动 - 只修改水平速度，不影响垂直速度
        float targetXVelocity = move.x * maxSpeed;
        rb.velocity = new Vector2(targetXVelocity, rb.velocity.y);

        // 角色翻转
        if (move.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (move.x < -0.01f)
            spriteRenderer.flipX = true;

        // 动画参数
        if (animator != null)
        {
            animator.SetBool("grounded", isGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(rb.velocity.x) / maxSpeed);
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
