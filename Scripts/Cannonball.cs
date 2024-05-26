using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    Vector2 dir;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f) Destroy(gameObject);
    }

    public void Move(Transform target)
    {
        this.target = target;
        dir = (target.position - transform.position).normalized;
    }
}
