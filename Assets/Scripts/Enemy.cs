using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private int baseDamage = 20;
    [SerializeField] private int coinValue = 10;

    private HPbar hpbar;

    private MapManager mapManager;
    private int pathIndex = 0;
    private Vector3 targetWorldPosition;

    private bool isDeadOrEscaped = false;

    private void Start()
    {
        currentHealth = maxHealth;
        mapManager = FindObjectOfType<MapManager>();
        

        if (mapManager == null) 
        {
            Destroy(gameObject);
            return;
        }

        pathIndex = 0;

        if (mapManager.monsterPath.Count > 0)
        {
            Vector2Int startCell = mapManager.monsterPath[pathIndex];
            targetWorldPosition = mapManager.GetWorldCenterForCell(startCell.x, startCell.y);
            transform.position = targetWorldPosition;

            pathIndex = 1;


            if (mapManager.monsterPath.Count > 1)
            {
                Vector2Int firstTarget = mapManager.monsterPath[pathIndex];
                targetWorldPosition = mapManager.GetWorldCenterForCell(firstTarget.x, firstTarget.y);
            }
        }
        
        hpbar = GetComponent<HPbar>();
        if (hpbar != null)
        {
            hpbar.UpdateHpbar(currentHealth, maxHealth);
        }
        
        
    }

    private void Update()
    {
        if (mapManager == null)
        {
            return;
        }

        if (pathIndex >= mapManager.monsterPath.Count)
        {
            if (!isDeadOrEscaped)
            {
                isDeadOrEscaped = true;
                Sponer.monstersAlive--;
                CheckWinCondition();

            }
            
            BaseController.Instance.TakeDamage(baseDamage);
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPosition) < 0.01f)
        {
            pathIndex++;

            if (pathIndex >= mapManager.monsterPath.Count)
            {
                return;
            }

            Vector2Int nextCell = mapManager.monsterPath[pathIndex];
            targetWorldPosition = mapManager.GetWorldCenterForCell(nextCell.x, nextCell.y);
        }

        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (hpbar != null)
        {
            hpbar.UpdateHpbar(currentHealth, maxHealth);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isDeadOrEscaped)
        {
            isDeadOrEscaped = true;
            Sponer.monstersAlive--;
            CheckWinCondition();

        }
 
        CurrentManager.Instance.AddCoin(coinValue);
        Destroy(gameObject);
    }

    private void CheckWinCondition()
    {
        if (Sponer.allWavesFinished && Sponer.monstersAlive <= 0)
        {
            UIManager.Instance.ShowWinPopup();
            Time.timeScale = 0f;
        }
    }
}

