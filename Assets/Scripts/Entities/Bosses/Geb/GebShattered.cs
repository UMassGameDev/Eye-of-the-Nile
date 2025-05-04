using UnityEngine;
using System;
using System.Collections.Generic;

/** \brief
This script makes Geb's pieces go flying.

Documentation updated 5/3/2025
\author Alexander Art
*/
public class GebShattered : MonoBehaviour
{
    /// Random number generator, used for launching the fragmented pieces of Geb.
    System.Random rng = new System.Random();

    void Start()
    {
        // Launch each shard of Geb a random amount.
        foreach (Transform shard in transform)
        {
            Rigidbody2D shardRigidbody = shard.GetComponent<Rigidbody2D>();
            shardRigidbody.velocity = new Vector2(100f * ((float)rng.NextDouble() - 0.5f), 50f * (float)rng.NextDouble());
        }   
    }
}
