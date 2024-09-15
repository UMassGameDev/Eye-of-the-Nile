using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EmptyAbilityInfo", menuName = "Abilities/Create New EmptyAbilityInfo")]
/*!<summary>
This is an empty ability that does nothing.
This allows for there to be empty slots in the active ability data.
Each of the abstract functions just write “This ability does nothing” to the console.

Documentation updated 8/21/2024
</summary>
\author Stephen Nuttall
\note This is a scriptable object, meaning you can make an instance of it in the Unity Editor that exists in the file explorer.
*/
public class EmptyAbilityInfo : BaseAbilityInfo
{
    protected override void AbilityOffense(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }

    protected override void AbilityDefense(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }

    protected override void AbilityUtility(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }

    protected override void AbilityPassive(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }
}
