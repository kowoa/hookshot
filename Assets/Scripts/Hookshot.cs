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
    private bool isHooking;

    

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        DrawRope();
        RotateHookShooter();

        if (Input.GetMouseButtonDown(0) && !isHooking)
        {
            StartHook();
        }
        else if (Input.GetMouseButtonDown(0) && isHooking)
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

            // Experiment with these values below

            joint.maxDistance = distanceToTarget * 0.8f;
            joint.minDistance = distanceToTarget * 0.25f;

            joint.spring = 10f; // controls pull/push
            joint.damper = 2f;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            isHooking = true;
        }
    }

    private void DrawRope()
    {
        if (joint)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hookTarget);
        }
    }

    private void StopHook()
    {
        lineRenderer.positionCount = 0;
        Destroy(joint);
        isHooking = false;
    }

    private void RotateHookShooter()
    {
        if (isHooking)
        {
            transform.LookAt(hookTarget);
        }
    }
}
