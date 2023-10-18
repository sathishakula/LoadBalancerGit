


using UnityEngine;

public class GrabScript: MonoBehaviour
{
    public GameObject[] transparentPrefabs; // Array of transparent prefabs
    public GameObject[] regularPrefabs; // Array of regular prefabs
    public Camera cam;
    private GameObject transparentObject;
    private GameObject placingObject; // Object that follows the cursor while placing
    private bool isPlacing = false;
    private int selectedPrefabIndex = -1;
    public float distanceFromPlayer = 2.0f; // Adjust the value as needed
    public float distanceFromPlayerWhilePlacing = 2.0f; // Adjust the value as needed
    public PivotLineManager pivotLineManager;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("mouse clicked");
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
            Debug.Log("RaycastHit");
            for (int i = 0; i < transparentPrefabs.Length; i++)
            {
                if (hit.collider.CompareTag("Selectable" + i)) // Use "Selectable0", "Selectable1", etc.
                {
                    Vector3 spawnPosition = hit.point + (hit.point - cam.transform.position).normalized * distanceFromPlayer;
                    transparentObject = Instantiate(transparentPrefabs[i], spawnPosition, Quaternion.identity);
                    selectedPrefabIndex = i;

                    // Create the placing object at the hit point to follow the cursor
                    Vector3 placingPosition = spawnPosition;
                    placingPosition.z = hit.point.z; // Keep the z position the same
                    placingObject = Instantiate(regularPrefabs[selectedPrefabIndex], placingPosition, Quaternion.identity);
                    placingObject.SetActive(false);

                    isPlacing = true;
                    break;
                }
            }
        }
    }


    /*
        private void UpdatePlacingObjectPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = distanceFromPlayerWhilePlacing; // Set the distance from the player as the z position
            Vector3 newPosition = cam.ScreenToWorldPoint(mousePosition);

            // Adjust the placing object's distance from the player while placing
            Vector3 playerToCursor = newPosition - cam.transform.position;
            playerToCursor.Normalize();
            newPosition = cam.transform.position + playerToCursor * distanceFromPlayerWhilePlacing;

            newPosition.z = placingObject.transform.position.z; // Maintain the z position

            // Rotate the transparent object using mouse scroll
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            transparentObject.transform.Rotate(Vector3.right, scrollInput * 10f); // Adjust the rotation speed as needed

            transparentObject.transform.position = newPosition;
        }
    */



    private void UpdatePlacingObjectPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Find the ground plane (you may need to specify the layer or tag of your ground objects)
            if (hit.collider.CompareTag("Ground"))
            {
                // Calculate the new position on the ground plane
                Vector3 groundNormal = hit.normal; // The normal of the ground
                Vector3 newPosition = hit.point + groundNormal * 0.1f; // Offset slightly above the ground

                // Rotate the transparent object using mouse scroll
                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                transparentObject.transform.Rotate(Vector3.right, scrollInput * 10f); // Adjust the rotation speed as needed

                transparentObject.transform.position = newPosition;
            }
        }
    }






    /*
        private void PlaceObject()
        {
            placingObject.SetActive(true);
            placingObject.transform.position = transparentObject.transform.position; // Set the position
            placingObject.transform.localScale = transparentObject.transform.localScale; // Set the scale
            Destroy(transparentObject);

            if (selectedPrefabIndex >= 0 && selectedPrefabIndex < regularPrefabs.Length)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = cam.nearClipPlane;
                Instantiate(regularPrefabs[selectedPrefabIndex], cam.ScreenToWorldPoint(mousePosition), Quaternion.identity);
            }

            selectedPrefabIndex = -1;
            isPlacing = false;
        }
    */
    private void PlaceObject()
    {
        placingObject.SetActive(true);
        placingObject.transform.position = transparentObject.transform.position; // Set the position

        // Get the current rotation of the transparent object




        if (selectedPrefabIndex >= 0 && selectedPrefabIndex < regularPrefabs.Length)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cam.nearClipPlane;
            Quaternion currentRotation = transparentObject.transform.rotation;
            // Apply the rotation to the placed object when instantiating
            GameObject placedObject = Instantiate(regularPrefabs[selectedPrefabIndex], cam.ScreenToWorldPoint(mousePosition), currentRotation);

            // Add the placed object to the PivotLineManager's list
            pivotLineManager.placedObjects.Add(placedObject);
        }
        Destroy(transparentObject);
        selectedPrefabIndex = -1;
        isPlacing = false;
    }


}
