using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityEffectType { Immediate, Continuous, Constant };

public abstract class AbilityEffect : ScriptableObject // MonoBehaviour
{
    public AbilityEffectType AbilityEffectType { get; set; }
    public abstract void Apply(AbilityOwner abilityOwner);
    public virtual void Disable(AbilityOwner abilityOwner) { }

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
