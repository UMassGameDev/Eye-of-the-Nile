/**************************************************

This script is only comments. It exists to generate pages in the documentation html.

**************************************************/

// MAIN PAGE
/*!
    \mainpage Welcome to the Game 1 Codebase Documentation!

    Welcome to the codebase documentation! This is automatically generated using Doxygen, which takes code comments formatted in a specific way and
    creates an HTML page that's easy to read an navigate. Whether you're confused on how a script or system works or just need to reference a specific
    variable name, this page is very helpful.

    Anything outside of the code itself is documented in the Game 1 Unity Documentation, found here:

    https://docs.google.com/document/d/19QBQ7h_pu8xe1ZAPgEQJj8Z77NwPePQkKF9CdKGLVXs/edit?usp=sharing

    When you add to the codebase yourself, it's very appreciated if you write similar comments (or at least
    comments in general) that explain the functionality and purpose of each variable, function, and script you make. Don't worry too much about formatting
    (I can always reformat it later to fit what Doxygen wants), as long as the information is there. If you do want to format it properly, use the other
    scripts as an example or reference the Doxygen documentation.
*/

// FOLDER TREE
/*!
    \page folderTree Classes by File Path

    \section Folder_General General
    - \subpage CustomDeathMessage
    - \subpage DataManager
    - \subpage ObjectHealth
    - \subpage ObjectInteractable
    - \subpage SoftCollider
    \section Folder_Abilities Abilities
    - \subpage AbilityInventory
        /subsection Folder_AbilityEffects Ability Effects
        - \subpage AbilityEffect
        /subsection Folder_AbilityInfo Ability Info
        - \subpage BaseAbilityInfo
    \section Folder_Audio Audio
    \section Folder_Entities Entities
    \section Folder_Hazards Hazards
    \section Folder_Player Player
    \section Folder_Stage Stage
    \section Folder_UI UI
*/


// EXPLANATION OF THE EVENTS IN ABILITYOWNER.cs
/*!
    \page abilityOwnerEvents Explanation of the events in AbilityOwner.cs
    This page explains what the events in AbilityOwner.cs do and how they work, because it's not intuitive to all devs (even some experienced ones).

    TLDR: These events are used to run functions that this script can't normally run because it doesn't have acccess to StartCoroutine()
    (AbilityOwner.cs does not inherit from MonoBehavior).

    The events in question are:
    \code{.cs}
    public delegate void ChargeUpEvent(AbilityOwner abilityOwner);
    public event ChargeUpEvent ChargeUp;

    public delegate void CooldownEvent(AbilityOwner abilityOwner);
    public event CooldownEvent CoolDown;

    public delegate void UpdateEvent(AbilityOwner abilityOwner);
    public event UpdateEvent AbilityUpdate;
    \endcode

    First thing to note that those new to Unity may not know: Events are functions that are triggered when something happens in a script,
    that other functions can then subscribe to. In this example, if an ability starts its cooldown, a function in a completely different script
    can be notified and start running code in response. This is very useful if you want to "do why when x happens." Instead of checking every frame
    if something has happened, it's recommended you use this.

    There's multiple ways to create events, but in this example, a delegate is created, defining the parameters of the event, and then an event is
    created using that delegate. When these events are called, the abilityOwner is attached so any function subscribed can know what abilityOwner
    is charging up/cooling down/updating.

    Now that we've explained the basics of events, what are these events doing?

    These events trigger a function subscribed to them PlayerAbilityController.cs, which in turn runs a function back in this script.
    Specifically:
    
    ChargeUp triggers UseAbilityChargingUp() in PlayerAbilityController.cs,
    which in turn starts a coroutine for ChargingUp() in this script.

    CoolDown triggers UseAbilityCoolingDown() in PlayerAbilityController.cs,
    which in turn starts a coroutine for CoolingDown() in this script.
    
    AbilityUpdate triggers UseAbilityUpdate() in PlayerAbilityController.cs,
    which in turn starts a coroutine for UpdateWithinDuration() in this script.

    In short, these events are part of a more elaborate way of just calling ChargingUp(), CoolingDown(), and UpdatingWithinDuration().
    
    Why? Because these functions are IEnumerator functions. IEnumerator functions allow for the program to wait an amount of time before
    continuing. This is how the charge up, cool down, and update times are implemented. A yield return statement is run for some amount of
    seconds, before doing whatever needs to be done after that time has passed.

    The problem? IEnumerator functions can't be called by normal functions, but have to instead use Unity's StartCoroutine() function.
    AbilityOwner.cs does not inherit from MonoBehavior though, meaning it does not have access to StartCoroutine(). That's where these events
    come in. By running these events, a script that does have access to the MonoBehavior functions (such as PlayerAbilityController.cs) can run
    StartCoroutine() for AbilityOwner.cs.

    Pro-tip: Press F12 in Visual Studio Code to go to where a variable is defined, and press F12 again to see everywhere where it's used.
    If your VSCode is configured correctly, it should show you a list of every file the variable is referenced in and where.
    Very useful for dissecting the functionality of unfamiliar code, and is how I initially figured out this unintuitive design.
    */