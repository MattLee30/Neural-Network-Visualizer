using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Mode { Spawning, Deleting, Moving, None }
    public static Mode activeMode = Mode.None;

    [Header("Spawn Settings")]
    public float spawnDistance = 15.0f;

    // Reference to the active SpawnObject that will handle spawning
    private static SpawnObject activeSpawnObject;

    void Update()
    {
        if (activeMode == Mode.Spawning && Input.GetMouseButtonDown(0))
        {
            // Check if there's an active SpawnObject to handle the spawn
            if (activeSpawnObject != null)
            {
                Vector3 mousePosition = GetMouseWorldPosition(spawnDistance);
                activeSpawnObject.HandleSpawn(mousePosition);
            }
            else
            {
                Debug.LogWarning("No active SpawnObject selected. Click a button to select what to spawn.");
            }
        }
        else if (activeMode == Mode.Deleting && Input.GetMouseButtonDown(0))
        {
            DeleteObjectAtMouse();
        }
    }

    public static void SetActiveSpawnObject(SpawnObject spawnObj)
    {
        activeSpawnObject = spawnObj;
        Debug.Log($"Selected spawn object: {spawnObj.spawnObject.name}");
    }

    public static SpawnObject GetActiveSpawnObject()
    {
        return activeSpawnObject;
    }

    public void SetSpawningMode()
    {
        SetMode(Mode.Spawning);
    }

    public void SetDeletingMode()
    {
        SetMode(Mode.Deleting);
    }

    public void SetMovingMode()
    {
        SetMode(Mode.Moving);
    }

    public void SetMode(Mode newMode)
    {
        if (activeMode == newMode)
        {
            return;
        }
        activeMode = newMode;
        switch (activeMode)
        {
            case Mode.Spawning:
                Debug.Log("Switched to Spawning Mode - Click to spawn objects");
                break;
            case Mode.Deleting:
                Debug.Log("Switched to Deleting Mode - Click to delete objects");
                break;
            case Mode.Moving:
                Debug.Log("Switched to Moving Mode - Drag objects to move them");
                break;
            default:
                Debug.Log("Unknown Mode");
                break;
        }
    }

    private void DeleteObjectAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the MoveObject component (indicating it's a spawned object)
            if (hit.collider.GetComponent<MoveObject>() != null)
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Deleted object: " + hit.collider.name);
            }
        }
    }

    public Vector3 GetMouseWorldPosition(float distance)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distance));
        return mousePosition;
    }

    public Vector3 IsometricCameraPos(float distance)
    {
        return GetMouseWorldPosition(distance);
    }
}