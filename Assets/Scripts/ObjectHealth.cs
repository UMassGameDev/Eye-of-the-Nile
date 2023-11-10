using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public Animator animator;
    public Transform hurtEffect;

    public bool IsDead { get { return currentHealth <= 0; } }

    public int maxHealth = 100;
    int currentHealth;
   
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(Transform attacker, int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        Collider2D objectCollider = transform.GetComponent<Collider2D>();

        Transform hurtPrefab = Instantiate(hurtEffect,
                objectCollider.bounds.center,
                Quaternion.identity);
        hurtPrefab.up = attacker.position - objectCollider.transform.position;

        if (currentHealth <= 0)
            Die();
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    void Die()
    {
        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        this.enabled = false;
    }

}
