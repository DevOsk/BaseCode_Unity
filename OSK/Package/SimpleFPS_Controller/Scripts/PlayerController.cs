﻿/*
    This PlayerController is based on a tutorial:
    https://www.youtube.com/watch?v=n-KX8AeGK7E
 */
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public AnimationCurve jumpFallOff;
    public float jumpHeight;
    bool isJumping;
    CharacterController charController;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        isJumping = false;
    }

    void Update()
    {
        PlayerMovement();
    }

    void PlayerMovement()
    {
        float vertInput = Input.GetAxis("Vertical") * movementSpeed;
        float horizInput = Input.GetAxis("Horizontal") * movementSpeed;

        Vector3 vertMovement = transform.forward * vertInput;
        Vector3 horizMovement = transform.right * horizInput;

        charController.SimpleMove(vertMovement + horizMovement);
        Jump();
    }

    private void Jump()
    {
        if (Input.GetKeyDown("space") && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpHeight * Time.deltaTime);
            timeInAir += Time.deltaTime;

            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = 45.0f;
        isJumping = false;
    }

    public void Lose()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Handle collisions
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Exit"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (col.CompareTag("River"))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
