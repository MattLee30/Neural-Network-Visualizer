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

        // Get the bounds of the object to spawn
        Bounds bounds = GetObjectBounds(spawnObject);
        float objectHeight = bounds.size.y;
        float objectRadius = Mathf.Max(bounds.size.x, bounds.size.z) / 2f;

        // Raycast downward to find the ground
        Vector3 adjustedPosition = position;
        Ray downRay = new Ray(position + Vector3.up * 100f, Vector3.down);
        RaycastHit groundHit;

        if (Physics.Raycast(downRay, out groundHit, Mathf.Infinity))
        {
            // Position the object on top of the ground
            adjustedPosition = groundHit.point + Vector3.up * (objectHeight / 2f);
        }
        else
        {
            // If no ground found, place at y = objectHeight/2 to avoid being below y=0
            adjustedPosition.y = Mathf.Max(position.y, objectHeight / 2f);
        }

        // Check if there's already an object at this location
        Collider[] overlaps = Physics.OverlapSphere(adjustedPosition, objectRadius);

        if (overlaps.Length > 0)
        {
            // There are objects nearby, try to find a clear spot
            bool foundClearSpot = false;
            float searchRadius = objectRadius * 2f;
            int maxAttempts = 8;

            for (int i = 0; i < maxAttempts; i++)
            {
                float angle = (360f / maxAttempts) * i;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * searchRadius,
                    0f,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * searchRadius
                );

                Vector3 testPosition = adjustedPosition + offset;

                // Raycast down from the test position to find ground
                Ray testRay = new Ray(testPosition + Vector3.up * 100f, Vector3.down);
                RaycastHit testGroundHit;

                if (Physics.Raycast(testRay, out testGroundHit, Mathf.Infinity))
                {
                    testPosition = testGroundHit.point + Vector3.up * (objectHeight / 2f);
                }

                Collider[] testOverlaps = Physics.OverlapSphere(testPosition, objectRadius * 0.9f);

                if (testOverlaps.Length == 0)
                {
                    adjustedPosition = testPosition;
                    foundClearSpot = true;
                    break;
                }
            }

            if (!foundClearSpot)
            {
                Debug.LogWarning("Could not find clear spot to spawn object. Spawning anyway with slight overlap.");
            }
        }

        GameObject newObject = Instantiate(spawnObject, adjustedPosition, Quaternion.identity);

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

        Debug.Log($"Spawned {spawnObject.name} at position: {adjustedPosition}");
    }

    private Bounds GetObjectBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

        // Try to get bounds from the prefab
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else
        {
            // If no renderer, try to get from collider
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                bounds = collider.bounds;
            }
        }

        return bounds;
    }
}