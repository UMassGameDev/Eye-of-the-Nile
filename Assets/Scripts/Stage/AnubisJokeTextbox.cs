using UnityEngine;
using TMPro;
using System.Collections;

public class AnubisJokeTextbox : MonoBehaviour
{
    public TMP_Text textbox;
    public string[] defaultJokes;
    public string[] fireDeathJokes;

    string deathMessage;

    void Awake()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        UpdateTextbox();
    }

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
