using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private void Start()
    {
        
        if (fillImage == null)
        {
            fillImage = GetComponentInChildren<Image>();
        }
    }

    public void UpdateHpbar(float currentHealth, float maxHealth)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }

}
