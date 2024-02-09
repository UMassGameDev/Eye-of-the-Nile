/**************************************************
Modified version of StageWarp.cs for the Anubis scene.
In that scene, there's a stage warp that brings you back to your spawn point.
Inherits from StageWarp.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class AnubisStageWarp : StageWarp
{
    void Awake()
    {
        sceneToWarpTo = GameObject.Find("DataManager").GetComponent<DataManager>().GetPrevSceneName();
        entryPoint = transform.Find("EntryPoint");
    }

}
