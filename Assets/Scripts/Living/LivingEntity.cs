using UnityEngine;
using System.Collections;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;

    protected float health;
    protected bool isAlive;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        isAlive = true;
        health = startingHealth;
    }

    public virtual void Takehit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    [ContextMenu("Self Destruct")]
    protected virtual void Die()
    {
        isAlive = false;
        if(OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (isAlive && health <= 0)
        {
            Die();
        }
    }
}
