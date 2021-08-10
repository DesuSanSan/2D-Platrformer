using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour 
{
    [SerializeField] float speed;
    public Animator animator;
    public int maxHealth = 100;
    int currentHealth;
    float timeToSpawn;
    float cooldownForSpawn;
    bool isDead = false;
    bool goingLeft = false;
    bool isAtPoint = false;
    [SerializeField] bool isWaiting = false;
    Rigidbody2D enemyRB;
    
    private void Start()
    {
        currentHealth = maxHealth;
        cooldownForSpawn = 3;
        enemyRB = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        if (isDead == false)
        {
            timeToSpawn = cooldownForSpawn;
            animator.SetBool("IsDead", false);
            Patrol();
            animator.SetFloat("Velocity", speed);
        }

        if (isDead == true)
        {
            WaitForSpawn();
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            animator.SetTrigger("Hurt");
        }



        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        enemyRB.constraints = RigidbodyConstraints2D.FreezePosition;
        animator.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        isDead = true;
    }

    void WaitForSpawn()
    {
        if(timeToSpawn > 0f)
        {
            timeToSpawn -= Time.deltaTime;
        }

        if(timeToSpawn < 1f)
        {
            animator.SetTrigger("Recovery");
            if (timeToSpawn < 0f)
            {
                enemyRB.constraints = RigidbodyConstraints2D.FreezeRotation;
                enemyRB.constraints = ~RigidbodyConstraints2D.FreezePosition;
                isDead = false;
                GetComponent<Collider2D>().enabled = true;
                currentHealth = maxHealth;
                animator.ResetTrigger("Recovery");
            }
        }        
    }
    void Patrol()
    {
        if (goingLeft == false && isWaiting == false)
        {
            enemyRB.velocity = new Vector2(-speed * Time.fixedDeltaTime, enemyRB.velocity.y);
            if (transform.position.x <= 1.284f && isAtPoint == false)
            {
                isAtPoint = true;
                if (transform.position.x <= 1.284f && isAtPoint== true)
                {
                    isWaiting = true;
                    animator.SetBool("IsStanding", true);
                    enemyRB.velocity = new Vector2(0, enemyRB.velocity.y);
                    StartCoroutine(WaitInPoint());
                }
            }
        }

        if (goingLeft == true && isWaiting == false)
        {
            enemyRB.velocity = new Vector2(speed * Time.fixedDeltaTime, enemyRB.velocity.y);
            if (transform.position.x >= 4.25f && isAtPoint == false)
            {
                isAtPoint = true;
                if (transform.position.x >= 4.25f && isAtPoint == true) 
                {
                    isWaiting = true;
                    animator.SetBool("IsStanding", true);
                    enemyRB.velocity = new Vector2(0, enemyRB.velocity.y);
                    StartCoroutine(WaitInPoint());
                }
            }
        }
    }

    IEnumerator WaitInPoint()
    {
        yield return new WaitForSeconds(3);
        if (goingLeft == false)
        {
            goingLeft = true;
            transform.localScale = new Vector3(-0.6337878f, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            goingLeft = false;
            transform.localScale = new Vector3(0.6337878f, transform.localScale.y, transform.localScale.z);
        }
        isWaiting = false;
        isAtPoint = false;
        animator.SetBool("IsStanding", false);
    }
}
