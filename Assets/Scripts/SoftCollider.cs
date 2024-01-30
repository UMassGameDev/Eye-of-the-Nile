/**************************************************
Used in the soft collider prefab, which is put as a child onto other objects.
Allows for other objects to pass through the this object, but push them out if they stay inside this object.
For example, a player can pass through a pot or an enemy, but will be pushed away if they try to stand inside it.
Think of how animals in Minecraft push each other around when there's too many in a small area.

Documentation updated 1/29/2024
**************************************************/
using System.Collections.Generic;
using UnityEngine;

public class SoftCollider : MonoBehaviour
{
    public Rigidbody2D ParentBody { get; set; }
    List<SoftCollider> opposingColliders;
    float pushMagnitude = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Push"))
        {
            SoftCollider opposingCollider = collision.GetComponent<SoftCollider>();
            if (!opposingColliders.Contains(opposingCollider))
                opposingColliders.Add(opposingCollider);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (opposingColliders.Count > 0)
        {
            foreach (SoftCollider opposingCollider in opposingColliders)
            {
                Vector2 difference = opposingCollider.transform.position - transform.position;
                float pushDirection = 0f;
                if (difference.x > 0f)
                {
                    pushDirection = 1f;
                }
                else
                {
                    pushDirection = -1f;
                }
                float absXDiff = Mathf.Abs(difference.x);

                // Push relative to distance between transforms
                float pushDistRatio = (100f + absXDiff) / (100f + 800f * Mathf.Pow(absXDiff, 3));
                // Push relative to current horizontal velocity
                float pushFactor = (30f + Mathf.Abs(opposingCollider.ParentBody.velocity.x))
                    / (30f + 10f * Mathf.Pow(Mathf.Abs(opposingCollider.ParentBody.velocity.x),2));
                opposingCollider.ParentBody.velocity =
                    new Vector2(opposingCollider.ParentBody.velocity.x + (pushDirection * pushMagnitude * pushDistRatio * pushFactor),
                    opposingCollider.ParentBody.velocity.y);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Push"))
        {
            SoftCollider opposingCollider = collision.GetComponent<SoftCollider>();
            if (opposingColliders.Contains(opposingCollider))
                opposingColliders.Remove(opposingCollider);
        }
    }

    void Awake()
    {
        ParentBody = transform.parent.GetComponent<Rigidbody2D>();
        opposingColliders = new List<SoftCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
