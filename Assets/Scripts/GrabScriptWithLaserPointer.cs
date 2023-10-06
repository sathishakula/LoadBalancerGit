using UnityEngine;

public class GrabScriptWithLaserPointer : MonoBehaviour
{
    public GameObject laserPrefab; // Laser pointer prefab
    public GameObject[] regularPrefabs; // Array of regular prefabs
    public Camera cam;
    private GameObject laserPointer;
    private GameObject heldObject; // Object currently held by the player

    private void Start()
    {
        // Create the laser pointer object at the player's position
        laserPointer = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laserPointer.SetActive(false); // Initially, hide the laser pointer
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject != null)
            {
                PlaceObject();
            }
            else
            {
                TryPickupObject();
            }
        }

        if (heldObject == null)
        {
            UpdateLaserPointer();
        }
    }

    private void UpdateLaserPointer()
    {
        // Raycast from the camera to see what the laser pointer hits
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            laserPointer.SetActive(true);
            laserPointer.transform.position = hit.point;
        }
        else
        {
            laserPointer.SetActive(false);
        }
    }

    private void TryPickupObject()
    {
        // Check if the laser pointer is active and pointing at an object tagged as "Pickupable"
        if (laserPointer.activeSelf)
        {
            RaycastHit hit;
            Ray ray = new Ray(laserPointer.transform.position, transform.forward);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Pickupable"))
                {
                    heldObject = hit.collider.gameObject;
                    heldObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics for the held object
                }
            }
        }
    }

    private void PlaceObject()
    {
        // Check if the laser pointer is active
        if (laserPointer.activeSelf)
        {
            // Create a ray from the laser pointer position forward
            Ray ray = new Ray(laserPointer.transform.position, transform.forward);
            RaycastHit hit;

            // Check if the ray hits something in the scene
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is a valid placement surface
                if (hit.collider.CompareTag("PlacementSurface"))
                {
                    // Calculate the placement position slightly above the hit point
                    Vector3 placementPosition = hit.point + Vector3.up * 0.1f;

                    // Enable physics for the held object
                    heldObject.GetComponent<Rigidbody>().isKinematic = false;

                    // Set the position of the held object to the placement position
                    heldObject.transform.position = placementPosition;

                    // Reset the held object reference
                    heldObject = null;
                }
            }
        }
    }
}
