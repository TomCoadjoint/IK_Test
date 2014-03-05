/**
*  Script written by OMA [www.armedunity.com]
**/

var crouchSpeed = 2.0;
var walkSpeed = 8.0;
var runSpeed = 20.0;

var fallDamageMultiplier : int = 2;
var inAirControl = 0.1;
var gravity = 20.0;
var maxVelocityChange = 10.0;
var canJump = true;
var canGym = true;
var gymHeightY = 0.5;
var gymHeightX = 3.0;
var jumpHeight = 2.0;
var fallSound : AudioClip;
var mainCameraGO : GameObject;

//@HideInInspector
var grounded = false;
private var sliding : boolean = false;
private var speed = 10.0;
private var limitDiagonalSpeed = true;
private var crouching : boolean;
private var normalHeight : float = 0.5;
private var crouchHeight : float = -0.2;
private var crouchingHeight = 0.3;
private var hit : RaycastHit;
private var myTransform : Transform;
private var rayDistance : float;

@script RequireComponent(Rigidbody, CapsuleCollider)

function Awake (){
    rigidbody.freezeRotation = true;
    rigidbody.useGravity = false;
	myTransform = transform;
	rayDistance = collider.height * .5 + collider.radius;
}

function FixedUpdate (){
    if (grounded){			
		var inputX = Input.GetAxis("Horizontal");
		var inputY = Input.GetAxis("Vertical");
		
		var inputModifyFactor = (inputX != 0.0 && inputY != 0.0 && limitDiagonalSpeed)? .7071 : 1.0;
		
        if (Physics.Raycast(myTransform.position, -Vector3.up, hit, rayDistance)) {
            if (Vector3.Angle(hit.normal, Vector3.up) > 30){
                sliding = true;
				rigidbody.AddRelativeForce (-Vector3.up * 500);
			}else{
				sliding = false;
				
			}	
		}

        // Calculate how fast we should be moving
        var targetVelocity = new Vector3(inputX * inputModifyFactor, 0.0, inputY * inputModifyFactor);
        targetVelocity = myTransform.TransformDirection(targetVelocity);
        targetVelocity *= speed;	
		
        // Apply a force that attempts to reach our target velocity
        var velocity = rigidbody.velocity;
        var velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
   
        
        if (canJump && Input.GetButton("Jump")){
            rigidbody.velocity = Vector3(velocity.x, CalculateJumpVerticalSpeed(jumpHeight), velocity.z);
        }
        
        if(canGym && Input.GetButton("Gym")){
        	rigidbody.velocity = gymHeightX*Vector3(velocity.x, CalculateJumpVerticalSpeed(gymHeightY), velocity.z);        
        }
		
		if(!crouching){
            if (grounded && Input.GetButton("Run") && Input.GetKey("w"))
				speed = runSpeed;
			    else 
			    speed = walkSpeed;
			}else{
			speed = crouchSpeed;
		}
	
	}else{
	
		// AirControl 
		targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		targetVelocity = transform.TransformDirection(targetVelocity) * inAirControl;
		rigidbody.AddForce(targetVelocity, ForceMode.VelocityChange);
	} 

    // Gravity 
    rigidbody.AddForce(Vector3 (0, -gravity * rigidbody.mass, 0));
    grounded = false;
}

function OnCollisionStay (col : Collision){
	if (Physics.Raycast(myTransform.position, -Vector3.up, hit, rayDistance)) {
		grounded = true;
	}	
}

function HitJumpPad(velocity: float) {
       rigidbody.velocity.z += velocity;
    }

function OnCollisionEnter (collision : Collision){
    if(!grounded){
		var currSpeed : float = collision.relativeVelocity.magnitude;
		
		if (currSpeed > 20) {
			var damage : float = currSpeed * fallDamageMultiplier;
			Debug.Log ("FallDamage" + damage);
			SendMessage ("PlayerDamage", damage, SendMessageOptions.DontRequireReceiver);
		}	
	}	
}

function CalculateJumpVerticalSpeed (height: float){
    return Mathf.Sqrt(2 * height * gravity);
}

function Update(){
	
	if(mainCameraGO.transform.localPosition.y > normalHeight){
		mainCameraGO.transform.localPosition.y = normalHeight;
	} else if(mainCameraGO.transform.localPosition.y < crouchHeight){
		mainCameraGO.transform.localPosition.y = crouchHeight;
	}
	
	if (Input.GetButtonDown("Crouch") && !crouching) {
        
        collider.height = 1.5;
		collider.center = Vector3 (0, -0.25, 0);
		crouching = true;
	} 

	if(Input.GetButtonUp("Crouch") && crouching){
		collider.height = 2.0;
	    collider.center = Vector3 (0, 0, 0);
        crouching = false;
	}
	
	if(crouching){
		if(mainCameraGO.transform.localPosition.y > crouchHeight){
			if(mainCameraGO.transform.localPosition.y - (crouchingHeight * Time.deltaTime/.1) < crouchHeight){
				mainCameraGO.transform.localPosition.y = crouchHeight;
			} else {
				mainCameraGO.transform.localPosition.y -= crouchingHeight * Time.deltaTime/.1;
			}
		}
	} else {
		if(mainCameraGO.transform.localPosition.y < normalHeight){
			if(mainCameraGO.transform.localPosition.y + (crouchingHeight * Time.deltaTime/.1) > normalHeight){
				mainCameraGO.transform.localPosition.y = normalHeight;
			} else {
				mainCameraGO.transform.localPosition.y += crouchingHeight * Time.deltaTime/.1;
			}
		}		
	}
}

function Accelerate (accelerateY : float, accelerateZ : float){
    grounded = false;
	rigidbody.AddRelativeForce (0, accelerateY, accelerateZ);	
}