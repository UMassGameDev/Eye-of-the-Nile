/**************************************************
Script for a zone which entities that inherit from BaseEntityController will patrol until a player is in range.

Documentation updated 1/29/2024
**************************************************/
using UnityEditor;
using UnityEngine;

public class PatrolZoneProto : MonoBehaviour
{
    public bool spawnedIn = false;
    public Vector2 SpawnBasePos { get; set; }
    public Vector2 leftEndOffset = new Vector2(0f, 0f);
    public Vector2 rightEndOffset = new Vector2(0f, 0f);
    Color leftColor = new Color((170f/255f), (250f/255f), (112f/255f), 1f);
    Color rightColor = new Color((62f/255f), (207f/255f), (76f/255f), 1f);

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

    private void OnDrawGizmos()
    {
        if (Selection.activeTransform == transform)
        {
            Gizmos.DrawIcon(LeftPoint(), "sv_icon_dot14_pix16_gizmo", true, leftColor);
            Gizmos.DrawIcon(RightPoint(), "sv_icon_dot14_pix16_gizmo", true, rightColor);
        }
    }

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
