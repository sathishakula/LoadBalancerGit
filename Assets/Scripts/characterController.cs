using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController : MonoBehaviour
{

    public float _speed = 10;
    public float _rotationSpeed = 180;



    private Vector3 rotation;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * _rotationSpeed * Time.deltaTime, 0);


        Vector3 move = new Vector3(0, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);
        //  move = this.transform.TransformDirection(move);
        // _controller.Move(move * _speed);
        this.transform.Rotate(this.rotation);

        // Debug.Log(Input.GetAxisRaw("Horizontal"));
        this.rotation = new Vector3(0, 0, Input.GetAxisRaw("Horizontal") * -1 * _rotationSpeed * Time.deltaTime);
    }
}