using UnityEngine;

/** \brief
Script for a boulder that rolls until its hitbox tells it to break.
BoulderHitbox is responsible for detecting collisions and dealing damage.

Documentation updated 8/26/2024
\author Roy Pascual
*/
public class BoulderHazard : MonoBehaviour
{
    /// Reference to the rigidbody of the boulder.
    Rigidbody2D boulderBody;
    /// Particles the boulder will spawn when it breaks.
    public Transform boulderParticles;
    /// Force that is applied when the boulder is first spawned in.
    float initialForce = 100f;
    /// Amount of damage the boulder will apply if it hits an object on one of the collisionLayers.
    public int damage = 30;
    /// \brief Objects on these layers will be damaged by the boulder. Set in the Unity Editor.
    /// \note To set the layer an object is on, go to the top right of the inspector tab and click the "Layers" drop down menu.
    /// Check the ones you want applied to that object. Then, you can come back to the boulder prefab and add that layer to the collision layers.
    public LayerMask collisionLayers;

    /// \brief Triggered by the boulder hitbox when it hits an object on the collisionLayers, or the boulder is set to despawn.
    /// Gets the object's collider, instantiates boulder particles, and destroys the boulder.
    public void BreakBoulder()
    {
        Collider2D thisCollider = GetComponent<Collider2D>();
        Instantiate(boulderParticles,
            thisCollider.bounds.center,
            Quaternion.identity);
        Destroy(gameObject);
    }

    /// Set reference to rigidbody.
    void Awake()
    {
        boulderBody = GetComponent<Rigidbody2D>();
    }

    /// Apply initialForce to the boulder.
    void Start()
    {
        boulderBody.AddForce(transform.up * initialForce, ForceMode2D.Impulse);
    }
}
