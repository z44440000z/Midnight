// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
using UnityEngine;
using System.Collections;

// Place the script in the Camera-Control group in the component menu
[AddComponentMenu("Camera-Control/Smooth Follow CSharp")]

public class SmoothFollowCSharp : MonoBehaviour
{
    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 5.0f;
    // the height we want the camera to be above the target
    public float maxdistance = 10.0f;
    // How much we 
    public float mindistance = 0.0f;

    float z = 500;

    public float ScrollWheelspeed = 60.0f;
    //public float horizontalRotation = 5.0f;

    public float sensitivity = 0.1f;

    public float x;
    public float y;

    Vector3 offset;

    Quaternion rotationEuler;

    private bool Rhit;

    private float h;

    void Start()
    {
        target = GameObject.Find("camera_t").transform;
    }
    void LateUpdate()
    {
        cameraRay();
        // Early out if we don't have a target
        if (!target)
            return;
        if (target)
        {
            
            x += Input.GetAxis("Mouse X") * Time.deltaTime * z;
            y -= Input.GetAxis("Mouse Y") * Time.deltaTime * z;

            if (x > 360)
            { x -= 360; }
            else if (x < 0)
            { x += 360; }

            y = Mathf.Clamp(y,-60,60);

            distance = distance - Input.GetAxis("Mouse ScrollWheel")* Time.deltaTime* z;
            distance = Mathf.Clamp(distance, mindistance, maxdistance);

            rotationEuler = Quaternion.Euler(y, x, 0);

            if (!Rhit)
            { offset = rotationEuler * new Vector3(0, 0, -distance) + target.position; }
            if (Rhit)
            { offset = rotationEuler * new Vector3(0, 0, -h) + target.position; }

            transform.position = Vector3.Lerp(transform.localPosition, offset, sensitivity);
            transform.rotation = Quaternion.Lerp(transform.localRotation, rotationEuler, sensitivity);


            transform.LookAt(target);

        }
    }

    void cameraRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(target.position, transform.TransformDirection(-Vector3.forward), out hit))
        {
            if (hit.collider.tag == "Wall")
            {
                h = hit.distance / 2;
                Rhit = true;
            }
        }
        else
        {
            Rhit = false;
        }

        if (h > distance && Rhit == true)
        {
            Rhit = false;
        }
    }
}
