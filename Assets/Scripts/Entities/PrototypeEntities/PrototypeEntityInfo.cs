using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PrototypeEntityInfo", menuName = "NPCs/Create New PrototypeEntityInfo")]
public class PrototypeEntityInfo : ScriptableObject
{
    [Header("Appearance Info")]
    public float xScale = 1f;

    [Header("Attack Info")]
    public int attackDamage = 30;
    public float attackCooldown = 0.8f;
    public float attackSize = 0.75f;

    /*[Header("State Info")]
    public LayerMask enemyLayers;  // This LayerMask includes the Player's layer so the enemy is alerted
    public PatrolZone patrolZone;*/

    [Header("Targeting Info")]
    public LayerMask enemyLayers;  // This LayerMask includes the Player's layer so the enemy is alerted
    public float detectionRange = 6f;
    public float activateAttackRange = 3f;  // range which entity will activate attack
    [SerializeField]
    public List<float> attackRanges;
    // [SerializeField]
    // public List<AttackDeadzone> attackDeadZones;

    [Header("Physics Info")]
    public float moveVelocity = 6.0f;
    public float linearDrag = 1.0f;
    public float groundedRaycastLength = 1.8f;

    [Header("Conditions")]
    [SerializeField]
    public List<string> baseConditions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*[Serializable]
public class AttackDeadzone
{
    public float lowerBound = 0f;
    public float upperBound = 0f;
}*/