using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Button placeButton;

    private Vector2Int pendingCell;
    private bool hasPendingCell = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        popupRoot.SetActive(false);
        placeButton.onClick.AddListener(OnClickPlace);
    }

    public void Show(Vector2Int cell, Vector3 worldPos)
    {
        pendingCell = cell;
        hasPendingCell = true;

        popupRoot.SetActive(true);

       popupRoot.transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }

    public void Hide()
    {
        popupRoot.SetActive(false);
        hasPendingCell = false;
    }

    private void OnClickPlace()
    {
        if (!hasPendingCell)
        {
            return;
        }

        MapManager.Instance.TryPlaceUnit(pendingCell.x, pendingCell.y);
        Hide();
    }
}
