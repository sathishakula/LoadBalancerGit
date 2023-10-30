using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public GameObject BoxWeightInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void WeightInfo()
    {
        BoxWeightInfo.SetActive(true);
    }

    public void WeightInfoExit()
    {
        BoxWeightInfo.SetActive(false);
    }
}
