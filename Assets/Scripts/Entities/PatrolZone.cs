/**************************************************
Script for a zone which entities that inherit from BaseEntityController will patrol until a player is in range.

Documentation updated 1/29/2024
**************************************************/
using UnityEditor;
using UnityEngine;

public class PatrolZone : MonoBehaviour
{
    Transform leftEnd;
    Transform rightEnd;

    public Vector2 LeftPoint()
    {
        return leftEnd.position;
    }

    public Vector2 RightPoint()
    {
        return rightEnd.position;
    }

    private void OnDrawGizmos()
    {
        if (leftEnd == null || rightEnd == null)
        {
            leftEnd = transform.Find("LeftEnd");
            rightEnd = transform.Find("RightEnd");
        }

        if (Selection.activeTransform == transform
            || Selection.activeTransform == leftEnd
            || Selection.activeTransform == rightEnd)
        {
            Gizmos.DrawIcon(leftEnd.position, "sv_icon_dot14_pix16_gizmo", true, Color.green);
            Gizmos.DrawIcon(rightEnd.position, "sv_icon_dot14_pix16_gizmo", true, Color.green);
        }
    }

    void Awake()
    {
        leftEnd = transform.Find("LeftEnd");
        rightEnd = transform.Find("RightEnd");
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
