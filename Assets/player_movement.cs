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
    [Header("�ƶ�����")]
    public float maxSpeed = 7f;
    public float jumpTakeOffSpeed = 10f;
    public float gravityScale = 3f; // �������ţ����������ٶȣ�

    [Header("������")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("��Ч")]
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

        // ��ʼ����������
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
        // ������Ծ - ֻ����Ծ״̬���ڵ���ʱ���������ٶ�
        if (jumpState == JumpState.Jumping && isGrounded)
        {
            // ����ˮƽ�ٶȣ�ֻ�޸Ĵ�ֱ�ٶ�
            rb.velocity = new Vector2(rb.velocity.x, jumpTakeOffSpeed);
            jumpState = JumpState.InFlight; // �����л�������״̬����ֹ�ظ���Ծ
        }
        else if (stopJump)
        {
            stopJump = false;
            if (rb.velocity.y > 0)
            {
                // �ɿ���Ծ����������С�����ٶ�
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        // ����ˮƽ�ƶ� - ֻ�޸�ˮƽ�ٶȣ���Ӱ�촹ֱ�ٶ�
        float targetXVelocity = move.x * maxSpeed;
        rb.velocity = new Vector2(targetXVelocity, rb.velocity.y);

        // ��ɫ��ת
        if (move.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (move.x < -0.01f)
            spriteRenderer.flipX = true;

        // ��������
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
