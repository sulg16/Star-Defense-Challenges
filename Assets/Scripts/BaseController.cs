using UnityEngine;

public class BaseController : MonoBehaviour
{
    public static BaseController Instance { get; private set;}

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private HPbar hpbar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        { 
            Instance = this;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        hpbar = GetComponentInChildren<HPbar>();

        if (hpbar != null)
        {
            hpbar.UpdateHpbar(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (hpbar != null)
        {
            hpbar.UpdateHpbar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowRestartButton();
        }
        Time.timeScale = 0;
    }

}