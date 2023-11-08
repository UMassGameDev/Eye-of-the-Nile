using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageWarp : MonoBehaviour
{
    public string srcWarpName = "DEFAULT_SOURCE";
    public string destWarpName = "DEFAULT_DESTINATION";
    public StageWarpType warpType = StageWarpType.DirectExit;

    // Edit this string to the name of the destination scene
    public string sceneToWarpTo;
    // Every StageWarp has an "EntryPoint" as a child which serves as a spawn point
    public Vector2 EntryPos { get { return entryPoint.position; } }
    private Transform entryPoint;

    void Awake()
    {
        entryPoint = transform.Find("EntryPoint");
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
