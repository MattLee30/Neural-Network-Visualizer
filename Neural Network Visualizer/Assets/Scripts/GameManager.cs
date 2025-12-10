using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public enum Mode { Spawning, Deleting, Moving, Editing, None }
    public static Mode activeMode = Mode.None;

    [Header("Spawn Settings")]
    public float spawnDistance = 15.0f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float rotationDuration = 0.5f; 
    public float movementDuration = 0.3f; 
    public float movementIncrement = 2.5f; 

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
            if (activeSpawnObject != null)
            {
                Vector3 mousePosition = GetMouseWorldPosition();
                BuildingSystem.current.handleSpawn(activeSpawnObject);
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
        else if (activeMode == Mode.Editing && Input.GetMouseButtonDown(0))
        {
            EditObjectAtMouse();
        }
    }

    private void HandleCameraMovement()
    {
        if (!isMoving)
        {
            Vector3 moveDirection = Vector3.zero;
            
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
            
            if (moveDirection != Vector3.zero)
            {
                startPosition = cameraTransform.position;
                targetPosition = startPosition + moveDirection;
                isMoving = true;
                movementProgress = 0f;
            }
        }
        
        if (isMoving)
        {
            movementProgress += Time.deltaTime / movementDuration;
            
            cameraTransform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                movementProgress
            );
            
            if (movementProgress >= 1f)
            {
                cameraTransform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    private void HandleCameraRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            startRotation = cameraTransform.rotation;
            targetRotation = Quaternion.Euler(0, -90, 0) * startRotation;
            isRotating = true;
            rotationProgress = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            startRotation = cameraTransform.rotation;
            targetRotation = Quaternion.Euler(0, 90, 0) * startRotation;
            isRotating = true;
            rotationProgress = 0f;
        }

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

    public void SetEditingMode()
    {
        SetMode(Mode.Editing);
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
            case Mode.Editing:
                Debug.Log("Switched to Editing Mode - Click Objects to edit them");
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

    private void EditObjectAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the MoveObject component (indicating it's a spawned object)
            if (hit.collider.GetComponent<MoveObject>() != null)
            {
                // Check if it has a Layer component
                Layer layer = hit.collider.GetComponent<Layer>();
                if (layer != null)
                {
                    // Show the editing UI
                    EditingUIManager.current.ShowEditingUI(hit.collider.gameObject);
                }
                else
                {
                    Debug.LogWarning($"Object {hit.collider.name} does not have a Layer component!");
                }
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