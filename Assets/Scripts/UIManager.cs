using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject gameOverPopupContainer;
    public GameObject winpopupContainer;
    public GameObject unitMenuPopup;
    private GameObject selecteUnit;
    public bool IsGameOver { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void ShowRestartButton()
    {
        if (gameOverPopupContainer != null)
        {
            gameOverPopupContainer.SetActive(true);
        }
        IsGameOver = true;
    }

    public void RestartGame()
    {
        IsGameOver = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowWinPopup()
    {
        winpopupContainer.SetActive(true);
    }

    public void ShowUnitMenu(GameObject unit)
    {
        HideUnitMenu();

        selecteUnit = unit;
        if (unitMenuPopup != null)
        {
            unitMenuPopup.SetActive(true);
        }
    }

    public void HideUnitMenu()
    {
        if (unitMenuPopup != null && unitMenuPopup.activeSelf)
        {
            unitMenuPopup.SetActive(false);
            selecteUnit = null;
        }
    }

    public void StartRelocationMode()
    {
        if (selecteUnit != null)
        {
            HideUnitMenu();
            RelocationManager.Instance.StartRelocation(selecteUnit);
        }
    }
}
