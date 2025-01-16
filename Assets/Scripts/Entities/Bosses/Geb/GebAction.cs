/** \brief
Enum for the actions that Geb uses in phases 1-3.
New actions unlock cumulatively with each additional phase until the fight is over.
This enum is NOT used for phases Inactive, OpeningCutscene, ClosingCutscene, or Defeated. Those phases are not part of the boss fight!

Documentation updated 1/16/2025
\author Alexander Art
**/
public enum GebAction
{
    /*! Unlocked in phase 1. On Idle, Geb stops moving and (briefly) does nothing.*/
    Idle,
    /*! Unlocked in phase 1. On Moving, Geb moves until reaching a target position.*/
    Moving,
    /*! Unlocked in phase 1. On RockThrowAttack, Geb stops moving, prepares to throw a rock, and ends by throwing it.*/
    RockThrowAttack,
    /*! Unlocked in phase 2. On WallSummon, Geb stops moving and raises a wall. (Counts as an attack.)*/
    WallSummon,
    /*! Unlocked in phase 2. On ChargeAttack, Geb runs towards the player, deals damage, and breaks any walls in his path.*/
    ChargeAttack,
    /*! Unlocked in phase 3. On Earthquake, Geb quakes the ground near him while rocks fall from the sky like meteors.*/
    Earthquake,
    /*! Unlocked in phase 3. On RockTornado, Geb surrounds himself in a protective tornado that damages the player on touch.*/
    RockTornado
}