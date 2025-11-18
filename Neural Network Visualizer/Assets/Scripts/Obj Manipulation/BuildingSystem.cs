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
}