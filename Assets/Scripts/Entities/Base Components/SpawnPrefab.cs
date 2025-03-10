using UnityEngine;

/** \brief
Spawns a prefab. This function can be triggered by a Unity Event.
This was initially made to drop potions when pots break.

Documentation updated 3/9/2024
\author Stephen Nuttall, Alexander Art
*/
public class SpawnPrefab : MonoBehaviour
{
    /// Reference to the prefab we want to spawn.
    [SerializeField] Transform prefabToSpawn;
    /// True if the spawned prefab should spawn with the same scale as the object that runs this script.
    [SerializeField] bool inheritScale;
    /// True if the spawned prefab should spawn with the same rotation as the object that runs this script.
    [SerializeField] bool inheritRotation;

    /// Spawns the prefab.
    public void Spawn()
    {
        /// Spawns the prefab at the parent object's position.
        Transform SpawnedPrefab = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

        // Set the scale of the spawned object if inheritRotation is true.
        if (inheritRotation == true)
        {
            SpawnedPrefab.rotation = transform.rotation;
        }

        // Set the scale of the spawned object if inheritScale is true.
        if (inheritScale == true)
        {
            SpawnedPrefab.localScale = transform.localScale;
        }
    }
}
