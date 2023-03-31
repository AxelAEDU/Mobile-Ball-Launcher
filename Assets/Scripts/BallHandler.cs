using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    // Serialized fields so can be edited in the inspector
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachedDelay;
    [SerializeField] private float respawnDelay;

    //for internal use
    private Rigidbody2D currentBallRb;
    private SpringJoint2D currentBallSpringJoint;
    private Camera mainCamera;
    private bool isDragging;
    
    // Start is called before the first frame update
    void Start()
    {
        //calls on the maincamera
        mainCamera = Camera.main;

        //calls the SpawnNewBall() method
        SpawnNewBall();
    }

    void OnEnable() 
    {
        //setting multi-touch input on mobile devices to Enable
        EnhancedTouchSupport.Enable();
    }

    void OnDisable() 
    {
        //setting multi-touch input on mobile devices to Disable
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // checks if there is a current ball in play, if not it returns.
        if(currentBallRb == null) 
        {
            return;
        }

        // if no touches on screen, luanching the ball
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

        // Makes the ball rigidbody kinematic
        currentBallRb.isKinematic = true;

        // Calculate the average position of all active touches
        Vector2 touchPos = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPos += touch.screenPosition;
        }

        touchPos /= Touch.activeTouches.Count;

        // Makes the touchposition to world coordinates and will set the ball position
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(touchPos);

        currentBallRb.position = worldPos;        
    }

    // Instantiate a new ball at the position of the pivot and sets up the components
    private void SpawnNewBall()
    {
        // Instantiate a new ball prefab at the position of the pivot
        GameObject ballInstance = Instantiate(ballPrefab,pivot.position,Quaternion.identity);

        // Getting rigidbody of the new ball instance
        currentBallRb = ballInstance.GetComponent<Rigidbody2D>();

        // Getting spring joint of the new ball
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        // Setting body of the spring joint to the pivot
        currentBallSpringJoint.connectedBody = pivot;
    }

    //launches the current ball and prepares to detach it from the pivot
    private void LaunchBall()
    {
        // Enable physics on the ball rigidbody
        currentBallRb.isKinematic = false;

        //Set the ball rigidbody to null
        currentBallRb = null;

        // Call the DetachBall method after a delay
        Invoke(nameof(DetachBall), detachedDelay);


    }

    //detaches the ball from the pivot and prepares to spawn a new ball
    private void DetachBall() 
    {
        // Disable the ball spring joint
        currentBallSpringJoint.enabled = false;

        // Set the ball spring joint to null
        currentBallSpringJoint = null;

        // Call the SpawnNewBall method after a delay
        Invoke(nameof(SpawnNewBall), respawnDelay);

    }
}
