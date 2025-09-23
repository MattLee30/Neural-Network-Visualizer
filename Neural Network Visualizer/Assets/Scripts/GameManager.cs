using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Mode { Spawning, Deleting, Moving }
    public static Mode activeMode = Mode.Spawning;

    [Header("Spawn Settings")]
    public GameObject cubePrefab;
    public float spawnDistance = 2.0f;
    
    [Header("UI References")]
    public SpawnObject spawnObjectScript;

    void Update()
    {
        if (activeMode == Mode.Spawning && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = GetMouseWorldPosition(spawnDistance);
            SpawnCube(mousePosition);
        }
        else if (activeMode == Mode.Deleting && Input.GetMouseButtonDown(0))
        {
            DeleteObjectAtMouse();
        }
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
                Debug.Log("Switched to Spawning Mode - Click to spawn cubes");
                break;
            case Mode.Deleting:
                Debug.Log("Switched to Deleting Mode - Click to delete cubes");
                break;
            case Mode.Moving:
                Debug.Log("Switched to Moving Mode - Drag cubes to move them");
                break;
            default:
                Debug.Log("Unknown Mode");
                break;
        }
    }

    public void SpawnCube(Vector3 position)
    {
        GameObject newCube = Instantiate(cubePrefab, position, Quaternion.identity);
        
        // Make sure the spawned cube has the MoveObject script for drag functionality
        if (newCube.GetComponent<MoveObject>() == null)
        {
            newCube.AddComponent<MoveObject>();
        }
        
        // Ensure the cube has a collider for mouse detection
        if (newCube.GetComponent<Collider>() == null)
        {
            newCube.AddComponent<BoxCollider>();
        }

        Debug.Log($"Spawned cube at position: {position}");
    }

    private void DeleteObjectAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the MoveObject component (indicating it's a spawned cube)
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