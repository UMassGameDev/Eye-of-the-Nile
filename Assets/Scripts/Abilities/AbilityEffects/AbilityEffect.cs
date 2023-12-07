using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityEffect : ScriptableObject // MonoBehaviour
{
    public enum AbilityEffectType { Immediate, Continuous, Constant };

    public AbilityEffectType abilityEffectType;
    public abstract void Apply(AbilityOwner abilityOwner);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(abilityInfo.baseCost);
            abilityInfo.baseCost += 1;
        }*/

    }
}
