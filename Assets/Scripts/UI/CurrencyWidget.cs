using UnityEngine;
using TMPro;

public class CurrencyWidget : MonoBehaviour
{
    public TMP_Text soulText;
    public TMP_Text godSoulText;
    DataManager dataManager;

    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    void Update()
    {
        dataManager.GetSouls();
    }
}
