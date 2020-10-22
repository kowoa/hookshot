using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Hookshot : MonoBehaviour
{
    public Transform playerCamera, player;
    public LayerMask hookableLayer; // layer containing all hookable objects
    public float maxDistance = 100f; // maximum distance the hook can shoot out

    private LineRenderer lineRenderer;
    private Vector3 hookTarget;
    private SpringJoint joint;

    

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartHook();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopHook();
        }
    }

    private void StartHook()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance))
        {
            hookTarget = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = hookTarget;

            float distanceToTarget = Vector3.Distance(player.position, hookTarget);

            joint.maxDistance = distanceToTarget * 0.8f;
            joint.minDistance = distanceToTarget * 0.25f;

            // Experiment with these values
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
    }

    private void StopHook()
    {

    }
}
