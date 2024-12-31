/** \brief
Simple enum for Geb for handling the phase/cutscene of the boss battle.

Documentation updated 12/30/2024
\author Alexander Art
**/
public enum GebPhase
{
    /*! Geb's boss battle hasn't been started yet and is waiting for the player.*/
    Inactive,
    /*! The player has showed up and Geb's opening cutscene is playing.*/
    OpeningCutscene,
    /*! The opening cutscene is over and the battle starts with Geb on phase 1.*/
    Phase1,
    /*! Geb lost some health and is on phase 2.*/
    Phase2,
    /*! Geb's health is low and is on phase 3.*/
    Phase3,
    /*! Geb has been defeated and the cutscene on his defeat is playing.*/
    ClosingCutscene,
    /*! The closing cutscene is over and Geb is now pacified.*/
    Defeated
}