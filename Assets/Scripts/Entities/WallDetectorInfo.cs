using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** \brief
This script is used by some entities (including the player) to detect when they are touching a wall.
If either side of the entity is touching the wall, onWall is set to true.
When the wall detector consists of two trigger zones, frontOnWall and backOnWall become available.

WallDetectorZone.cs is what updates this script. See WallDetectorZone.cs for more info!

Documentation updated 12/19/2024
\author Alexander Art
*/
public class WallDetectorInfo : MonoBehaviour
{
    /// The layers which objects are considered part of the wall are on.
    public LayerMask wallLayer;

    /// True when either side of the entity is touching a wall.
    public bool onWall { get; private set; }
    /// True when there is a front trigger zone and it is touching a wall.
    public bool frontOnWall { get; private set; }
    /// True when there is a back trigger zone and it is touching a wall.
    public bool backOnWall { get; private set; }

    public void wallTouched(WallDetectorSide side)
    {
        // onWall is set to true when any side is touching the wall.
        onWall = true;

        switch (side)
        {
            case WallDetectorSide.Front:
                frontOnWall = true;
                break;
            case WallDetectorSide.Back:
                backOnWall = true;
                break;
        }
    }

    public void wallUntouched(WallDetectorSide side)
    {
        switch (side)
        {
            case WallDetectorSide.Both:
                onWall = false;
                break;
            case WallDetectorSide.Front:
                frontOnWall = false;

                // If neither side is on the wall, onWall should be false too.
                if (backOnWall == false)
                {
                    onWall = false;
                }

                break;
            case WallDetectorSide.Back:
                backOnWall = false;

                // If neither side is on the wall, onWall should be false too.
                if (frontOnWall == false)
                {
                    onWall = false;
                }

                break;
        }
    }
}