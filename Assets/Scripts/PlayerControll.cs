using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float waitTime;
    [SerializeField] float waitHeavy;
    [SerializeField] float timeToNextAttack;
    float horizontalInput;
    float cooldown;
    bool isOnGround = true;
    bool facingRight = true;
    int i = 0;

    public Animator animator;
    public Transform attackPointLight;
    public Transform attackPointHeavy;
    public float attackRangeLight = 0.5f;
    public float attackRangeHeavy = 0.5f;
    public LayerMask enemyLayers;
    public int damageLight = 20;
    public int damageHeavy = 50;

    Rigidbody2D p_rigidbody;


    private void Start()
    {
        p_rigidbody = GetComponent<Rigidbody2D>();
        cooldown = 0.5f;
        timeToNextAttack = cooldown;
    }
    void Update()
    {

        // Checking if we can move after move
        if (!animator.GetBool("IsAttackingHeavy"))
        {
            Move();
        }
        else
        {
            p_rigidbody.velocity = Vector2.zero;
        }

        // Timer that check if is attack is ready
        if (timeToNextAttack > 0)
        {
            timeToNextAttack -= Time.deltaTime;
        }

        // Attacking
        if (isOnGround && timeToNextAttack <= 0)
        {
            Attack();
        }
        animator.SetFloat("yVelocity", p_rigidbody.velocity.y);
    }

    void Move()
    {
        // Jumping code
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isOnGround && i <= 1)
        {
            i++;
            p_rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Changing jump animation to true
            animator.SetBool("IsJumping", true);

            // Checking how many jumps we did
            if (i == 1)
            {
                i = 0;
                animator.SetBool("IsJumping", true);
                isOnGround = false;
            }
        }
       
        // Running code
        horizontalInput = Input.GetAxis("Horizontal");
        p_rigidbody.velocity = new Vector2(speed * horizontalInput, p_rigidbody.velocity.y);

        // If we facing right and want go left
        if (facingRight && horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            facingRight = false;
        }
        // If we facing left and want go right
        else if (!facingRight && horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            facingRight = true;
        }

        // Running animation code
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        
    }

    // Detecting if we stand on ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;

            // Changing jump animation to false
            animator.SetBool("IsJumping", false);
        }
    }

    // Attack
    void Attack()
    {
        Collider2D[] hitEnemiesLight = Physics2D.OverlapCircleAll(attackPointLight.position, attackRangeLight, enemyLayers);
        Collider2D[] hitEnemiesHeavy = Physics2D.OverlapCircleAll(attackPointHeavy.position, attackRangeHeavy, enemyLayers);

        // Press J to light attack
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(WaitAfterAttack1(waitTime));
            timeToNextAttack = cooldown;
            foreach(Collider2D enemy in hitEnemiesLight)
            {
                enemy.GetComponent<Bandit>().TakeDamage(damageLight);
            }
        }

        // Press K to heavy attack
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(WaitAfterHeavyAttack(waitHeavy));
            timeToNextAttack = cooldown;
            foreach (Collider2D enemy in hitEnemiesHeavy)
            {
                enemy.GetComponent<Bandit>().TakeDamage(damageHeavy);
            }
        }
    }

    // Waiting for attack animation to end
    IEnumerator WaitAfterAttack1(float waitTime)
    {
            animator.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(waitTime);
            animator.SetBool("IsAttacking", false);
    }

    // Waiting for heavy attack animation to end
    IEnumerator WaitAfterHeavyAttack(float waitHeavy)
    {
        animator.SetBool("IsAttackingHeavy", true);
        yield return new WaitForSeconds(waitHeavy);
        animator.SetBool("IsAttackingHeavy", false);
    }

    // Drawing attack range in unity
    private void OnDrawGizmosSelected()
    {
        if (attackPointLight == null)
        {
            return;
        }
        if (attackPointHeavy== null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPointHeavy.position, attackRangeHeavy);
        Gizmos.DrawWireSphere(attackPointLight.position, attackRangeLight);
    }
}
