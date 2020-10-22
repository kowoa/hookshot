using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float expiryTime = 3f;

    private Camera cam;
    private bool lockedCamera = false;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }


    void Update()
    {
        LockCamera();
        Destroy(gameObject, expiryTime);
    }

    // Locks the camera on the projectile if a button is pressed
    private void LockCamera()
    {
        // toggled, not held down
        if (Input.GetKeyDown(KeyCode.LeftShift) && !lockedCamera)
        {
            lockedCamera = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && lockedCamera)
        {
            lockedCamera = false;
        }

        if (lockedCamera)
        {
            Vector3 projectileToCamera = transform.position - cam.transform.position;
            cam.transform.forward = projectileToCamera;
        }
    }
}
