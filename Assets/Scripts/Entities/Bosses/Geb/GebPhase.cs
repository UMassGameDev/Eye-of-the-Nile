/** \brief
Simple enum for Geb for handling the phase of the boss battle.

Documentation updated 12/28/2024
\author Alexander Art
**/
public enum GebPhase
{
    /*! Geb's boss battle hasn't been started yet.*/
    Inactive,
    /*! The player showed up, the battle starts, and Geb is on phase 1.*/
    Phase1, 
    /*! Geb lost some health and is on phase 2.*/
    Phase2, 
    /*! Geb's health is low and is on phase 3.*/
    Phase3, 
    /*! Geb has been defeated and the boss battle is over.*/
    Defeated
}