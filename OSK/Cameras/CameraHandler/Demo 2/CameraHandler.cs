﻿// Add script to MainCamera
// Script use Camera Perspective

using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public float PanSpeed = 1200F;
    private float currentSpeed;
    public float ZoomSpeedTouch = 0.1f;
    public float ZoomSpeedMouse = 0.5f;

    public float[] BoundsX = new float[] {-50, 50};
    public float[] BoundsZ = new float[] {-50, 20};
    public float[] ZoomBounds = new float[] {30, 75};

    // private float timeSmooth = 20F;
    private Camera cam;

    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    void Awake()
    {
        cam = GetComponent<Camera>(); // add to CameraMain
    }

    void Update()
    {
        if (Input.touchSupported) HandleTouch();
        else HandleMouse();
    }

    void HandleTouch()
    {
        switch (Input.touchCount)
        {
            case 1: // Panning
                wasZoomingLastFrame = false;

                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }

                break;
            case 2: // Zooming
                Vector2[] newPositions = new Vector2[]
                {
                    Input.GetTouch(0).position,
                    Input.GetTouch(1).position
                };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;
                    ZoomCamera(offset, ZoomSpeedTouch);
                    lastZoomPositions = newPositions;
                }
                break;
            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        
        const float distanceMinFiel = 5;
        const float distanceMaxFiel = 5;
        
        float speedCamFielMin = 0.75f;
        float speedCamFielMax = 2f;
        float speedCamDragX = 1.5F;
        
        // calculator speed
        if (cam.fieldOfView < (ZoomBounds[0] + distanceMinFiel))
        {
            currentSpeed = (PanSpeed * speedCamFielMin) * Time.deltaTime;
        }
        else if (cam.fieldOfView >= (ZoomBounds[0] + distanceMinFiel) && cam.fieldOfView <= (ZoomBounds[1] - distanceMaxFiel))
        {
            currentSpeed = PanSpeed * Time.deltaTime;
        }
        else if(cam.fieldOfView > (ZoomBounds[1] - distanceMaxFiel))
        {
            currentSpeed = (PanSpeed * speedCamFielMax) * Time.deltaTime;
        }
        Vector3 move = new Vector3(offset.x * (currentSpeed * speedCamDragX), 0, offset.y * currentSpeed);

        // Perform the movement
        transform.Translate(move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);
        transform.position = pos;

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0) return;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }
}