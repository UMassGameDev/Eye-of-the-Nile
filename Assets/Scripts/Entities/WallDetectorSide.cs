/** \brief
Simple enum for the wall detector trigger zones to say which side they are on.

Documentation updated 12/19/2024
\author Alexander Art
**/
public enum WallDetectorSide
{
    /*! This trigger zone detects when the entity is facing a wall.*/
    Front,
    /*! This trigger zone detects when the entity is back against a wall.*/
    Back,
    /*! The trigger zone takes the role of both Front and Back.*/
    Both
}