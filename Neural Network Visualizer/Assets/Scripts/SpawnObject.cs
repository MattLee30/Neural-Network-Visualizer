using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject spawnObject;

    // Remove Start method - buttons will now call SelectThisSpawner() directly

    void OnMouseDown()
    {
        // When this button is clicked, select it as the active spawner
        SelectThisSpawner();
    }

    // Call this method from Unity Button onClick event
    public void SelectThisSpawner()
    {
        GameManager.SetActiveSpawnObject(this);

        // Optionally switch to spawning mode when a button is clicked
        if (gameManager != null && GameManager.activeMode != GameManager.Mode.Spawning)
        {
            gameManager.SetSpawningMode();
        }
    }

    public void HandleSpawn(Vector3 position)
    {
        if (spawnObject == null)
        {
            Debug.LogWarning("No spawn object assigned to SpawnObject component!");
            return;
        }

        GameObject newObject = Instantiate(spawnObject, position, Quaternion.identity);

        // Make sure the spawned object has the MoveObject script for drag functionality
        if (newObject.GetComponent<MoveObject>() == null)
        {
            newObject.AddComponent<MoveObject>();
        }

        // Ensure the object has a collider for mouse detection
        if (newObject.GetComponent<Collider>() == null)
        {
            newObject.AddComponent<BoxCollider>();
        }

        Debug.Log($"Spawned {spawnObject.name} at position: {position}");
    }
}