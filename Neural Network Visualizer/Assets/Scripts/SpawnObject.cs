using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnDistance = 2.0f;

    void Update() {
        if (GameManager.spawningMode == false) { return; }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, spawnDistance));

            SpawnCube(mousePosition);
        }
    }


    public void SpawnCube(Vector3 mousePosition){
        Instantiate(cubePrefab, mousePosition, Quaternion.identity);
    }

}
