using UnityEngine;
using System.Collections;

public class Handgun : MonoBehaviour {
	
	public Transform main_Camera;
	public Transform re_pos;
	public Transform wrist_shoot;
	public Transform wrist;
	public Transform shoulder;
	public Transform elbow;
	
	public float recoil = 0f;
	
	private Vector3 target;
	private float dist;
	private float forearm_length;
	private float upper_length;
	private float e_angle_init;
	private float s_angle_init;
	private float e_angle;
	private float s_angle;
	private float s_angle_global;
	private float e_angle_global;
	private float init_rotation;
	
	private float timer = 0f;

	private Quaternion cameraRo; 
	
	private bool fired = false;
	private bool reloading = false;
	private bool up = false;
	
	private int ammo = 6;
	
	private MouseLook mouseLook;
	
	public Animation animationComponent;

	void Start () {	
		
		mouseLook = main_Camera.GetComponent("MouseLook") as MouseLook;
		animation.Stop();
		animation.wrapMode = WrapMode.Loop;
		
		animationComponent["Handgun_idle"].layer = 1;
		animationComponent["Handgun_idle"].speed = 0.25f;
		
		animationComponent["Handgun_idle_fire"].layer = 1;
		animationComponent["Handgun_idle_fire"].speed = 0.25f;
				
		animationComponent["Handgun_reload"].layer = 2;
		animationComponent["Handgun_reload"].speed = 1.5f;
		animationComponent["Handgun_reload"].weight = 1f;
		animationComponent["Handgun_reload"].wrapMode = WrapMode.Once;

		target = wrist_shoot.position;
		
		forearm_length = Vector3.Distance (elbow.position, wrist.position);
		upper_length = Vector3.Distance (shoulder.position, elbow.position);
		
		dist = Vector3.Magnitude(re_pos.position - shoulder.position);
		
		e_angle = Vector3.Angle(elbow.position - wrist.position, elbow.position-shoulder.position) - Mathf.Rad2Deg*Mathf.Acos(-(dist*dist - forearm_length*forearm_length - upper_length*upper_length)/(2f*forearm_length*upper_length));
		s_angle = Mathf.Rad2Deg*Mathf.Acos(-(forearm_length*forearm_length-upper_length*upper_length - dist*dist)/(2*upper_length*dist)) - Vector3.Angle (shoulder.position - elbow.position, shoulder.position - re_pos.position);
	}

	void Update () {
			
		if(!reloading)
		{
			if(Input.GetButtonDown("Fire1") && !up)
			{			
				if(ammo == 0f)
				{
					reloading = true;
					timer = 0f;
				}
				else
				{
					init_rotation = mouseLook.rotationY;
					--ammo;
					fired = true;
					up = true;					
				}		
			}
			else
			{
				if(Input.GetButtonDown("Reload"))
				{					
					reloading = true;
					timer = 0f;
				}						
			}
		
			if(fired)				
			{					
				animationComponent.CrossFade("Handgun_idle");
				animation.Stop();		
				dist = Vector3.Magnitude(re_pos.position - shoulder.position);

				e_angle = Vector3.Angle(elbow.position - wrist.position, elbow.position-shoulder.position) - Mathf.Rad2Deg*Mathf.Acos(-(dist*dist - forearm_length*forearm_length - upper_length*upper_length)/(2f*forearm_length*upper_length));
				s_angle = Mathf.Rad2Deg*Mathf.Acos(-(forearm_length*forearm_length-upper_length*upper_length - dist*dist)/(2*upper_length*dist)) - Vector3.Angle(shoulder.position-re_pos.position,shoulder.position - elbow.position);
				
				Quaternion target2 = shoulder.localRotation * Quaternion.Euler (0f,0f,-s_angle);
				Quaternion target1 = elbow.localRotation * Quaternion.Euler (0f,0f,e_angle);
		
				elbow.localRotation = Quaternion.Lerp(elbow.localRotation, target1, Time.deltaTime*18f);
				shoulder.localRotation = Quaternion.Slerp(shoulder.localRotation, target2, Time.deltaTime*20f);
		
				if(up)
				{
					CameraRecoil(init_rotation);
					target = wrist_shoot.TransformPoint((1.2f-recoil/100f)*new Vector3(0f,0.3f,-0.2f));
					re_pos.position = Vector3.Lerp (re_pos.position, target, 40f*Time.deltaTime);	
				
					if(Vector3.Distance(re_pos.position, target) < 0.001f)
					{
						up = false;					
					}
				}
				else
				{
					target = wrist_shoot.position;
					re_pos.position = Vector3.Lerp (re_pos.position, target, 8f*Time.deltaTime);
				
					if(Vector3.Distance (re_pos.position,target) < 0.001f)
					{
						fired = false;
					}
				}
			}	
			else 
			{	
					animationComponent.CrossFade("Handgun_idle");
					re_pos.position = wrist_shoot.position;
					wrist_shoot.position = wrist.position;
					target = wrist_shoot.position;
			}			
		}
		else
		{
			animationComponent.CrossFade("Handgun_reload");			
			
			timer += Time.deltaTime;
			
			if(animationComponent["Handgun_reload"].time >= animationComponent["Handgun_reload"].length-0.05f)
			{
				 animation.CrossFadeQueued("Handgun_idle", 0.2f, QueueMode.CompleteOthers);
				ammo = 6;
				reloading = false;
			}			
		}		
	}
	
	void CameraRecoil(float angle)
	{
		float distance = Vector3.Distance(re_pos.position, wrist_shoot.position);
		mouseLook.rotationY = Mathf.Lerp (angle, angle + (100f-recoil)*distance,30f*Time.deltaTime);		
	}
}
