using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public enum Mode { Spawning, Deleting, Moving, None }
    public static Mode activeMode = Mode.None;

    [Header("Spawn Settings")]
    public float spawnDistance = 15.0f;

    private static GameObject activeSpawnObject;

    private void Awake() {
        current = this;
    }

    void Update()
    {
        if (activeMode == Mode.Spawning && Input.GetMouseButtonDown(0))
        {
            // Check if there's an active SpawnObject to handle the spawn
            if (activeSpawnObject != null)
            {
                Vector3 mousePosition = GetMouseWorldPosition();
                BuildingSystem.current.handleSpawn(activeSpawnObject);
                // SpawnObject.handleSpawn(activeSpawnObject);
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

    public static void SetActiveSpawnObject(GameObject spawnObj)
    {
        activeSpawnObject = spawnObj;
        Debug.Log($"Selected spawn object: {spawnObj.name}");
    }

    public static GameObject GetActiveSpawnObject()
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

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    
}