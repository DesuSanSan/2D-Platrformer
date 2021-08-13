using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditAgro : MonoBehaviour
{
    public bool isChasing = false;

    Vector3 offset = new Vector3(0, 0.4f, 0);
    
    public Bandit bandit;
    public GameObject banditGO;

    // Update is called once per frame
    void Update()
    {
        transform.position = banditGO.transform.position + offset;

        if (isChasing == true && bandit.isAttacking != true)
            bandit.ChasePlayer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == ("Player"))
            isChasing = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == ("Player"))
            isChasing = false;
    }
}
