using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthbarGUI : MonoBehaviour
{
    public RectTransform healthbarTransform;
    PlayerHealth playerHealth;

    public float healthbarXLoc;  // this is where the healthbar's x should be "pinned" to
    float healthMultiplier = 5f;

    void Awake()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    void Start()
    {
        healthMultiplier = healthbarTransform.sizeDelta.x/playerHealth.maxHealth;
    }
    
    void Update()
    {
        healthbarTransform.sizeDelta = new Vector2(playerHealth.GetHealth() * healthMultiplier, healthbarTransform.sizeDelta.y);

        float XOffset = (playerHealth.maxHealth - playerHealth.GetHealth())  * healthMultiplier/2;
        healthbarTransform.anchoredPosition = new Vector2(healthbarXLoc + XOffset, healthbarTransform.anchoredPosition.y);
    }
}
