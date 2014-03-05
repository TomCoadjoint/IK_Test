var speed 									= 6.0	;
var jumpSpeed 								= 8.0	;
var gravity 								= 20.0	;
var isCrouched 		: boolean 				= false	;
var tryingtostand 	: boolean 				= false	;
var controller 		: CharacterController			;
var walker 			: FPSWalker						;
var keytimer								=	0	;
var audioJump:AudioClip;

controller 	= GetComponent(CharacterController);
walker 		= GetComponent(FPSWalker);

private var moveDirection = Vector3.zero;
private var grounded : boolean = false;

	function Update () 
	{
		keytimer++;

		if(keytimer>10)
		{
 			if(Input.GetButton("crouch")) 
 			{
   				keytimer=0;

				if(!isCrouched)
				{
                	walker.gravity=30;
                	tryingtostand=false;
					crouch();
				}  
				else  
				{
                	if(!tryingtostand){
                    	stand();
                	}
            	}
			}
		}    

    	if(tryingtostand){
            if(controller.height<2){   

                controller.height += 5 * Time.deltaTime;
                transform.position.y += 2*Time.deltaTime;
                walker.gravity=0;

            } else {
                controller.height = 2;
                walker.gravity=30;
                tryingtostand=false;
           }
	    }
	}	

 
function crouch() {

   controller.height = .5;

   isCrouched = true;

}

 

function stand(){

	if(!tryingtostand){ 

	isCrouched = false;
	tryingtostand=true; 
	}

}

function FixedUpdate() 
{
	if (grounded) {
		// We are grounded, so recalculate movedirection directly from axes
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;
		
		if (Input.GetButton ("Jump")) {
			moveDirection.y = jumpSpeed;
			//audio.clip = audioJump;
			//audio.Play();
		}
	}

	// Apply gravity
	moveDirection.y -= gravity * Time.deltaTime;
	
	// Move the controller
	var controller : CharacterController = GetComponent(CharacterController);
	var flags = controller.Move(moveDirection * Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
}

@script RequireComponent(CharacterController)