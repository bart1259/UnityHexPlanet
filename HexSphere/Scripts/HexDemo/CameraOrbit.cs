using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float orbitSpeed;
    public float orbitRadius = 20;
    public float smoothness = 1;
    public GameObject origin;
    
    private Vector3 _offset;
    private Vector3 _target;

    // Start is called before the first frame update
    void Start()
    {
        _offset = (transform.position - origin.transform.position).normalized * orbitRadius;
        _target = origin.transform.position + _offset;
        transform.position = _target;
        transform.LookAt(Vector3.zero, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        float yMove = Input.GetAxis("Vertical");
        float xMove = Input.GetAxis("Horizontal");

        if(Input.GetKey(KeyCode.LeftBracket)) {
            orbitRadius -= 0.1f;
        } else if(Input.GetKey(KeyCode.RightBracket)) {
            orbitRadius += 0.1f;
        }

        _offset += (transform.right * xMove * Time.deltaTime * orbitSpeed) + (transform.up * yMove * Time.deltaTime * orbitSpeed);
        _offset = _offset.normalized * orbitRadius;
        _target = origin.transform.position + _offset;
        transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime / smoothness);
        transform.LookAt(origin.transform.position, Vector3.up);
    }
}
