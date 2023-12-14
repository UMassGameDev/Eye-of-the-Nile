using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityOwner // : MonoBehaviour
{
    public enum OwnerState { ReadyToUse, Activation, OnCooldown };

    public delegate void ChargeUpEvent(AbilityOwner abilityOwner);
    public event ChargeUpEvent ChargeUp;

    public delegate void CooldownEvent(AbilityOwner abilityOwner);
    public event CooldownEvent CoolDown;

    public delegate void UpdateEvent(AbilityOwner abilityOwner);
    public event UpdateEvent AbilityUpdate;

    public Transform OwnerTransform { get; set; }
    public UnityEvent OnActivateAbility;
    public BaseAbilityInfo abilityInfo;
    public OwnerState currentState = OwnerState.ReadyToUse;

    float cooldownEnd = 0f;
    float updateEnd = 0f;

    public AbilityOwner(Transform ownerTransform,
        UnityEvent onActivateAbility,
        BaseAbilityInfo newAbilityInfo)
    {
        OwnerTransform = ownerTransform;
        OnActivateAbility = onActivateAbility;
        abilityInfo = newAbilityInfo;
    }

    public IEnumerator ChargingUp()
    {
        Debug.Log("Charge Up");
        yield return new WaitForSeconds(abilityInfo.chargeUp);
        abilityInfo.AbilityActivate(this);
        OnActivateAbility?.Invoke();
        currentState = OwnerState.OnCooldown;
        /*cooldownEnd = Time.time + abilityInfo.cooldown;*/
        // StartCoroutine(CoolingDown());
        updateEnd = Time.time + abilityInfo.duration;
        AbilityUpdate(this);
        CoolDown(this);
    }

    public IEnumerator CoolingDown()
    {
        Debug.Log("Cool Down");
        yield return new WaitForSeconds(abilityInfo.cooldown);
        currentState = OwnerState.ReadyToUse;
    }

    public IEnumerator UpdateWithinDuration()
    {
        while (Time.time < updateEnd)
        {
            // Use the update method
            abilityInfo.AbilityUpdate(this);
            yield return new WaitForSeconds(abilityInfo.tickRate);
        }
        // After the ability's duration is over
        abilityInfo.AbilityDisable(this, AbilityEffectType.Immediate);
        abilityInfo.AbilityDisable(this, AbilityEffectType.Constant);
        abilityInfo.AbilityDisable(this, AbilityEffectType.Continuous);
    }

    public void ActivateAbility()
    {
        if (currentState != OwnerState.ReadyToUse)
            return;

        currentState = OwnerState.Activation;
        // Charge up ability
        // StartCoroutine(ChargingUp());
        ChargeUp(this);
    }

    // Update is called once per frame
    /*void Update()
    {
        *//*if (Time.time > cooldownEnd && currentState == OwnerState.OnCooldown)
            currentState = OwnerState.ReadyToUse;*//*
    }*/
}
