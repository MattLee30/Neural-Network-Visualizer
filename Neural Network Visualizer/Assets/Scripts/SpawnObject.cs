using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public void handleSpawn(GameObject prefab) {
        Vector3 spawnPosition = BuildingSystem.current.SnapCoordinateToGrid(Vector3.zero);

        GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newObject.AddComponent<MoveObject>();
    }
}