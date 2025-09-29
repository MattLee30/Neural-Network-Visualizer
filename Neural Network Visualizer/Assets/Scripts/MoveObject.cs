// MoveObject.cs
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        if (GameManager.activeMode == GameManager.Mode.Moving)
        {
            isDragging = true;
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            dragOffset = transform.position - mouseWorldPos;
        }
        else 
        if (GameManager.activeMode == GameManager.Mode.Deleting)
        {
            Destroy(gameObject);
            Debug.Log("Deleted object: " + gameObject.name);
        }
    }

    void OnMouseDrag()
    {
        if (isDragging && GameManager.activeMode == GameManager.Mode.Moving)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            transform.position = mouseWorldPos + dragOffset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
