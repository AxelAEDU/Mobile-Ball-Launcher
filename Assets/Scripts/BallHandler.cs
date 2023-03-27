using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachedDelay;
    [SerializeField] private float respawnDelay;

    private Rigidbody2D currentBallRb;
    private SpringJoint2D currentBallSpringJoint;
    private Camera mainCamera;
    private bool isDragging;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    void OnEnable() 
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable() 
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBallRb == null) 
        {
            return;
        }

        // if no touches on screen 
        if(Touch.activeTouches.Count == 0)
        {
            if(isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
            
            return;
        }
        isDragging = true;
        currentBallRb.isKinematic = true;

        Vector2 touchPos = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPos += touch.screenPosition;
        }

        touchPos /= Touch.activeTouches.Count;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(touchPos);

        currentBallRb.position = worldPos;        
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab,pivot.position,Quaternion.identity);

        currentBallRb = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRb.isKinematic = false;
        currentBallRb = null;

        Invoke(nameof(DetachBall), detachedDelay);


    }

    private void DetachBall() 
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);

    }
}
