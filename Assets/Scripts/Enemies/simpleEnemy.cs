using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleEnemy : EnemyScript
{
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (damagesOnContact)
        {
            if (Collider.IsTouching(player.col))
            {
                player.TakeDamage(1);
            }
        }
    }

    public override void TakeDamage(float damage, Transform hitTransform)
    {
        health -= damage;
        GameObject h = Instantiate(hitParticles,transform.position,hitTransform.rotation);
        Destroy(h, 1f);
        if (health <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        GameObject d = Instantiate(deathParticles, transform.position, transform.rotation);
        Destroy(d, 0.5f);
        Destroy(gameObject);
    }
}
