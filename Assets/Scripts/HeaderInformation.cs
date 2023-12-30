using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInformation : MonoBehaviourPun
{
    public TextMeshProUGUI playerName;
    public Image healthBar;
    private float maxHealthValue;
    
    public void Initialized(string text, int maxVal)
    {
        playerName.text = text;
        maxHealthValue = maxVal;
        healthBar.fillAmount = 1.0f;
    }

    [PunRPC]
    void UpdateHealthBar(int value)
    {
        healthBar.fillAmount = (float)value / maxHealthValue;
    }
}
