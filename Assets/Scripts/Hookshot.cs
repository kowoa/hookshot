using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Hookshot : MonoBehaviour
{
    public Transform playerCamera, player, target;
    public float maxDistance = 100f; // maximum distance the hook can shoot out

    private LineRenderer lineRenderer;
    private SpringJoint joint;
    private bool shotHook = false;
    private Vector3 currentTargetDestination;
    private Vector3 newTargetDestination;

    

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        DrawRope();

        if (shotHook) // hook travels toward targetDestination
        {
            TravelHook();
        }
        else if (Input.GetMouseButtonDown(0) && !shotHook) // raycast targetDestination
        {
            ShootHook();
        }
    }

    // hook (target) travels toward target destination
    private void TravelHook()
    {
        if (Vector3.Distance(target.transform.position, newTargetDestination) < 0.01f) // hooked onto something
        {
            currentTargetDestination = newTargetDestination;
            // if the target is approximately at same position as targetDestination, spawn joint
            SpawnJoint();
            lineRenderer.positionCount = 2;

            target.transform.position = transform.position; // reset target position
            target.transform.SetParent(transform.parent); // reset target parent
            shotHook = false; // can shoot another hook
        }
        else
        {
            // travel toward target destination
            target.transform.position = Vector3.MoveTowards(target.transform.position, newTargetDestination, 5f * Time.deltaTime);
        }
    }

    private void ShootHook()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance))
        {
            // If raycast hit an object, target begins travelling toward targetDestination
            newTargetDestination = hit.point;
            shotHook = true;
            target.transform.SetParent(null); // set target to have no parent (so that it doesn't follow player when travelling)
        }
        else // If raycast failed to hit anything
        {
            // destroy current joint/hook if it exists
            StopHook();
        }
    }

    // spawn a joint that acts as a hook on the object
    private void SpawnJoint()
    {
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = currentTargetDestination;

        float distanceToTarget = Vector3.Distance(player.position, currentTargetDestination);

        // Experiment with these values below

        joint.maxDistance = distanceToTarget * 0.8f;
        joint.minDistance = distanceToTarget * 0.25f;

        joint.spring = 10f; // controls pull/push
        joint.damper = 2f;
        joint.massScale = 4.5f;

        
    }

    private void DrawRope()
    {
        if (joint)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentTargetDestination);
        }
    }

    private void StopHook()
    {
        if (joint != null)
        {
            lineRenderer.positionCount = 0;
            Destroy(joint);
        }
    }
}
