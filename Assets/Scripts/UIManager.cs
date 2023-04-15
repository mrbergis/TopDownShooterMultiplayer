using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField]
    TMP_Text healthText;

    [SerializeField]
    TMP_Text ammoText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetHealthText(int health)
    {
        if (healthText)
        {
            healthText.text = "Health: " + health;
        }
    }

    public void SetAmmoText(int ammoCount)
    {
        if (ammoText)
        {
            ammoText.text = "Ammo: " + ammoCount;
        }
    }
}
