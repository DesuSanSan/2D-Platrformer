using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour 
{
    [SerializeField] float speed;
    [SerializeField] float attackRange;
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] bool isAttacking = false;

    int countPlayers = 0;
    int currentHealth;
    int maxHealth = 100;
    float timeToSpawn;
    float cooldownForSpawn;
    bool isDead = false;
    bool goingRight = false;
    bool isAtPoint = false;
    bool isWaiting = false;

    Rigidbody2D enemyRB;

    public Animator animator;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public int damage = 25;
    

    private void Start()
    {
        currentHealth = maxHealth;
        cooldownForSpawn = 3;
        enemyRB = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        // Checking if bandit is alive
        if (isDead == false)
        {
            timeToSpawn = cooldownForSpawn;
            animator.SetBool("IsDead", false);

            if(isAttacking != true)
                Patrol();
            else
                enemyRB.velocity = new Vector2(0, enemyRB.velocity.y);

            animator.SetFloat("Velocity", speed);
        }

        // If bandit is dead then wait for respawn
        else
            WaitForSpawn();
    }

    // Script that determinets what bandit do after taking hit
    public void TakeDamage(int damage)
    {
        // Checking if bandit current health is higher than 0
        if (currentHealth > 0)
        {
            // Subtract dmg taken from bandit current health
            currentHealth -= damage;
            // Triggering hurt animation
            animator.SetTrigger("Hurt");
        }


        // Checking if bandit current health is same or less than 0
        if(currentHealth <= 0)
        {
            // If health is same or less than 0 then bandit die
            Die();
        }
    }

    // Freezing bandit position, set dead animation to true, disable bandit collider, set dead bool value to true
    void Die()
    {
        enemyRB.constraints = RigidbodyConstraints2D.FreezePosition;
        animator.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        isDead = true;
    }

    // Wait after death to respawn bandit and respawning him
    void WaitForSpawn()
    {
        if(timeToSpawn > 0)
        {
            // Waiting for time to respawn to be 0
            timeToSpawn -= Time.deltaTime;
        }

        if(timeToSpawn < 1)
        {
            // Start recovery animation
            animator.SetTrigger("Recovery");
            if (timeToSpawn < 0)
            {
                // Freezing bandit rotation, unfreezing bandito position (ability to move around), set dead value to false, enable bandit collieder, set current health to max health, reset animator trigger recovery
                enemyRB.constraints = RigidbodyConstraints2D.FreezeRotation;
                enemyRB.constraints = ~RigidbodyConstraints2D.FreezePosition;
                isDead = false;
                GetComponent<Collider2D>().enabled = true;
                currentHealth = maxHealth;
                animator.ResetTrigger("Recovery");
            }
        }        
    }

    // Patrol function (making bandit moving from one point to another one at certain location repetitively
    void Patrol()
    {
        // Checking if bandit is not going right and is not waiting
        if (goingRight == false && isWaiting == false)
        {
            // Seting x velocity to -speed so bandit will walk left
            enemyRB.velocity = new Vector2(-speed * Time.fixedDeltaTime, enemyRB.velocity.y);
            if (transform.position.x <= 1.284f && isAtPoint == false)
            {
                isAtPoint = true;
                // If bandit is on certain point
                if (transform.position.x <= 1.284f && isAtPoint== true)
                {
                    // Bandit must wait, with idel animation, his x velocity is set to 0 so he will not slid on ground and start waiting in patrol point
                    isWaiting = true;
                    animator.SetBool("IsStanding", true);
                    enemyRB.velocity = new Vector2(0, enemyRB.velocity.y);
                    StartCoroutine(WaitInPoint());
                }
            }
        }

        // Checking if bandit is going right and is not waiting
        if (goingRight == true && isWaiting == false)
        {
            // Setting x velocity to speed so bandit will walk right
            enemyRB.velocity = new Vector2(speed * Time.fixedDeltaTime, enemyRB.velocity.y);
            if (transform.position.x >= 4.25f && isAtPoint == false)
            {
                isAtPoint = true;
                // If bandit is on certain point
                if (transform.position.x >= 4.25f && isAtPoint == true) 
                {
                    // Bandit must wait, with idel animaton, his x velocity is set to 0 so he will not slid on ground and start waiting in patrol point
                    isWaiting = true;
                    animator.SetBool("IsStanding", true);
                    enemyRB.velocity = new Vector2(0, enemyRB.velocity.y);
                    StartCoroutine(WaitInPoint());
                }
            }
        }
    }

    // If bandit is in patrol point 
    IEnumerator WaitInPoint()
    {
        // He will be waiting for 3 seconds
        yield return new WaitForSeconds(3);
        // Checking if he walked right
        if (goingRight == false)
        {
            // If not he will be walking right
            goingRight = true;
            // Flip bandit
            transform.localScale = new Vector3(-0.6337878f, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // If true he will be walking left
            goingRight = false;
            // Flip bandit
            transform.localScale = new Vector3(0.6337878f, transform.localScale.y, transform.localScale.z);
        }
        // Enable bandit to walk
        isWaiting = false;
        // Bandit leave patrol point
        isAtPoint = false;
        // Bandit is no more standing still
        animator.SetBool("IsStanding", false);
    }

    // Drawing gizmos for attack for easily adjusments
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        if (other.CompareTag("Player"))
        {
            isAttacking = true;
            countPlayers++;
            animator.SetFloat("CountPlayers", countPlayers);
            animator.SetTrigger("IsAttacking");
            foreach (Collider2D player in hitPlayer)
            {
                 player.GetComponent<PlayerControll>().TakeDamagePlayer(damage);
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            countPlayers--;
            animator.SetFloat("CountPlayers", countPlayers);
            StartCoroutine(WaitForAttackAnimation());
        }
    }

    IEnumerator WaitForAttackAnimation()
    {
        yield return new WaitForSeconds(0.7f);
        animator.SetTrigger("IsAttacking");
        isAttacking = false;
    }
}
