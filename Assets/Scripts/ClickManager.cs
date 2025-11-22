using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public LayerMask unitLayer;
    public LayerMask groundLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Unit.activeMenuUnit != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Collider2D unitcol = Physics2D.OverlapPoint(mousePos, unitLayer);
        if (unitcol != null)
        {
            Unit clickedUnit = unitcol.GetComponentInParent<Unit>();
            if (clickedUnit != null)
            {
                HandleUnitClick(clickedUnit);
                return;
            }
        }

        Collider2D groundCol = Physics2D.OverlapPoint(mousePos, groundLayer);
        if (groundCol != null)
        {
            HandleBackgroundClick();
        }
        
    }

    private void HandleUnitClick(Unit clickedUnit)
    {
        if (Unit.activeMenuUnit != null && Unit.activeMenuUnit != clickedUnit)
        {
            Unit.activeMenuUnit.CloseMenu();
        }
        else if (Unit.activeMenuUnit == clickedUnit)
        {
            clickedUnit.CloseMenu();
            return;
        }

        clickedUnit.OpenMenu();
    }

    private void HandleBackgroundClick()
    {
        if (Unit.activeMenuUnit != null)
        {
            Unit.activeMenuUnit.CloseMenu();
        }
    }
}
