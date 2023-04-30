using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleEnemy : EnemyScript
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {

    }
}
