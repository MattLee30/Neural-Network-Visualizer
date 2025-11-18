// MoveObject.cs
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 dragOffset;

    void OnMouseDown()
    {
        if (GameManager.activeMode == GameManager.Mode.Moving)
        {
            isDragging = true;

            dragOffset = transform.position - GameManager.current.GetMouseWorldPosition();

            Vector3 mouseWorldPos = GameManager.current.GetMouseWorldPosition();
            dragOffset = transform.position - mouseWorldPos;
        }
        else if (GameManager.activeMode == GameManager.Mode.Deleting)
        {
            Destroy(gameObject);
            Debug.Log("Deleted object: " + gameObject.name);
        }
    }

    void OnMouseDrag()
    {
        if (isDragging && GameManager.activeMode == GameManager.Mode.Moving)
        {
            Vector3 pos = GameManager.current.GetMouseWorldPosition() + dragOffset;
            transform.position = BuildingSystem.current.SnapCoordinateToGrid(pos);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
