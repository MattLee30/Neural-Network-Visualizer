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

    private void Awake() {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position) {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        return grid.GetCellCenterWorld(cellPosition);
    }

    public void handleSpawn(GameObject prefab) {
        // Vector3 spawnPosition = SnapCoordinateToGrid(Vector3.zero);
        Vector3 spawnPosition = SnapCoordinateToGrid(GameManager.current.GetMouseWorldPosition());

        GameObject newObject = Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObject.AddComponent<MoveObject>();
    }
}
