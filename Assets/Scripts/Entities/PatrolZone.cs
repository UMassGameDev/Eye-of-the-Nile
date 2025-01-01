#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

/** \brief
Script for a zone which entities that inherit from BaseEntityController will patrol until a player is in range.
Also used by Geb to stay within his bossroom.

Documentation updated 8/26/2024
\author Roy Pascual
*/
public class PatrolZone : MonoBehaviour
{
    /// Reference to a point representing the left end of the patrol zone.
    /// The entity will go back and forth between this point and rightEnd while patrolling.
    Transform leftEnd;
    /// Reference to a point representing the right end of the patrol zone.
    /// The entity will go back and forth between this point and leftEnd while patrolling.
    Transform rightEnd;

    /// Returns the position of the left end of the patrol zone.
    public Vector2 LeftPoint()
    {
        return leftEnd.position;
    }

    /// Returns the position of the right end of the patrol zone.
    public Vector2 RightPoint()
    {
        return rightEnd.position;
    }

    /// <summary>
    /// Shows icons for the left and right ends in the Unity Editor scene view.
    /// This allows developers to see where these points are so they can be easily adjusted.
    /// </summary>
    /// \important Must be commented out or removed to export the game. Otherwise, Unity will throw compiler errors.
    private void OnDrawGizmos()
    {
        if (leftEnd == null || rightEnd == null)
        {
            leftEnd = transform.Find("LeftEnd");
            rightEnd = transform.Find("RightEnd");
        }

        #if UNITY_EDITOR
            if (Selection.activeTransform == transform
                || Selection.activeTransform == leftEnd
                || Selection.activeTransform == rightEnd)
            {
                Gizmos.DrawIcon(leftEnd.position, "sv_icon_dot14_pix16_gizmo", true, Color.green);
                Gizmos.DrawIcon(rightEnd.position, "sv_icon_dot14_pix16_gizmo", true, Color.green);
            }
        #endif
    }

    /// Sets the left and right ends and sets their references so their positions can be accessed later.
    void Awake()
    {
        leftEnd = transform.Find("LeftEnd");
        rightEnd = transform.Find("RightEnd");
    }
}
