using UnityEngine;

/*!<summary>
Script for a basic projectile object created from an ability.
</summary>
\deprecated This script obsolete does not work properly. Use a script that inherits from BasicProjectile instead.*/
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
        if (other.gameObject.tag == "DamageableByProjectile")
        {
            other.GetComponent<ObjectHealth>().TakeDamage(transform, damage);
        }
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
        if (projSprite != null)
            GetComponent<SpriteRenderer>().sprite = projSprite;
    }

    void Start()
    {
        initialTime = Time.time;
        projRb.AddForce(transform.up * initialForce, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (Time.time > initialTime + lifetime)
            Destroy(gameObject);
    }
}
