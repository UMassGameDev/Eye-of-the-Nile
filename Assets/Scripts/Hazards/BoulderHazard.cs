/**************************************************
Script for a boulder that rolls until its hitbox tells it to break.
BoulderHitbox is responsible for detecting collisions and dealing damage.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;
public class BoulderHazard : MonoBehaviour
{
    Rigidbody2D boulderBody;
    public Transform boulderParticles;
    float initialForce = 100f;
    public int damage = 30;
    public LayerMask collisionLayers;

    public void BreakBoulder()
    {
        Collider2D thisCollider = GetComponent<Collider2D>();
        Instantiate(boulderParticles,
            thisCollider.bounds.center,
            Quaternion.identity);
        Destroy(gameObject);
    }

    void Awake()
    {
        boulderBody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        boulderBody.AddForce(transform.up * initialForce, ForceMode2D.Impulse);
    }
}
