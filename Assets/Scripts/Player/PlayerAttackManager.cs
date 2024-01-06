using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public Transform attackPoint;

    public void ShootProjectile(GameObject projectilePrefab)
    {
        // create projectile object
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left, flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0) {
            projectile.GetComponent<BasicProjectile>().FlipDirection();
        }
    }

    public void ShootProjectileBurst(GameObject projectilePrefab, int numProjectiles, float delay)
    {
        StartCoroutine(projectileBurst(projectilePrefab, numProjectiles, delay));
    }

    IEnumerator projectileBurst(GameObject projectilePrefab, int numProjectiles, float delay)
    {
        Debug.Log("Firing Projectile Burst");
        for (int i = 0; i < numProjectiles; i++)
        {
            Debug.Log("Firing Projectile #" + i);
            ShootProjectile(projectilePrefab);
            yield return new WaitForSeconds(delay);
        }
    }
}
