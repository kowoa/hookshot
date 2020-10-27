using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TO TWEAK HOOK ROPE PHYSICS, GO TO SPAWNJOINT() AND EXPERIMENT WITH VALUES

[RequireComponent(typeof(LineRenderer))]
public class Hookshot : MonoBehaviour
{
    public Transform playerCamera, player, target;
    public KeyCode button;
    public float maxDistance = 100f; // maximum distance the hook can shoot out
    public float hookSpeed = 20f;

    public enum HookMode
    {
        swing,
        pull
    }

    public HookMode hookMode;

    private LineRenderer lineRenderer;
    private SpringJoint joint;
    private bool isHooked = false;
    private bool shotHook = false;
    private Vector3 currentTargetDestination;
    private Vector3 newTargetDestination; // used to shoot out hook without destroying current joint

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!shotHook && !isHooked && Input.GetKeyDown(button))
        {
            ShootHook(); // shoot hook (hook begins to travel toward targetDestination)
        }
        else if (isHooked && Input.GetKeyDown(button))
        {
            ReleaseHook(); //  release hook
        }
        else if (shotHook && !isHooked)
        {
            TravelHook(); // hook travels toward targetDestination until it hooks onto something
        }

        DrawRope();

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
            isHooked = true;
        }
        else // not hooked onto something
        {
            // travel toward target destination
            target.transform.position = Vector3.MoveTowards(target.transform.position, newTargetDestination, hookSpeed * Time.deltaTime);
        }
    }

    private void ShootHook()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance)) // hit an object
        {
            // If raycast hit an object, target begins travelling toward targetDestination
            newTargetDestination = hit.point;
            target.transform.SetParent(null); // set target to have no parent (so that it doesn't follow player when travelling)
            shotHook = true;

            lineRenderer.enabled = true;
        }
        else // If raycast failed to hit anything
        {
            // destroy current joint/hook if it exists
            ReleaseHook();
        }
    }

    // spawn a joint that acts as a hook on the object
    private void SpawnJoint()
    {
        ReleaseHook();
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = currentTargetDestination;

        float distanceToTarget = Vector3.Distance(player.position, currentTargetDestination);

        // --------- EXPERIMENT WITH THESE VALUES --------- //

        if (hookMode == HookMode.swing)
        {
            // Settings for swinging
            joint.maxDistance = distanceToTarget * 0.5f;
            joint.minDistance = distanceToTarget * 0.25f;
        }
        else if (hookMode == HookMode.pull)
        {
            // Settings for pulling
            joint.maxDistance = distanceToTarget * 0.1f;
            joint.minDistance = distanceToTarget * 0.05f;
        }

        joint.spring = 10f;
        joint.damper = 2f;
        joint.massScale = 4.5f;

        // ------------------------------------------------ //
    }

    // draaw a line from joint to hook origin
    private void DrawRope()
    {
        if (isHooked) // hooked
        {
            // Draw line from hook origin to target destination
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentTargetDestination);
        }
        else if (shotHook && !isHooked) // hook is travelling
        {
            // Draw line from hook origin to target
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.position);

        }
    }

    // destroy the joint and line, releasing the player
    private void ReleaseHook()
    {
        if (isHooked)
        {
            lineRenderer.enabled = false;
            Destroy(joint);
            isHooked = false;
        }
    }
}
