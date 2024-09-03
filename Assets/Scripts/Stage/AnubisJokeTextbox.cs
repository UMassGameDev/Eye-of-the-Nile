using UnityEngine;
using TMPro;
using System.Collections;

/** \brief
This script is responsible for displaying the joke that Anubis is supposed to tell when the player dies (stored in the DataManager) in
the speech bubble above Anubis' head.

Documentation updated 9/3/2024
\author Stephen Nuttall
*/
public class AnubisJokeTextbox : MonoBehaviour
{
    /// Reference to the textbox inside the speech bubble that needs to be modified.
    public TMP_Text textbox;
    /// List of the default jokes Anubis can tell. These are told if there's no specialized joke given from the object that killed the player.
    public string[] defaultJokes;
    /// \brief List of the default jokes Anubis can tell specifically if the player burns to death.
    /// The object that set the player on fire does not know when the player succumbs to the fire damage, so the list needs to be kept here.
    public string[] fireDeathJokes;

    string deathMessage;

    /// Wait a second before displaying the joke. This allows Awake() in the DataManager to run its course first, so the correct data is used.
    void Awake()
    {
        StartCoroutine(Delay());
    }

    /// Waits a second, then updates the textbox.
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        UpdateTextbox();
    }

    /// \brief Gets the joke to display from the DataManager, and set the text in the speech bubble to match it.
    /// If the joke is [DEFAULT] or [FIRE], we need to choose a joke ourselves from the corresponding list.
    void UpdateTextbox()
    {
        deathMessage = GameObject.Find("DataManager").GetComponent<DataManager>().GetAnubisDeathMessage();

        if (deathMessage == "[DEFAULT]" || deathMessage == null) {
            deathMessage = defaultJokes[Random.Range(0, defaultJokes.Length)];
        } else if (deathMessage == "[FIRE]") {
            deathMessage = fireDeathJokes[Random.Range(0, fireDeathJokes.Length)];
        }

        textbox.SetText(deathMessage);
    }
}
