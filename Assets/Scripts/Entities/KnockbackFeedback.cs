/**************************************************
Put this script on an object to allow it to take knockback..

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackFeedback : MonoBehaviour
{
    public Rigidbody2D rb;
    float kbDelay = 0.15f;
    public float defaultStrength = 50;

    public UnityEvent OnBegin, OnDone;

    public void ApplyKnockback(GameObject sender)
    {
        StopAllCoroutines();
        OnBegin.Invoke();
        Vector2 direction = (transform.position - sender.transform.position).normalized;
        rb.AddForce(direction * defaultStrength, ForceMode2D.Impulse);
        StartCoroutine(Reset());
    }

    public void ApplyKnockback(GameObject sender, float strength)
    {
        StopAllCoroutines();
        OnBegin.Invoke();
        Vector2 direction = (transform.position - sender.transform.position).normalized;
        rb.AddForce(direction * strength, ForceMode2D.Impulse);
        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(kbDelay);
        rb.velocity = Vector2.zero;
        OnDone.Invoke();
    }
}