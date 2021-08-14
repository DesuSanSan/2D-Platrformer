using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditAgro : MonoBehaviour
{

    Vector3 offset = new Vector3(0, 0.4f, 0);
    
    public float playerCords;
    public bool isChasing = false;
    public Bandit bandit;
    public GameObject banditGO;
    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        transform.position = banditGO.transform.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == ("Player"))
        {
            isChasing = true;
            bandit.isWaiting = false;
            bandit.isAtPoint = false;
            bandit.animator.SetBool("IsStanding", false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == ("Player"))
        {
            isChasing = false;
            playerCords = player.transform.position.x;
        }
    }
}
