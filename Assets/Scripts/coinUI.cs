using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class coinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        UpdateCoinUI(CurrentManager.Instance.currentCoin);
    }

    private void OnEnable()
    {
        CurrentManager.OnCoinChanged += UpdateCoinUI;
    }

    private void OnDisable()
    {
        CurrentManager.OnCoinChanged -= UpdateCoinUI;
    }

    private void UpdateCoinUI(int coin)
    {
        coinText.text = coin.ToString();
    }
}
