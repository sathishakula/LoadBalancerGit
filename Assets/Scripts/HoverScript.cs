using UnityEngine;

public class HoverScript : MonoBehaviour
{
    public Camera cam;
    public GameObject centerLine;
    private GameObject currentHoverObject;

    private const float gravity = 9.81f; // Earth's gravity in m/s^2
    private GUIStyle guiStyle = new GUIStyle();

    void Update()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Check if the hovered object has the "PlacedObject" tag
            if (hitObject.tag == "PlacedObject")
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
            Vector3 screenPos = cam.WorldToScreenPoint(currentHoverObject.transform.position);
            float distanceFromCenter = Vector3.Distance(centerLine.transform.position, currentHoverObject.transform.position);
            float weight = ParseWeightFromName(currentHoverObject.name);
            float force = weight * gravity; // Force due to gravity
            float pivotMoment = distanceFromCenter * force; // Torque calculation

            string infoText = $"Distance from center: {distanceFromCenter:F2}\nWeight: {weight}\nPivot Moment: {pivotMoment:F2}";
            guiStyle.fontSize = 20;
            guiStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y - 100, 200, 100), infoText, guiStyle);
        }
    }

    float ParseWeightFromName(string name)
    {
        // Extracting the weight (mass) from the GameObject's name
        string numberString = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
        return float.TryParse(numberString, out float weight) ? weight : 0;
    }
}
