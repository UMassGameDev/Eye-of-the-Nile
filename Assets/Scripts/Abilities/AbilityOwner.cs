/**************************************************
Handles timed events related to abilities. Specifically, charge ups, activating the ability, repeated events during
duration time (such as healing over time), and cooldowns.
It also holds useful data, namely the OwnerTransform (player’s transform), the ability info this ability is using,
and the current ability state (ready to use vs on cooldown).

Note that this script is not a monobehavior, so it does not have many of the default unity functions like Start() and Update().

Documentation updated 8/11/2024
**************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AbilityOwner // : MonoBehaviour
{
    public enum OwnerState { ReadyToUse, Activation, OnCooldown };

    // These events trigger a function subscribed to them PlayerAbilityController.cs, which then triggers the corresponding function in this script.
    // This allows for these IEnumerator functions to trigger each other.
    // Pro-tip: Press F12 in Visual Studio Code to go to where a variable is defined, and press F12 again to see everywhere where it's used.

    // When triggered, a function subscribed to it in PlayerAbilityController.cs will run ChargingUp().
    public delegate void ChargeUpEvent(AbilityOwner abilityOwner);
    public event ChargeUpEvent ChargeUp;

    // When triggered, a function subscribed to it in PlayerAbilityController.cs will run CoolingDown().
    public delegate void CooldownEvent(AbilityOwner abilityOwner);
    public event CooldownEvent CoolDown;

    // When triggered, a function subscribed to it in PlayerAbilityController.cs will run UpdateWithinDuration().
    public delegate void UpdateEvent(AbilityOwner abilityOwner);
    public event UpdateEvent AbilityUpdate;

    public Transform OwnerTransform { get; set; }  // transform of the ability owner (the player's transform).
    public UnityEvent OnActivateAbility;  // generic event that is called when an ability is activated.
    public BaseAbilityInfo abilityInfo;  // ability info that this ability uses
    public OwnerState currentState = OwnerState.ReadyToUse;  // whether this ability is ready to use, activating, or on cooldown

    float updateEnd = 0f;  // Time until ability duration is up.

    // Constructor
    public AbilityOwner(Transform ownerTransform,
        UnityEvent onActivateAbility,
        BaseAbilityInfo newAbilityInfo)
    {
        OwnerTransform = ownerTransform;
        OnActivateAbility = onActivateAbility;
        abilityInfo = newAbilityInfo;
    }

    // Waits [abilityInfo.chargeUp] seconds, before triggering the ability, prepares for cooldown, and then triggers AbilityUpdate and CoolDown.
    public IEnumerator ChargingUp()
    {
        // Debug.Log("Charge Up");
        yield return new WaitForSeconds(abilityInfo.chargeUp);

        abilityInfo.AbilityActivate(this);
        OnActivateAbility?.Invoke();
        currentState = OwnerState.OnCooldown;
        updateEnd = Time.time + abilityInfo.duration;

        AbilityUpdate(this);
        CoolDown(this);
    }

    // Waits [abilityInfo.cooldown] seconds before resetting currentState to ReadyToUse.
    public IEnumerator CoolingDown()
    {
        // Debug.Log("Cool Down");
        yield return new WaitForSeconds(abilityInfo.cooldown);
        currentState = OwnerState.ReadyToUse;
    }

    // Runs abilityInfo.AbilityUpdate() repeatedly (waiting abilityInfo.tickRate second each time) until the ability ends.
    // Then running abilityInfo.AbilityDisable() for each effect type.
    public IEnumerator UpdateWithinDuration()
    {
        while (Time.time < updateEnd)
        {
            abilityInfo.AbilityUpdate(this);
            yield return new WaitForSeconds(abilityInfo.tickRate);
        }
        // After the ability's duration is over
        abilityInfo.AbilityDisable(this, AbilityEffectType.Immediate);
        abilityInfo.AbilityDisable(this, AbilityEffectType.Constant);
        abilityInfo.AbilityDisable(this, AbilityEffectType.Continuous);
    }

    // If the ability is ready to use, start the charge up.
    public void ActivateAbility()
    {
        if (currentState != OwnerState.ReadyToUse)
        {
            AudioManager.Instance.PlaySFX(abilityInfo.onCooldownSound);
            return;
        }

        currentState = OwnerState.Activation;
        ChargeUp(this);
    }
}
