using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** \brief
This script is used by WallDetectorZone.cs to keep track of when the parent entity is touching a wall.
If either side of the entity is touching the wall, onWall is set to true.
When the wall detector has a front half and a back half, frontOnWall and backOnWall become available.
See WallDetectorZone.cs for more info!

Documentation updated 12/18/2024
\author Alexander Art
*/
public class WallDetectorInfo : MonoBehaviour
{
    /// The layers which objects are considered part of the wall are on.
    public LayerMask wallLayer;

    /// True when either side of the entity is touching a wall.
    public bool onWall { get; private set; } = true;
    /// True when there is a left trigger zone and it is touching a wall.
    public bool frontOnWall { get; private set; }
    /// True when there is a right trigger zone and it is touching a wall.
    public bool backOnWall { get; private set; }

    public void onWallUpdate(bool touching)
    {
        if (touching == true)
        {
            onWall = true;
        }
        else if (touching == false)
        {
            onWall = false;
        }
    }

    public void frontOnWallUpdate(bool touching)
    {
        if (touching == true)
        {
            frontOnWall = true;
            onWall = true;
        }
        else if (touching == false)
        {
            frontOnWall = false;
            if (backOnWall == false)
            {
                onWall = false;
            }
        }
    }

    public void backOnWallUpdate(bool touching)
    {
        if (touching == true)
        {
            backOnWall = true;
            onWall = true;
        }
        else if (touching == false)
        {
            backOnWall = false;
            if (frontOnWall == false)
            {
                onWall = false;
            }
        }
    }
}
