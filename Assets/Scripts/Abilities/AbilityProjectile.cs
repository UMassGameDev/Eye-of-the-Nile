/**************************************************
--- DEPRECATED ---
Script for a basic projectile object created from an ability.
I recommend using a different projectile script that inherits from BasicProjectile

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    Rigidbody2D projRb;
    public Sprite projSprite;
    public Transform projParticlePrefab;
    private float initialTime;
    public float lifetime = 999f;
    public float initialForce;
    public int damage = 0;
    public LayerMask collisionLayers;

    public virtual void Impact(Collider2D other)
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & collisionLayers.value) > 0)
        {
            Impact(other);
            Destroy(gameObject);
        }
    }

    void Awake()
    {
        projRb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        initialTime = Time.time;
        projRb.AddForce(transform.up * initialForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > initialTime + lifetime)
            Destroy(gameObject);
    }
}
