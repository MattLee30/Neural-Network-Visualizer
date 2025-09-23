using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameManager gameManager;

    void OnMouseDown()
    {
        // Handle spawning when this object is clicked and in spawning mode
        if (GameManager.activeMode == GameManager.Mode.Spawning)
        {
            Vector3 mousePosition = gameManager.GetMouseWorldPosition(gameManager.spawnDistance);
            gameManager.SpawnCube(mousePosition);
        }
    }
}