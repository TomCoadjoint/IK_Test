var walkSpeed 				= 4.0	;
var runSpeed 				= 12.0	;

var controllerHeight: float = 0.3	;

var audioJump		: AudioClip		;

var controller 		: CharacterController			;
var walker 			: FPSWalkerAdvanced				;
var player			:GameObject;

controller = GetComponent(CharacterController)		;
walker 	   = GetComponent(FPSWalkerAdvanced)		;

// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
var limitDiagonalSpeed = true;

// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
// There must be a button set up in the Input Manager called "Run"
var toggleRun = false;

// If checked, the crouch key toggles between crouching and not crouching. Otherwise player crouches if the key is held down and doesn't crouch otherwise
var toggleCrouch = false;


var jumpSpeed = 8.0;
var gymSpeed = 4.0	;
var smooth = 2.0;
var gravity = 20.0;

// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
var fallingDamageThreshold = 10.0;

// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
var slideWhenOverSlopeLimit = false;

// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
var slideOnTaggedObjects = false;

var slideSpeed = 12.0;

// If checked, then the player can change direction while in the air
var airControl = false;

// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
var antiBumpFactor = .75;

// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping 
var antiBunnyHopFactor = 1;

private var moveDirection = Vector3.zero;
private var grounded = false;
private var myTransform : Transform;
private var speed : float;
private var hit : RaycastHit;
private var fallStartLevel : float;
private var falling = false;
private var slideLimit : float;
private var rayDistance : float;
private var contactPoint : Vector3;
private var playerControl = false;
private var jumpTimer : int;
private var gymTimer: int;
private var isRotating = false;
private var isOpposite = false;
private var timer:float;
private var lastTime:float = 0;
private var target;

function Start () {
    timer = Time.time;
	myTransform = transform;
    speed = walkSpeed;
    rayDistance = controller.height * .5 + controller.radius;
    slideLimit = controller.slopeLimit - .1;
    jumpTimer = antiBunnyHopFactor;
    oldPos = transform.position;
}

function FixedUpdate() {
    var inputX = Input.GetAxis("Horizontal");
    var inputY = Input.GetAxis("Vertical");
    // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
    var inputModifyFactor = (inputX != 0.0 && inputY != 0.0 && limitDiagonalSpeed)? .7071 : 1.0;
    
	if(isRotating == false && isOpposite == false)
    {
    
    	if(Input.GetButton("Gym") && Input.GetKey("a"))
       	{    
			
    		lastTime = Time.time;
    		timer = 0; 
    		target = 60;   	
     		isRotating = true;
    	}
    	else if(Input.GetButton("Gym") && Input.GetKey("d"))
    	{    
    		lastTime = Time.time;
    		timer = 0; 
    		target = 298;   	
     		isRotating = true;
    	}    	
    }
	
    if (grounded) {
        var sliding = false;
        // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
        // because that interferes with step climbing amongst other annoyances
        if (Physics.Raycast(myTransform.position, -Vector3.up, hit, rayDistance)) {
            if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                sliding = true;
        }
        // However, just raycasting straight down from the center can fail when on steep slopes
        // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
        else {
            Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, hit);
            if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                sliding = true;
        }

        // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
        if (falling) {
            falling = false;
            if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                FallingDamageAlert (fallStartLevel - myTransform.position.y);
        }
        
        // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
        if (!toggleRun) 
            speed = Input.GetButton("Run")? runSpeed : walkSpeed;

		if(!toggleCrouch)
		{
			if(Input.GetButton("Crouch"))
			{
				walker.gravity = 30;
				controller.height = controllerHeight;
			}
			else
			{
				controller.height = 2;
				walker.gravity = 20;
			}
		}	
        // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
        if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
            var hitNormal = hit.normal;
            moveDirection = Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize (hitNormal, moveDirection);
            moveDirection *= slideSpeed;
            playerControl = false;
        }
        // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
        else {
            moveDirection = Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;
            playerControl = true;
        }

        // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
        if (!Input.GetButton("Jump"))
            gymTimer++;
        else if (gymTimer >= antiBunnyHopFactor) 
		{
            moveDirection.y = jumpSpeed;
            gymTimer = 0;
			audio.clip = audioJump;
			audio.Play();
        }
		
		if (!Input.GetButton("Gym"))
            jumpTimer++;
        else if (jumpTimer >= antiBunnyHopFactor) 
		{
            moveDirection = moveDirection * gymSpeed;
            moveDirection.y = jumpSpeed;                                                                                                                                                                                                                                                                                           
            jumpTimer = 0;
			audio.clip = audioJump;
			audio.Play();
		}
		
    }
    else {
        // If we stepped over a cliff or something, set the height at which we started falling
        if (!falling) {
            falling = true;
            fallStartLevel = myTransform.position.y;
        }
        
        // If air control is allowed, check movement but don't touch the y component
        if (airControl && playerControl) {
            moveDirection.x = inputX * speed * inputModifyFactor;
            moveDirection.z = inputY * speed * inputModifyFactor;
            moveDirection = myTransform.TransformDirection(moveDirection);
        }
    }

    // Apply gravity
    moveDirection.y -= gravity * Time.deltaTime;

    // Move the controller, and set grounded true or false depending on whether we're standing on something
    grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
}

var smoothing = 1.0;
var smoothing1 = 1.0;
private var angle = 0;

function Update () 
{
    // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
    // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
    if (toggleRun && grounded && Input.GetButtonDown("Run"))
        speed = (speed == walkSpeed? runSpeed : walkSpeed);
	
	timer = Time.time -lastTime;
	
	//Debug.Log(isRotating + "" + isOpposite);
	
	if(isRotating)
    {

    	angle = Mathf.LerpAngle(0, target, timer*smoothing);
    	player.transform.localEulerAngles.z = angle;
		//Debug.Log("Main camera " + player.transform.localEulerAngles.z);
   	    		    		    	
    	if(player.transform.localEulerAngles.z == target || player.transform.localEulerAngles.z == 300 )
    	{
    		isRotating = false;
    		isOpposite = true;
			timer = 0;
			lastTime = Time.time;
    	  }

    }	
    if(isOpposite)
    {
    	angle = Mathf.LerpAngle(target,0,timer*smoothing1);
    	player.transform.localEulerAngles.z = angle;
    	
    if(player.transform.localEulerAngles.z <= 1)
    	{
    		
    		isOpposite = false;
    	}
    	
    }	
		
	// If the crouch button is set to toggle, then switch between crouch / not crouch. (We use Update for this..._
	// Fixed Update is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
	if (toggleCrouch && grounded && Input.GetButtonDown("Crouch"))
	{
		walker.gravity = (walker.gravity == 20? 30 : 20);
		controller.height = (controller.height == 2? 0.3 : 2);
	}
	
	if (Input.GetButtonDown("Run Toggle"))
	{
		toggleRun = (toggleRun == true? false : true);
	}
	if (Input.GetButtonDown("Crouch Toggle"))
	{
		toggleCrouch = ( toggleCrouch == true? false : true);
	}
	

}

// Store point that we're in contact with for use in FixedUpdate if needed
function OnControllerColliderHit (hit : ControllerColliderHit) {
    contactPoint = hit.point;
}

// If falling damage occured, this is the place to do something about it. You can make the player
// have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
function FallingDamageAlert (fallDistance : float) {
    Debug.Log ("Ouch! Fell " + fallDistance + " units!");   
	//FPSPlayer.hitPoints -=fallDistance;
}

@script RequireComponent(CharacterController)