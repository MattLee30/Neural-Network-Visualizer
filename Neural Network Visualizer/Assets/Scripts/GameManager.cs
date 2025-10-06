using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public enum Mode { Spawning, Deleting, Moving, None }
    public static Mode activeMode = Mode.None;

    [Header("Spawn Settings")]
    public float spawnDistance = 15.0f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float rotationDuration = 0.5f; // Duration of rotation in seconds
    public float movementDuration = 0.3f; // Duration of movement in seconds
    public float movementIncrement = 2.5f; // Distance to move per key press

    private static GameObject activeSpawnObject;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isRotating = false;
    private float rotationProgress = 0f;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float movementProgress = 0f;

    private void Awake() {
        current = this;
        
        // Auto-assign main camera if not set
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        targetRotation = cameraTransform.rotation;
        targetPosition = cameraTransform.position;
    }

    void Update()
    {
        // Handle camera rotation
        HandleCameraRotation();
        
        // Handle camera movement
        HandleCameraMovement();

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

    private void HandleCameraMovement()
    {
        // Only accept new movement input if not currently moving
        if (!isMoving)
        {
            Vector3 moveDirection = Vector3.zero;
            
            // Get camera's forward and right directions (projected on XZ plane)
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();
            
            Vector3 right = cameraTransform.right;
            right.y = 0;
            right.Normalize();
            
            if (Input.GetKeyDown(KeyCode.W))
            {
                moveDirection = forward * movementIncrement;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                moveDirection = -forward * movementIncrement;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                moveDirection = -right * movementIncrement;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                moveDirection = right * movementIncrement;
            }
            
            // If a direction was pressed, start moving
            if (moveDirection != Vector3.zero)
            {
                startPosition = cameraTransform.position;
                targetPosition = startPosition + moveDirection;
                isMoving = true;
                movementProgress = 0f;
            }
        }
        
        // Smoothly interpolate to target position
        if (isMoving)
        {
            movementProgress += Time.deltaTime / movementDuration;
            
            cameraTransform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                movementProgress
            );
            
            // Check if movement is complete
            if (movementProgress >= 1f)
            {
                cameraTransform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    private void HandleCameraRotation()
    {
        // Rotate camera left (Q key)
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            startRotation = cameraTransform.rotation;
            // Rotate around world Y-axis (up) to maintain isometric angle
            targetRotation = Quaternion.Euler(0, -90, 0) * startRotation;
            isRotating = true;
            rotationProgress = 0f;
        }
        // Rotate camera right (E key)
        else if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            startRotation = cameraTransform.rotation;
            // Rotate around world Y-axis (up) to maintain isometric angle
            targetRotation = Quaternion.Euler(0, 90, 0) * startRotation;
            isRotating = true;
            rotationProgress = 0f;
        }

        // Smoothly interpolate to target rotation
        if (isRotating)
        {
            rotationProgress += Time.deltaTime / rotationDuration;
            
            cameraTransform.rotation = Quaternion.Slerp(
                startRotation, 
                targetRotation, 
                rotationProgress
            );

            // Check if rotation is complete
            if (rotationProgress >= 1f)
            {
                cameraTransform.rotation = targetRotation;
                isRotating = false;
                Debug.Log("Rotation complete!");
            }
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