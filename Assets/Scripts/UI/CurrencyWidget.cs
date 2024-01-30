/**************************************************
Updates the text for the souls and godsouls when the player's soul amount changes.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;
using TMPro;
using System;

public class CurrencyWidget : MonoBehaviour
{
    public TMP_Text soulText;
    public TMP_Text godSoulText;

    public static event Action onStart;

    void OnEnable()
    {
        DataManager.newSoulTotal += updateSoulText;
        DataManager.newGodSoulTotal += updateGodSoulText;
    }

    void OnDisable()
    {
        DataManager.newSoulTotal -= updateSoulText;
        DataManager.newGodSoulTotal -= updateGodSoulText;
    }

    void Start()
    {
        onStart?.Invoke();
    }

    void updateSoulText(int newTotal)
    {
        soulText.text = newTotal.ToString();
    }

    void updateGodSoulText(int newTotal)
    {
        godSoulText.text = newTotal.ToString();
    }
}
