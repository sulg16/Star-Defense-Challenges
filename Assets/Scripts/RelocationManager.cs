using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class RelocationManager : MonoBehaviour
{
    public static RelocationManager Instance { get; private set; }
    public bool IsRelocating = false;
    private GameObject unitToRelocate = null;
    private Vector3 originalPosition;

    public LayerMask groundLayer;
    private Camera mainCamera;

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

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void StartRelocation(GameObject unit)
    {

        if (IsRelocating)
        {
            return;
        }

        unitToRelocate = unit;

        if (MapManager.Instance != null)
        {
            Vector2Int currentCell = MapManager.Instance.GetCellPositionOnGrid(unit.transform.position);
            Vector3 worldCenter = MapManager.Instance.GetWorldCenterForCell(currentCell.x, currentCell.y);
            worldCenter.z = unit.transform.position.z;
            originalPosition = worldCenter;
        }
        else
        { 
            originalPosition = unit.transform.position;
        }

        IsRelocating = true;

        Unit unitComponent = unitToRelocate.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.CloseMenu();
        }
        Unit.activeMenuUnit = null;

        unitToRelocate.transform.position += Vector3.up * 0.5f;
    }

    private void Update()
    {

        if (!IsRelocating || unitToRelocate == null)
        {
            return;
        }

        Vector3 targetPos = GetSnappedMouseWorldPosition();
        unitToRelocate.transform.position = targetPos;

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceUnit(targetPos);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            EndRelocation(false, originalPosition);
        }
    }

    private Vector3 GetSnappedMouseWorldPosition()
    {
        Vector3 screenPoint = Input.mousePosition;

        screenPoint.z = Mathf.Abs(mainCamera.transform.position.z + unitToRelocate.transform.position.z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);

        if (MapManager.Instance != null)
        {
            Vector2Int cellPos = MapManager.Instance.GetCellPositionOnGrid(worldPoint);
            Vector3 snappedWorldCenter = MapManager.Instance.GetWorldCenterForCell(cellPos.x, cellPos.y);
            snappedWorldCenter.z = unitToRelocate.transform.position.z;

            return snappedWorldCenter;
        }

        return unitToRelocate.transform.position;
    }

    private void TryPlaceUnit(Vector3 targetPos)
    {
        if (IsValidPlacement(targetPos))
        {
            EndRelocation(true, targetPos);
        }
        else
        {
            EndRelocation(false, originalPosition);
        }
    }

    private bool IsValidPlacement(Vector3 targetPos)
    {
        if (MapManager.Instance == null || !MapManager.Instance.IsPlacementValid(targetPos))
        {
            return false;
        }

        if (MapManager.Instance.IsOnMonsterPath(targetPos))
        {
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPos, 0.1f);

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject == unitToRelocate)
            {
                continue;
            }

            if (col.GetComponent<Unit>() != null)
            {
                return false;
            }
        }

        return true;
    }

    private void EndRelocation(bool success, Vector3 finalPos)
    {
        if (unitToRelocate != null)
        {
            unitToRelocate.transform.position = finalPos;

            if (success)
            {
                if (MapManager.Instance != null)
                {
                    Vector2Int oldCell = MapManager.Instance.GetCellPositionOnGrid(originalPosition);
                    MapManager.Instance.UpdateLogicalGrid(oldCell.x, oldCell.y, 0);

                    Vector2Int newCell = MapManager.Instance.GetCellPositionOnGrid(finalPos);
                    MapManager.Instance.UpdateLogicalGrid(newCell.x, newCell.y, 1);

                    int newGridValue = MapManager.Instance.GetLogicalGridValue(newCell.x, newCell.y);
                    Debug.Log($"[Relocation Success] New Cell ({newCell.x}, {newCell.y}) updated to: {newGridValue}");
                }
            }
        }

        unitToRelocate = null;
        IsRelocating = false;

        Unit.activeMenuUnit = null;
    }


}
