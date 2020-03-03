using System;
using UnityEngine;

public class PlayerMovementModified: MonoBehaviour {

    public Vector3 wallJumpSphere;
    public float wallJumpSphereRadius;
    public bool onWall, isWallJumping = false;

    [Range(0f,5000f)]
    public float climbSpeed = 1000f;

    RaycastHit hit;
    [Range(1f,100f)]
    public float hitDistance = 10f;
    public GameObject cubePrefab;
    public GameObject hand;
    //private Vector3 handScale = new Vector3(0.15f, 0.15f, 0.15f);
    //private Vector3 crouchHandScale = new Vector3(0.15f, 0.3f, 0.15f);
    //public Transform explodeJumpPoint;
    //public bool explodeJump = false,canExplodeJump = true;
    //public float explodeJumpForce = 500f;
    //public float explodeJumpRadius = 30f;
    //public bool grapple = false,isGrappling;
    //private Transform grapplePoint = null;
    //public float grappleSpeed = 3000f;

    public Transform playerCam;
    private Vector3 camScale = new Vector3(1f, 1f, 1f);
    private Vector3 crouchCamScale = new Vector3(1f, 2f, 1f);
    public Transform orientation;
    
    private Rigidbody rb;

    private float xRotation;
    [Range(30f,100f)]
    public float sensitivity = 50f;
    [Range(0.1f,5f)]
    public float sensMultiplier = 1f;
    
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;
    
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

   private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    
    float x, y;
    bool jumping, sprinting, crouching;
    
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        MyInput();
        Look();
    }

    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
      
        if (Input.GetKeyDown(KeyCode.LeftControl) && grounded)
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();

        if (isWallJumping)
            y = 0;
    }

    private void StartCrouch() {
        transform.localScale = crouchScale;
        playerCam.gameObject.transform.localScale = crouchCamScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 1f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch() {
        transform.localScale = playerScale;
        playerCam.gameObject.transform.localScale = camScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {

        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        CounterMovement(x, y, mag);

        if (readyToJump && jumping) Jump();

        float maxSpeed = this.maxSpeed;

        if (crouching && grounded && readyToJump && rb.velocity.magnitude>10f)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 4000);
            return;
        }

        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        float multiplier = 1f, multiplierV = 1f;

        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        //if (grounded && crouching) multiplierV = 0f; // prevent movement while crouching

        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);

        Collider[] collisions = Physics.OverlapSphere(transform.position + wallJumpSphere, wallJumpSphereRadius, whatIsGround);
        int colliderCount = 0;
        foreach (Collider c in collisions)
        {
            if (c == true)
            {
                colliderCount++;
                onWall = true;
                break;
            }
        }
        if (colliderCount == 0)
            onWall = false;

        if (onWall && grounded)
        {
            onWall = false;
            y = 0;
        }
        
        if (onWall && y == 1)
            wallClimb();
        
        ///if(Physics.Raycast(hit))

    }
       

    private void Jump() {
        if (grounded && readyToJump){
            readyToJump = false;                                          

            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
           Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (onWall)
        {
            readyToJump = false;
            onWall = false;
            isWallJumping = true;

            rb.AddForce(Vector2.up * jumpForce * 1.75f);
            rb.AddForce(-orientation.transform.forward * jumpForce * 1.75f);
           
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown * 5f);
            Invoke(nameof(ResetWallJump), jumpCooldown * 5f);
        }
    }

    private void wallClimb()
    {
        rb.AddForce(orientation.forward * Time.deltaTime * 3000);
        rb.velocity += new Vector3(0f,climbSpeed / 1000f, 0f); ;
    }

    private void ResetWallJump()
    {
        isWallJumping = false;
    }
    
    private void ResetJump() {
        readyToJump = true;
    }
    
    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;
        
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * 0.9f * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }


    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    
    private void OnCollisionStay(Collision other) {
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;
        
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }
        
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + wallJumpSphere, wallJumpSphereRadius);
    }

}
