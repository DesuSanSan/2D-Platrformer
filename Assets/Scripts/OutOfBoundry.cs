using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundry : MonoBehaviour
{
    private int boundry = 19;
    void Update()
    {
        MapBoundry();   
    }

    void MapBoundry()
    {
        if(transform.position.x > 19)
        {
            transform.position = new Vector2(boundry, transform.position.y);
        }
        if (transform.position.x < -19)
        {
            transform.position = new Vector2(-boundry, transform.position.y);
        }
    }
}
