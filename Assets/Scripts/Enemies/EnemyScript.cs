using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyScript : MonoBehaviour
{
    public bool givesUpwardVelocity;
    public bool damagesOnContact;
    public GameObject deathParticles;
    public GameObject hitParticles;
    public Collider2D Collider;
    public float health;

    protected PlayerScript player;

    public abstract void TakeDamage(float damage,Transform hitTransform);
    public abstract void Die();
}
