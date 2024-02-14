/**************************************************
This script holds information about what joke Anubis should tell if the player dies to this object

Documentation updated 2/13/2024
**************************************************/
using UnityEngine;

public class CustomDeathMessage : MonoBehaviour
{
    public string[] customAnubisJokes = new string[1];

    public string GetRandomJoke()
    {
        if (customAnubisJokes.Length == 1)
            return customAnubisJokes[0];

        int i;
        do {
            i = Random.Range(0, customAnubisJokes.Length - 1);
        } while (customAnubisJokes[i] == null);

        return customAnubisJokes[i];
    }
}
