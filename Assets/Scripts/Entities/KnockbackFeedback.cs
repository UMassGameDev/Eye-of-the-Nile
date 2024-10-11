using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/** \brief
Put this script on an object to allow it to take knockback.

Documentation updated 10/10/2024
\author Stephen Nuttall
*/
public class KnockbackFeedback : MonoBehaviour
{
    /// Reference to the rigidbody component of the object.
    [SerializeField] Rigidbody2D rb;
    /// How long after the knockback starts the force on the force should stop being applied.
    [SerializeField] protected float kbDelay = 0.15f;
    /// The strength of the knockback (amount of force applied) if no strength is given.
    [SerializeField] protected float defaultStrength = 50;
    /// The amount of strength subtracted from any knockback applied to this object.
    [SerializeField] protected float kbResistance = 0;

    /// Event that is triggered when the knockback begins to be applied. Functions can be subscribed to this events in the Unity Editor.
    public UnityEvent OnBegin;
    /// Event that is triggered when the knockback begins to be applied. Functions can be subscribed to this events in the Unity Editor.
    public UnityEvent OnDone;

    /// <summary>
    /// Applies knockback to this object by adding force in the opposite direction of the sender.
    /// </summary>
    /// <param name="sender">The object causing the knockback to happen.</param>
    public void ApplyKnockback(GameObject sender)
    {
        ApplyKnockback(sender, defaultStrength);
    }

    /// <summary>
    /// Applies knockback to this object by adding force in the opposite direction of the sender.
    /// This overload allows one to specify the strength of the knockback.
    /// </summary>
    /// <param name="sender">The object causing the knockback to happen.</param>
    /// <param name="strength">The strength of the force applied to the object.</param>
    public void ApplyKnockback(GameObject sender, float strength)
    {
        if (strength - kbResistance > 0)
        {
            StopAllCoroutines();
            OnBegin.Invoke();
            Vector2 direction = (transform.position - sender.transform.position).normalized;
            rb.AddForce(direction * (strength - kbResistance), ForceMode2D.Impulse);
            StartCoroutine(Reset());
        }
    }

    /// <summary>
    /// Waits kbDelay seconds, then resets the object's velocity.
    /// </summary>
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(kbDelay);
        rb.velocity = Vector2.zero;
        OnDone.Invoke();
    }
}