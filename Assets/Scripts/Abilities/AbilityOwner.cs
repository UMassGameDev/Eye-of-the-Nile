using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/*!<summary>
Handles timed events related to abilities. Specifically, charge ups, activating the ability, repeated events during
duration time (such as healing over time), and cooldowns.
It also holds useful data, namely the OwnerTransform (player’s transform), the ability info this ability is using,
and the current ability state (ready to use vs on cooldown).

Documentation updated 8/21/2024
</summary>
\author Roy Pascual
\note This script does not inherit from monobehavior, so it does not have many of the default unity functions like Start() and Update().
\note This class does not inhert from monobehavior, so it does not have access to unity functions such as Start() or Update().*/
public class AbilityOwner // : MonoBehaviour
{
    public enum OwnerState { ReadyToUse, Activation, OnCooldown };

    /// \brief delegate used to create the ChargeUp event.
    public delegate void ChargeUpEvent(AbilityOwner abilityOwner);
    /// \brief When triggered, a function subscribed to it in PlayerAbilityController.cs will run ChargingUp().
    /// \note For a detailed description of how this event is used, visit /ref abilityOwnerEvents
    public event ChargeUpEvent ChargeUp;

    /// \brief delegate used to create the CoolDown event.
    public delegate void CooldownEvent(AbilityOwner abilityOwner);
    /// \brief When triggered, a function subscribed to it in PlayerAbilityController.cs will run CoolingDown().
    /// \note For a detailed description of how this event is used, visit /ref abilityOwnerEvents
    public event CooldownEvent CoolDown;

    /// \brief delegate used to create the AbilityUpdate event.
    public delegate void UpdateEvent(AbilityOwner abilityOwner);
    /// \brief When triggered, a function subscribed to it in PlayerAbilityController.cs will run UpdateWithinDuration().
    /// \note For a detailed description of how this event is used, visit /ref abilityOwnerEvents
    public event UpdateEvent AbilityUpdate;

    /// \brief Transform of the ability owner (usually the player's transform).
    public Transform OwnerTransform { get; set; }
    /// \brief A generic event that is called when an ability is activated. Assigned in the Unity Editor under PlayerAbilityController.cs.
    public UnityEvent OnActivateAbility;
    /// \brief The ability info that this ability uses.
    public BaseAbilityInfo abilityInfo;
    /// \brief Whether this ability is ready to use, activating, or on cooldown.
    public OwnerState currentState = OwnerState.ReadyToUse;

    /// \brief Time until ability duration is up.
    float updateEnd = 0f;
    /// \brief Time when ability cooldown ends.
    public float cooldownEnd = 0f;

    /// <summary>
    /// When a new abilityOwner object is created in another class, this function automatically runs.
    /// It assigns the three variables to the given parameters, which must be given in order to create this object.
    /// </summary>
    /// <param name="ownerTransform"> The transform of the player. </param>
    /// <param name="onActivateAbility"> A generic event that is called when an ability is activated. </param>
    /// <param name="newAbilityInfo"> The ability info that this ability uses. </param>
    public AbilityOwner(Transform ownerTransform,
        UnityEvent onActivateAbility,
        BaseAbilityInfo newAbilityInfo)
    {
        OwnerTransform = ownerTransform;
        OnActivateAbility = onActivateAbility;
        abilityInfo = newAbilityInfo;
    }

    /// <summary>
    /// Waits [abilityInfo.chargeUp] seconds, before triggering the ability, prepares for cooldown, and then triggers AbilityUpdate and CoolDown.
    /// </summary>
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

    /// <summary>
    /// Waits [abilityInfo.cooldown] seconds before resetting currentState to ReadyToUse.
    /// </summary>
    public IEnumerator CoolingDown()
    {
        cooldownEnd = Time.time + abilityInfo.cooldown;

        // Debug.Log("Cool Down");
        yield return new WaitForSeconds(abilityInfo.cooldown);
        currentState = OwnerState.ReadyToUse;
    }

    /// <summary>
    /// Runs abilityInfo.AbilityUpdate() repeatedly (waiting abilityInfo.tickRate second each time) until the ability ends.
    /// Then running abilityInfo.AbilityDisable() for each effect type.
    /// </summary>
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

    /// If the ability is ready to use, start the charge up.
    public void ActivateAbility()
    {
        if (currentState != OwnerState.ReadyToUse)
        {
            AudioManager.instance.PlaySFX(abilityInfo.onCooldownSound);
            return;
        }

        currentState = OwnerState.Activation;
        ChargeUp(this);
    }

    public void EnablePassive()
    {
        if (abilityInfo.currentForm == AbilityForm.Passive)
        {
            abilityInfo.AbilityActivate(this);
            Debug.Log("Passive ability enabled for the '" + abilityInfo.abilityName + "' set.");
        }
    }

    public void DisablePassive()
    {
        abilityInfo.DisablePassive(this);
        Debug.Log("Passive ability disabled for the '" + abilityInfo.abilityName + "' set.");
    }
}
