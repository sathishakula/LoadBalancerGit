using UnityEngine;
using System.Collections.Generic;

public class PivotLineManager : MonoBehaviour
{
    public List<GameObject> placedObjects; // List to store the placed objects

    private void Start()
    {
        placedObjects = new List<GameObject>();
    }

    private void Update()
    {
        // Check the relative positions of placed objects and adjust the pivot line
        UpdatePivotLinePosition();
    }

    private void UpdatePivotLinePosition()
    {
        if (placedObjects.Count == 0)
        {
            // No objects placed, no need to update the pivot line
            return;
        }

        // Calculate the center position of all placed objects
        Vector3 centerPosition = Vector3.zero;
        foreach (GameObject obj in placedObjects)
        {
            centerPosition += obj.transform.position;
        }
        centerPosition /= placedObjects.Count;

        // Move the pivot line toward the side with more objects
        float leftTotal = 0f;
        float rightTotal = 0f;
        foreach (GameObject obj in placedObjects)
        {
            if (obj.transform.position.z < centerPosition.z)
            {
                leftTotal++;
            }
            else
            {
                rightTotal++;
            }
        }

        if (leftTotal > rightTotal)
        {
            // Move the pivot line toward the left (decrement its Z-position)
            transform.position = new Vector3(transform.position.x, transform.position.y, centerPosition.z - 1.0f);
        }
        else if (rightTotal > leftTotal)
        {
            // Move the pivot line toward the right (increment its Z-position)
            transform.position = new Vector3(transform.position.x, transform.position.y, centerPosition.z + 1.0f);
        }
    }


}
