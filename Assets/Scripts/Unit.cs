using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public static List<Unit> AllUnits = new List<Unit>();

    //기본
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 3.0f;
    private float attackTimer;
    private GameObject currentTarget;

    //머지 승급
    [SerializeField] private int level = 1; //기본
    [SerializeField] private int mergeCost = 50;
    [SerializeField] private Unit upgradedPrefab;

    public int Level => level;


    [SerializeField] private GameObject unitMenuCanvas;
    private bool isMenuOpen = false;

    public static Unit activeMenuUnit = null;

    private void Start()
    {
        if (unitMenuCanvas != null)
        {
            unitMenuCanvas.SetActive(false);
        }
        CircleCollider2D rangeCollider = GetComponentInChildren<CircleCollider2D>();
        if (rangeCollider != null)
        { 
            rangeCollider.radius = attackRange;
            rangeCollider.isTrigger = true;
        }
    }
    private void Update()
    {
        if (currentTarget != null)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
    }
    private void OnEnable()
    {
        AllUnits.Add(this);
    }

    private void OnDisable()
    {
        AllUnits.Remove(this);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && currentTarget == null)
        {
            currentTarget = other.gameObject;
            attackTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentTarget)
        {
            currentTarget = null;
        }
    }
    private void Attack()
    {
        if (currentTarget != null)
        {
            Enemy monster = currentTarget.GetComponent<Enemy>();
            if (monster != null)
            {
                monster.TakeDamage(attackDamage);
            }
            else
            {
                currentTarget = null;
            }
        }
    }

    public void OpenMenu()
    {
        if (unitMenuCanvas == null)
        {
            return;
        }
        if (isMenuOpen)
        {
            return;
        }

        isMenuOpen = true;
        unitMenuCanvas.SetActive(true);
        activeMenuUnit = this;

    }
    public void CloseMenu()
    {
        if (unitMenuCanvas != null && unitMenuCanvas.activeSelf)
        {
            unitMenuCanvas.SetActive(false);
            isMenuOpen = false;
            activeMenuUnit = null;
        }
    }


    public void OnRelocateButtonClick()
    {
        CloseMenu();
        RelocationManager.Instance?.StartRelocation(gameObject);
    }

    public void TryMergeUpgrade()
    {
        CloseMenu();
        activeMenuUnit = null;

        if (!CurrentManager.Instance.SubtractCoin(mergeCost))
        {
            Debug.Log("코인이 없어서 업그레이드 안돼요");
            return;
        }

        Unit other = FindAnotherSameLevelUnit();
        if (other == null)
        {
            Debug.Log("같은 레벨의 유닛이 없어요");
            CurrentManager.Instance.AddCoin(mergeCost);
            return;
        }

        if (MapManager.Instance != null)
        {
            var otherCell = MapManager.Instance.GetCellPositionOnGrid(other.transform.position);
            MapManager.Instance.UpdateLogicalGrid(otherCell.x, otherCell.y, 0);
        }

        Vector3 spawnPos = transform.position;
        Quaternion rot = transform.rotation;
        Instantiate(upgradedPrefab, spawnPos, rot);

        Destroy(other.gameObject);
        Destroy(this.gameObject);
    }

    private Unit FindAnotherSameLevelUnit()
    {
        foreach (var u in AllUnits)
        {
            if (u == this)
            {
                continue;
            }

            if (u.level == this.level)
            {
                return u;
            }
        }
        return null;

    }
}
