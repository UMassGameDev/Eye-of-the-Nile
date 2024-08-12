/**************************************************
Add this script to any object which you want custom Anubis jokes for when the player dies to it.
For example, this is used on fire to make Anubis tell a fire pun after you burn to death.

Documentation updated 8/12/2024
**************************************************/
using UnityEngine;

public class CustomDeathMessage : MonoBehaviour
{
    // Stores the possible jokes that Anubis can tell. These can be added in the Unity Editor.
    public string[] customAnubisJokes = new string[1];

    // Returns a random joke from the array.
    public string GetRandomJoke()
    {
        // if there's only one joke, just choose that joke
        if (customAnubisJokes.Length == 1)
            return customAnubisJokes[0];

        // choose a new random joke until we get one that's not null
        int i;
        do {
            i = Random.Range(0, customAnubisJokes.Length - 1);
        } while (customAnubisJokes[i] == null);

        return customAnubisJokes[i];
    }
}
