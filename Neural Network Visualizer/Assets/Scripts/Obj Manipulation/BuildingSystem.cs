using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem: MonoBehaviour
{
    public static BuildingSystem current;

    [Header("Grid Settings")]
    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase whiteTile;

    [Header("Preview Settings")]
    [SerializeField] private GameObject previewObject;
    private GameObject activePreviewInstance;

    [Header("Grid Visualization")]
    [SerializeField] private bool showGridLines = true;
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private Material gridLineMaterial;
    [SerializeField] private Color gridLineColor = new Color(0, 0, 0, 0.3f);
    [SerializeField] private float gridLineWidth = 0.02f;

    private GameObject gridLinesContainer;

    private void Start()
    {
        if (showGridLines)
        {
            CreateGridLines();
        }
    }

    private void Awake() {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update() {
        // Handle preview object visibility and position
        if (GameManager.activeMode == GameManager.Mode.Spawning && GameManager.GetActiveSpawnObject() != null)
        {
            // Create preview if it doesn't exist
            if (activePreviewInstance == null && previewObject != null)
            {
                activePreviewInstance = Instantiate(previewObject);
            }

            // Update preview position to follow cursor with snapping
            if (activePreviewInstance != null)
            {
                Vector3 mouseWorldPos = GameManager.current.GetMouseWorldPosition();
                Vector3 snappedPosition = SnapCoordinateToGrid(mouseWorldPos);
                activePreviewInstance.transform.position = snappedPosition;
            }
        }
        else
        {
            // Destroy preview if mode changes or no spawn object selected
            if (activePreviewInstance != null)
            {
                Destroy(activePreviewInstance);
                activePreviewInstance = null;
            }
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position) {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        return grid.GetCellCenterWorld(cellPosition);
    }

    public void handleSpawn(GameObject prefab) {
        Vector3 spawnPosition = SnapCoordinateToGrid(GameManager.current.GetMouseWorldPosition());

        GameObject newObject = Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObject.AddComponent<MoveObject>();
    }

    private void CreateGridLines()
    {
        gridLinesContainer = new GameObject("GridLines");
        gridLinesContainer.transform.SetParent(transform);

        // Create vertical lines
        for (int x = -gridWidth; x <= gridWidth; x++)
        {
            GameObject lineObj = new GameObject($"VerticalLine_{x}");
            lineObj.transform.SetParent(gridLinesContainer.transform);

            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.material = gridLineMaterial;
            line.startColor = gridLineColor;
            line.endColor = gridLineColor;
            line.startWidth = gridLineWidth;
            line.endWidth = gridLineWidth;
            line.positionCount = 2;

            Vector3 start = gridLayout.CellToWorld(new Vector3Int(x, -gridHeight, 0));
            Vector3 end = gridLayout.CellToWorld(new Vector3Int(x, gridHeight, 0));

            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }

        // Create horizontal lines
        for (int y = -gridHeight; y <= gridHeight; y++)
        {
            GameObject lineObj = new GameObject($"HorizontalLine_{y}");
            lineObj.transform.SetParent(gridLinesContainer.transform);

            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.material = gridLineMaterial;
            line.startColor = gridLineColor;
            line.endColor = gridLineColor;
            line.startWidth = gridLineWidth;
            line.endWidth = gridLineWidth;
            line.positionCount = 2;

            Vector3 start = gridLayout.CellToWorld(new Vector3Int(-gridWidth, y, 0));
            Vector3 end = gridLayout.CellToWorld(new Vector3Int(gridWidth, y, 0));

            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }
    }

    public void ToggleGridLines(bool show)
    {
        if (gridLinesContainer != null)
        {
            gridLinesContainer.SetActive(show);
        }
    }
}