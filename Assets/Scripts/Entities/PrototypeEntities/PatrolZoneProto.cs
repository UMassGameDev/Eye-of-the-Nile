#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

/** \brief
Script for a zone which entities that inherit from BaseEntityController will patrol until a player is in range.

Documentation updated 9/7/2024
\author Roy Pascual
*/
public class PatrolZoneProto : MonoBehaviour
{
    /// True if the prototype entity has been spawned in. Used to determine what LeftPoint() and RightPoint() should return.
    public bool spawnedIn = false;
    /// The base position of the patrol zone. This will be set to the entity's position in PrototypeBaseEntity.InitializePatrolZone().
    public Vector2 SpawnBasePos { get; set; }
    /// \brief The offset from SpawnBasePos that the left end of the patrol zone is.
    /// The entity will go back and forth between this point and the right end while patrolling.
    /// This will be set to leftOffset in PrototypeBaseEntity.InitializePatrolZone().
    public Vector2 leftEndOffset = new Vector2(0f, 0f);
    /// \brief The offset from SpawnBasePos that the right end of the patrol zone is.
    /// The entity will go back and forth between this point and the left end while patrolling.
    ///This will be set to rightOffset in PrototypeBaseEntity.InitializePatrolZone().
    public Vector2 rightEndOffset = new Vector2(0f, 0f);
    /// In the Unity Editor, the left point will be displayed as a dot with this color.
    Color leftColor = new Color((170f/255f), (250f/255f), (112f/255f), 1f);
    /// In the Unity Editor, the right point will be displayed as a dot with this color.
    Color rightColor = new Color((62f/255f), (207f/255f), (76f/255f), 1f);

    /// Returns the position of the left end of the patrol zone.
    public Vector2 LeftPoint()
    {
        if (!spawnedIn)
        {
            Vector2 basePos = transform.position;
            return basePos + leftEndOffset;
        }
        else
        {
            return SpawnBasePos + leftEndOffset;
        }
            
    }

    /// Returns the position of the right end of the patrol zone.
    public Vector2 RightPoint()
    {
        if (!spawnedIn)
        {
            Vector2 basePos = transform.position;
            return basePos + rightEndOffset;
        }
        else
        {
            return SpawnBasePos + rightEndOffset;
        }
    }

    /// \breif Shows icons for the left and right ends in the Unity Editor scene view.
    /// This allows developers to see where these points are so they can be easily adjusted.
    /// \important Must be commented out or removed to export the game. Otherwise, Unity will throw compiler errors.
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
            if (Selection.activeTransform == transform)
            {
                Gizmos.DrawIcon(LeftPoint(), "sv_icon_dot14_pix16_gizmo", true, leftColor);
                Gizmos.DrawIcon(RightPoint(), "sv_icon_dot14_pix16_gizmo", true, rightColor);
            }
        #endif
    }
}
