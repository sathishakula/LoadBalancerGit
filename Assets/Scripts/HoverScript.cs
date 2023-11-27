using System.Collections.Generic;
using UnityEngine;

public class HoverScript : MonoBehaviour
{
    public Camera cam;
    public GameObject centerLine;
    private GameObject currentHoverObject;
    private List<GameObject> placedObjects = new List<GameObject>();

    private const float gravity = 9.81f; // Earth's gravity in m/s^2
    private GUIStyle guiStyle = new GUIStyle();

    void Update()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        UpdatePlacedObjectsList(); // Update the list of placed objects

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (IsPlacedObject(hitObject))
            {
                currentHoverObject = hitObject;
            }
            else
            {
                currentHoverObject = null;
            }
        }
        else
        {
            currentHoverObject = null;
        }
    }

    void OnGUI()
    {
        if (currentHoverObject != null)
        {
            DisplayObjectInfo(currentHoverObject);
        }

        DisplayTotalMoment();
    }

    private void DisplayObjectInfo(GameObject obj)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(obj.transform.position);
        float distanceFromCenter = obj.transform.position.z - centerLine.transform.position.z;
        float weight = ParseWeightFromName(obj.name);
        float force = weight * gravity;
        float pivotMoment = distanceFromCenter * force;

        string infoText = $"Distance from center: {distanceFromCenter:F2}m\nWeight: {weight}kg\nPivot Moment: {pivotMoment:F2}Nm";
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y - 100, 200, 100), infoText, guiStyle);
    }

    private void DisplayTotalMoment()
    {
        float totalMoment = CalculateTotalMoment();
        Debug.Log($"Total Moment Calculated: {totalMoment}"); // Debugging line
        GUI.Label(new Rect(10, 10, 200, 50), $"Total Moment: {totalMoment:F2}Nm", guiStyle);
    }

    private float CalculateTotalMoment()
    {
        float totalMoment = 0f;
        Debug.Log($"Calculating Total Moment for {placedObjects.Count} objects"); // Debugging line
        foreach (var obj in placedObjects)
        {
            float distanceFromCenter = obj.transform.position.z - centerLine.transform.position.z;
            float weight = ParseWeightFromName(obj.name);
            float force = weight * gravity;
            totalMoment += distanceFromCenter * force;
            Debug.Log($"Object: {obj.name}, Moment: {distanceFromCenter * force}"); // Debugging line
        }
        return totalMoment;
    }

    private void UpdatePlacedObjectsList()
    {
        placedObjects.Clear();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (IsPlacedObject(obj))
            {
                placedObjects.Add(obj);
                Debug.Log($"Added Object: {obj.name}"); // Debugging line
            }
        }
    }
    private bool IsPlacedObject(GameObject obj)
    {
        return obj.tag.StartsWith("PlacedObject");
    }

    private float ParseWeightFromName(string name)
    {
        string numberString = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
        return float.TryParse(numberString, out float weight) ? weight : 0;
    }
}
