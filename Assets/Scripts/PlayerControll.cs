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
    [SerializeField] float attackRangeLight;
    [SerializeField] float attackRangeHeavy;

    float horizontalInput;
    float cooldown;
    [SerializeField] float timeAfterHit = 1;
    bool isOnGround = true;
    bool facingRight = true;
    [SerializeField] bool isHurt = false;
    int i = 0;

    Rigidbody2D p_rigidbody;

    public Animator animator;
    public Transform attackPointLight;
    public Transform attackPointHeavy;
    public LayerMask enemyLayers;
    public int damageLight = 1;
    public int damageHeavy = 2;
    public int maxHealth = 12;
    public int currentHealth;

    private void Start()
    {
        p_rigidbody = GetComponent<Rigidbody2D>();
        cooldown = 0.5f;
        timeToNextAttack = cooldown;
        currentHealth = maxHealth;
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

        if (isHurt == true && timeAfterHit < 0)
        {
            isHurt = false;
        }

        if (timeAfterHit > 0)
        {
            timeAfterHit -= Time.deltaTime;
        }
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
        if (collision.gameObject.CompareTag("Enemy"))
        {

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
                Bandit bandit = enemy.GetComponent<Bandit>();
                if (bandit.GetComponent<BoxCollider2D>() != null)
                    bandit.TakeDamage(damageLight);
            }
        }

        // Press K to heavy attack
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(WaitAfterHeavyAttack(waitHeavy));
            timeToNextAttack = cooldown;
            foreach (Collider2D enemy in hitEnemiesHeavy)
            {
                Bandit temp = enemy.GetComponent<Bandit>();
                if( temp != null ) 
                    temp.TakeDamage(damageHeavy);
            }
        }
    }


    public void TakeDamagePlayer(int damageBandit)
    {
        // Checking if our current health is higher than 0
        if (currentHealth > 0 && isHurt == false)
        {
            // Subtract dmg taken from our current health
            currentHealth -= damageBandit;
            // Triggering hurt animation
            animator.SetTrigger("Hurt");
            isHurt = true;
            timeAfterHit = 1;
            StartCoroutine(WaitForHurtAnimation());
        }


        // Checking if our current health is same or less than 0
        if (currentHealth <= 0)
        {
            // If health is same or less than 0 then we die
            Die();
        }
    }

    void Die()
    {
        p_rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        animator.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
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
    IEnumerator WaitForHurtAnimation()
    {
        yield return new WaitForSeconds(0.15f);
        animator.SetBool("IsDead", false);
        animator.ResetTrigger("Hurt");

    }

    // Drawing attack range in unity
    private void OnDrawGizmosSelected()
    {
        // Checking if attack point exist
        if (attackPointLight == null)
        {
            return;
        }
        if (attackPointHeavy== null)
        {
            return;
        }

        // Drawing attack range in edytor
        Gizmos.DrawWireSphere(attackPointHeavy.position, attackRangeHeavy);
        Gizmos.DrawWireSphere(attackPointLight.position, attackRangeLight);
    }
}
