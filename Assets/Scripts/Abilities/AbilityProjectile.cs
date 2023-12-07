using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    Rigidbody2D projRb;
    public Sprite projSprite;
    public Transform projParticlePrefab;
    public float lifetime;
    public float initialForce;
    public int damage;
    public LayerMask collisionLayers;

    void Awake()
    {
        projRb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        projRb.AddForce(transform.up * initialForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
