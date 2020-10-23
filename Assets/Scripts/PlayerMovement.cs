using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float moveSpeed = 10f;
    private float jumpHeight = 2f;
    private Rigidbody rb;
    private float xMovement, zMovement;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // XZ - Walking
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");

        rb.MovePosition(transform.position + Time.deltaTime * moveSpeed * transform.TransformDirection(xMovement, 0f, zMovement));

        // Y - Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.CheckSphere(groundCheck.position, 1, groundLayer))
            {
                rb.velocity = new Vector3(0, Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), 0);
            }
        }
    }
}
