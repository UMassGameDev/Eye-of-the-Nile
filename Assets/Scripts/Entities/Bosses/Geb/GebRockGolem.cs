using UnityEngine;
using System;

/** \brief
Additional script for the rock golems in Geb's bossroom.
What this script does:
- Decrements the rock golem count in Geb's bossroom when the rock golems die (function activated by OnDeath() in ObjectHealth).
- Prevents the rock golems from getting stuck in the outer walls of Geb's bossroom.
- Removes the rock golems when Geb is defeated.

Documentation updated 1/28/2025
\author Alexander Art
*/
public class GebRockGolem : MonoBehaviour
{
    /// Reference to the rock golem's health script.
    protected ObjectHealth objectHealth;
    /// Reference to Geb's phase controller.
    protected GebPhaseController gebPhaseController;
    /// Reference to Geb's room controller.
    protected GebRoomController gebRoomController;

    /// There is an additional range outside of the bounds of Geb's bossroom where the rock golems can spawn but the player can't go.
    [SerializeField] protected float maxOutOfBoundsRange = 25f;

    /// The minimum x position that the rock golems can have. Calculated using the golem's width and the bounds of the room.
    private float minPosX;
    /// The maximum x position that the rock golems can have. Calculated using the golem's width and the bounds of the room.
    private float maxPosX;

    void Awake()
    {
        objectHealth = GetComponent<ObjectHealth>();
        gebPhaseController = GameObject.Find("Geb").GetComponent<GebPhaseController>();
        gebRoomController = GameObject.Find("Geb").GetComponent<GebRoomController>();
    }

    /// Subscribes to the GebPhaseController.onGebDefeated event.
    void OnEnable()
    {
        GebPhaseController.onGebDefeated += Die;
    }

    /// Unsubscribes from the GebPhaseController.onGebDefeated event.
    void OnDisable()
    {
        GebPhaseController.onGebDefeated -= Die;
    }

    void Start()
    {
        // Get the width of the golem.
        float golemWidth = GetComponent<BoxCollider2D>().bounds.size.x;
        // The golems are allowed to go only 25 units out of bonuds.
        // Calculate the minimum x position for the golems, factoring in the width of the golem, plus an additional range.
        minPosX = gebRoomController.bounds.LeftPoint().x + golemWidth / 2f - maxOutOfBoundsRange;
        // Calculate the maximum x position for the golems, factoring in the width of the golem, plus an additional range.
        maxPosX = gebRoomController.bounds.RightPoint().x - golemWidth / 2f + maxOutOfBoundsRange;
    }

    /// Prevent the rock golems from getting stuck in the wall and get rid of the golems when Geb is defeated.
    void Update()
    {
        // If the golem is past the left boundary, move it right.
        // If the golem is past the right boundary, move it left.
        if (minPosX > transform.position.x)
        {
            transform.position = new Vector2(minPosX, transform.position.y);
        }
        else if (maxPosX < transform.position.x)
        {
            transform.position = new Vector2(maxPosX, transform.position.y);
        }
    }

    /// Activated by onGebDefeated event when Geb is defeated.
    public void Die()
    {
        objectHealth.TakeDamage(this.transform, objectHealth.currentHealth);
    }

    /// Subtracts 1 from Geb's bossroom's rock golem count. Called by the rock golem's ObjectHealth when it dies.
    public void DecrementRockGolemCount()
    {
        // Update rockGolemCount.
        gebRoomController.rockGolemCount--;
    }
}
