using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour 
{
    public Animator animator;
    public int maxHealth = 100;
    int currentHealth;
    float timeToSpawn;
    float cooldownForSpawn;
    bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        cooldownForSpawn = 3;
    }

    private void Update()
    {
        if(currentHealth > 0)
        {
            animator.SetBool("IsDead", false);
        }

        if (isDead == false)
        {
            timeToSpawn = cooldownForSpawn;
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
                isDead = false;
                GetComponent<Collider2D>().enabled = true;
                currentHealth = maxHealth;
                animator.ResetTrigger("Recovery");
            }
        }        
    }
}
