using UnityEngine;
using TMPro;

public class GrabScript3 : MonoBehaviour
{
    public GameObject[] transparentPrefabs; // Array of transparent prefabs
    public GameObject[] regularPrefabs; // Array of regular prefabs
    public Camera cam;
    public float distanceFromPlayer = 2.0f; // Adjust the value as needed
    public PivotLineManager pivotLineManager;

    private GameObject transparentObject;
    private GameObject placingObject; // Object that follows the cursor while placing
    private bool isPlacing = false;
    private int selectedPrefabIndex = -1;
    private Vector3 lastTransparentPosition; // Store the last transparent object's position

    public TextMeshProUGUI weightText; 

    private int weight1 = 1;
    private int weight2 = 2;
    private int weight3 = 3;
    private int totalWeight = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isPlacing)
            {
                PlaceObject();
            }
            else
            {
                SelectObject();
            }
        }

        if (isPlacing)
        {
            UpdatePlacingObjectPosition();
            RotatePlacingObject();
        }
    }

    private void RotatePlacingObject()
    {
        // Rotate the transparent object using mouse scroll
        float rotationSpeed = 10f; // Adjust the rotation speed as needed
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Rotate the transparent object around its own up axis (Y-axis) based on the scroll input
        transparentObject.transform.Rotate(Vector3.up, scrollInput * rotationSpeed);
    }

    private void SelectObject()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            for (int i = 0; i < transparentPrefabs.Length; i++)
            {
                if (hit.collider.CompareTag("Selectable" + i))
                {
                    Vector3 spawnPosition = hit.point + (hit.point - cam.transform.position).normalized * distanceFromPlayer;
                    transparentObject = Instantiate(transparentPrefabs[i], spawnPosition, Quaternion.identity);
                    lastTransparentPosition = spawnPosition; // Store the position
                    selectedPrefabIndex = i;

                    // Create the placing object at the hit point to follow the cursor
                    Vector3 placingPosition = spawnPosition;
                   // placingPosition.z = hit.point.z; // Keep the z position the same
                   // placingObject = Instantiate(regularPrefabs[selectedPrefabIndex], placingPosition, Quaternion.identity);
                    //placingObject.SetActive(false);

                    isPlacing = true;
                    break;
                }
            }
        }
    }

    private void UpdatePlacingObjectPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Ground"))
        {
            Vector3 groundNormal = hit.normal;
            Vector3 newPosition = hit.point + groundNormal * 0.1f; // Offset slightly above the ground

            // Rotate the transparent object using mouse scroll
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            transparentObject.transform.Rotate(Vector3.right, scrollInput * 10f);

            transparentObject.transform.position = newPosition;
        }
    }

    private void PlaceObject()
    {
        //placingObject.SetActive(true);

        // Set the position and rotation of the placed object to match the last transparent object's position and rotation
        if (selectedPrefabIndex >= 0 && selectedPrefabIndex < regularPrefabs.Length)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cam.nearClipPlane;
            Quaternion currentRotation = transparentObject.transform.rotation;
            // Destroy the transparent object before instantiating the regular object
            Destroy(transparentObject);

            GameObject placedObject = Instantiate(regularPrefabs[selectedPrefabIndex], transparentObject.transform.position, currentRotation);

            pivotLineManager.placedObjects.Add(placedObject);

            switch(selectedPrefabIndex)
            {
                case 0:
                    totalWeight += weight1;
                    break;
                case 1:
                    totalWeight += weight1;
                    break;
                case 2:
                    totalWeight += weight2;
                    break;
                case 3:
                    totalWeight += weight1;
                    break;
                case 4:
                    totalWeight += weight2;
                    break;
                case 5:
                    totalWeight += weight3; 
                    break;
            }

            weightText.SetText(totalWeight.ToString() + " kg");
           
        }

        
        selectedPrefabIndex = -1;
        isPlacing = false;

        // Set the position of the placing object to match the last transparent object's position
      //  placObject.transform.position = lastTransparentPosition;
        
    }
}
