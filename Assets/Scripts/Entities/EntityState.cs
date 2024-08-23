/** \brief
Simple enum for the BaseEntityController for handling state.

Documentation updated 8/23/2024

\author Roy Rapscual
**/
public enum EntityState
{
    /*! Entity is patrolling between two points in a patrol zone.*/
    Patrol, 
    /*! Entity is chasing after an enemy.*/
    Chase, 
    /*! Entity is attacking an enemy.*/
    CloseAttack, 
    /*! Entity is dead.*/
    Dead 
}