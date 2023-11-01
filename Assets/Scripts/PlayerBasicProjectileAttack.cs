using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicProjectileAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float xOffset = 1.5f;
    public float yOffset = 0.5f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Instantiate(projectilePrefab, new Vector2(transform.position.x + xOffset, transform.position.y + yOffset), Quaternion.identity);
        }
    }
}
