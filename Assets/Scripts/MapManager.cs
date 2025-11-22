using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private string placementTag = "only Unit";
    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private float raycastDistance = 10f;

    public const int MAP_WIDTH = 7;
    public const int MAP_HEIGHT = 11;

    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private Vector3 mapOrigin = Vector3.zero;
    [SerializeField] private GameObject unitPrefab;

    private int[,] logicalGrid = new int[MAP_HEIGHT, MAP_WIDTH];

    public List<Vector2Int> monsterPath = new List<Vector2Int>();

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

    public void Start()
    {
        for (int y = 0; y < MAP_HEIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                logicalGrid[y, x] = 0;
            }
        }
    }

    public Vector3 GetWorldCenterForCell(int x, int y)
    {
        float worldX = mapOrigin.x + (x * cellSize) + (cellSize / 2f);
        float worldY = mapOrigin.y + (y * cellSize) + (cellSize / 2f);

        return new Vector3(worldX, worldY, mapOrigin.z);
    }

    public Vector2Int GetCellPositionOnGrid(Vector3 worldPosition)
    {
        float relativeX = worldPosition.x - mapOrigin.x;
        float relativeY = worldPosition.y - mapOrigin.y;

        int cellX = Mathf.FloorToInt(relativeX / cellSize);
        int cellY = Mathf.FloorToInt(relativeY / cellSize);

        return new Vector2Int(cellX, cellY);
    }

    private void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsGameOver)
        {
            return;
        }

        if (RelocationManager.Instance != null && RelocationManager.Instance.IsRelocating)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int cellPos = GetCellPositionOnGrid(mouseWorldPos);
            TryPlaceUnit(cellPos.x, cellPos.y);
        }
    }
    public bool IsPlacementValid(Vector3 worldPosition)
    {
        Debug.DrawRay(worldPosition, Vector2.down * raycastDistance, Color.red, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.down, raycastDistance, placementLayer);

        if (hit.collider == null)
        {
            return false;
        }
        if (!hit.collider.CompareTag(placementTag))
        {
            return true;
        }

        Debug.Log(" 배치 가능 ");
        return true;
    }


    public void TryPlaceUnit(int x, int y)
    {

        if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
        {
            Debug.Log("맵 바깥을 클릭함");
            return;
        }

        if (logicalGrid[y, x] != 0)
        {
            Debug.Log($"셀 ({x},{y})는 이미 배치된 타일입니다");
            return;
        }

        Vector3 worldPos = GetWorldCenterForCell(x, y);
        Vector2Int cellPos = new Vector2Int(x, y);

        if (!IsPlacementValid(worldPos))
        {
            return;
        }

        if (monsterPath.Contains(cellPos))
        {
            return;
        }

        if (CurrentManager.Instance.SubtractCoin(20) == false)
        {
            Debug.Log("코인이 부족해요");
            return;
        }

        Instantiate(unitPrefab, worldPos, Quaternion.identity);
        logicalGrid[y, x] = 1;

    }

    public void UpdateLogicalGrid(int x, int y, int value)
    {
        if (x >= 0 && x < MAP_WIDTH && y >= 0 && y < MAP_HEIGHT)
        {
            logicalGrid[y, x] = value;
        }
    }

    public bool IsOnMonsterPath(Vector3 worldPosition)
    {
        Vector2Int cellPos = GetCellPositionOnGrid(worldPosition);

        if (cellPos.x < 0 || cellPos.x >= MAP_WIDTH || cellPos.y < 0 || cellPos.y >= MAP_HEIGHT)
        {
            return false;
        }

        return monsterPath.Contains(cellPos);

    }





    public int GetLogicalGridValue(int x, int y)
    {
        if (x >= 0 && x < MAP_WIDTH && y >= 0 && y < MAP_HEIGHT)
        { 
            return logicalGrid[y,x];
        }
        return -1;
    }


    // 디버그
    private void OnDrawGizmos()
    {
        // 1. 격자 선의 색상 설정 (눈에 잘 띄는 색으로)
        Gizmos.color = Color.green;

        // 2. 맵의 전체 범위(MAP_WIDTH, MAP_HEIGHT)를 순회합니다.
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                // 3. 해당 셀의 월드 좌표 중앙점을 계산합니다.
                //    MapManager 내부에 이미 구현된 함수를 사용합니다.
                Vector3 center = GetWorldCenterForCell(x, y);

                // 4. 격자 셀의 윤곽선을 그립니다.
                //    cellSize 크기의 상자를 중앙점(center)에 그립니다.
                Vector3 size = new Vector3(cellSize, cellSize, 0.1f);

                // 유닛이 배치된 셀은 다른 색으로 표시할 수도 있습니다.
                // if (logicalGrid[y, x] == 1) Gizmos.color = Color.red; 

                // 윤곽선만 그리기 (상자를 채우지 않음)
                Gizmos.DrawWireCube(center, size);

                // if (logicalGrid[y, x] == 1) Gizmos.color = Color.green; // 색상 초기화
            }
        }

        // 5. 몬스터 경로 표시 (선택 사항)
        if (monsterPath != null && monsterPath.Count > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < monsterPath.Count - 1; i++)
            {
                Vector2Int currentCell = monsterPath[i];
                Vector2Int nextCell = monsterPath[i + 1];

                Vector3 currentWorldPos = GetWorldCenterForCell(currentCell.x, currentCell.y);
                Vector3 nextWorldPos = GetWorldCenterForCell(nextCell.x, nextCell.y);

                // 두 셀의 중앙점을 연결하는 선을 그립니다.
                Gizmos.DrawLine(currentWorldPos, nextWorldPos);
            }
        }
    }
}

