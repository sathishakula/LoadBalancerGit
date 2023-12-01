using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GrabScript : MonoBehaviour
{
    public GameObject[] transparentPrefabs;
    public GameObject[] regularPrefabs;
    public Camera cam;
    public float distanceFromPlayer = 2.0f;
    public PivotLineManager pivotLineManager;
    public GameObject centerLine; // Reference to the pivot object
    //AGK CHANGES
    // supply count
    public int cleanSupply;
    public int clothes;
    public int firstAid;
    public int food;
    public int kitchenSupply;
    public int vehicle;
    //LER CHANGES
    // total weight and supply weights
    public int weight = 0;
    public const int CLEANSUPPLY_WEIGHT = 200;
    public const int CLOTHES_WEIGHT = 100;
    public const int FIRSTAID_WEIGHT = 200;
    public const int FOOD_WEIGHT = 100;
    public const int KITCHENSUPPLY_WEIGHT = 100;
    public const int VEHICLE_WEIGHT = 300;
    // weight text
    [SerializeField] private TextMeshProUGUI weightText;

    // Check gameobjects
    public GameObject[] Checks;
    



    private GameObject transparentObject;
    private bool isPlacing = false;
    private int selectedPrefabIndex = -1;
    private Vector3 lastTransparentPosition;

    private List<GameObject> placedObjects = new List<GameObject>(); // Track all placed objects

    private class PlacedObjectData
    {
        public GameObject Object;
        public float DistanceFromPivot;
        public float AngleFromPivot;

        public PlacedObjectData(GameObject obj, float distance, float angle)
        {
            Object = obj;
            DistanceFromPivot = distance;
            AngleFromPivot = angle;
        }
    }

    private List<PlacedObjectData> placedObjectsData = new List<PlacedObjectData>(); // Store pivot movement values

    /*  private void Update()
      {
          if (Input.GetMouseButtonDown(0))
          {
              RaycastHit hit;
              Ray ray = cam.ScreenPointToRay(Input.mousePosition);

              if (Physics.Raycast(ray, out hit))
              {
                  GameObject clickedObject = hit.collider.gameObject;

                  // Delete object if it's a placed object and clicked again
                  if (placedObjects.Contains(clickedObject) && !isPlacing)
                  {
                      // Remove from placedObjects and placedObjectsData before destroying
                      placedObjects.Remove(clickedObject);
                      placedObjectsData.RemoveAll(data => data.Object == clickedObject);
                      Destroy(clickedObject);
                  }
                  else if (isPlacing)
                  {
                      Debug.Log("Enetered place object");
                      PlaceObject();
                  }
                  else
                  {
                      SelectObject();
                  }
              }
          }

          if (isPlacing)
          {
              UpdatePlacingObjectPosition();
              RotatePlacingObject();
          }
      }
      */
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                
                if (isPlacing)
                {
                    PlaceObject();
                }
                else
                {
                    SelectObject();
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                 
                if (placedObjects.Contains(clickedObject))
                {
                    RemoveAndDestroyPlacedObject(clickedObject);
                }
            }
        }

        if (isPlacing && transparentObject != null)
        {
            UpdatePlacingObjectPosition();
            RotatePlacingObject();
        }

        // update total weight based on count of each type of object
        weight = (
            cleanSupply * CLEANSUPPLY_WEIGHT +
            clothes * CLOTHES_WEIGHT+
            firstAid * FIRSTAID_WEIGHT +
            food * FOOD_WEIGHT +
            kitchenSupply * KITCHENSUPPLY_WEIGHT +
            vehicle * VEHICLE_WEIGHT
        );

        // update weight text
        weightText.text = weight.ToString() + " kg";

    }

    private void RemoveAndDestroyPlacedObject(GameObject objectToRemove)
    {
        if (objectToRemove != null)
        {
            if (objectToRemove.CompareTag("PlacedObject0"))
            {
                cleanSupply--;}
            else if (objectToRemove.CompareTag("PlacedObject1"))
            {
                clothes--;}
            else if (objectToRemove.CompareTag("PlacedObject2"))
            {
                firstAid--;}
            else if (objectToRemove.CompareTag("PlacedObject3"))
            {
                food--;}
            else if (objectToRemove.CompareTag("PlacedObject4"))
            {
                kitchenSupply--;}
            else if (objectToRemove.CompareTag("PlacedObject5"))
            {
                vehicle--;}
            placedObjects.Remove(objectToRemove);
            placedObjectsData.RemoveAll(data => data.Object == objectToRemove);
            Destroy(objectToRemove);
            requirementCheck();
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
            placedObject.tag = "PlacedObject" + selectedPrefabIndex; // Tagging the object as placed
            placedObjects.Add(placedObject); // Add to placed objects list
            UpdatePivotMovementValues(placedObject); // Calculate pivot movement values

            pivotLineManager.placedObjects.Add(placedObject);

        }

        // Updates Supplies count
        switch (selectedPrefabIndex)
        {
            case 0: cleanSupply++;
                break;
            case 1: clothes++;
                break;
            case 2: firstAid++;
                break;
            case 3: food++;
                break;
            case 4: kitchenSupply++;
                break;
            default: vehicle++;
                break;
                
        }
        selectedPrefabIndex = -1;
        isPlacing = false;
        requirementCheck();

        // Set the position of the placing object to match the last transparent object's position
        //  placObject.transform.position = lastTransparentPosition;

    }

    private void UpdatePivotMovementValues(GameObject placedObject)
    {
        float distance = Vector3.Distance(centerLine.transform.position, placedObject.transform.position);
        Vector3 direction = (placedObject.transform.position - centerLine.transform.position).normalized;
        float angle = Vector3.Angle(centerLine.transform.forward, direction);
        Debug.Log(placedObject.name);
        PlacedObjectData data = new PlacedObjectData(placedObject, distance, angle);
        placedObjectsData.Add(data);
    }

    private void requirementCheck()
    {
        if (vehicle >= 2)
        {
            Checks[0].SetActive(true);
        }
        else
        {
            Checks[0].SetActive(false);
        }
        if (kitchenSupply >= 1)
        {
            Checks[1].SetActive(true);
        }
        else
        {
            Checks[1].SetActive(false);
        }
        if (food >= 3)
        {
            Checks[2].SetActive(true);
        }
        else
        {
            Checks[2].SetActive(false);
        }
        if (firstAid >= 4)
        {
            Checks[3].SetActive(true);
        }
        else
        {
            Checks[3].SetActive(false);
        }
    }
}
