using UnityEngine;

/** \brief
Spawns a prefab. This function can be triggered by a Unity Event.
This was initially made to drop potions when pots break.

Documentation updated 3/5/2024
\author Stephen Nuttall
*/
public class SpawnPrefab : MonoBehaviour
{
    /// Reference to the prefab we want to spawn.
    [SerializeField] Transform prefabToSpawn;

    /// Spawns the prefab at the parent object's position.
    public void Spawn()
    {
        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
    }
}
