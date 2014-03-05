using UnityEngine;
using System.Collections;

public class Assault : MonoBehaviour {
	
	public Animation animationComponent;
	
	public float recoil = 0;
	
	public Transform re_pos;
	public Transform gun_pos;
	public Transform main_Camera;	
	
	private int ammo = 40;
	private float nextFireTime = 0f;
	private float fireRate = 0.05f;
	private float init_rotation = 0f;
	
	private Vector3 target;
		
	private MouseLook mouseLook;	
	
	private bool up = false;
	
	private enum State
	{
		Idle,
		Reloading,
		Firing
	}
	
	private State state;
	
	public void Start() {
		
		mouseLook = main_Camera.GetComponent("MouseLook") as MouseLook;
	
		state = State.Idle;
		
		animation.Stop();
		
		animationComponent["Assault_idle"].layer = 1;
		animationComponent["Assault_idle"].speed = 0.5f;
		animationComponent["Assault_idle"].wrapMode = WrapMode.Loop;
		
		animationComponent["Assault_reload"].layer = 1;
		animationComponent["Assault_reload"].speed = 1.5f;
		animationComponent["Assault_reload"].wrapMode = WrapMode.Once;
		
		animationComponent["Assault_fire"].layer = 1;
		animationComponent["Assault_fire"].wrapMode = WrapMode.Loop;
		animationComponent["Assault_fire"].speed = 1.5f;
	}
	
	public void LateUpdate () {
		
		switch(state)
		{
		case State.Firing:
			
			init_rotation = mouseLook.rotationY;
			
			animationComponent.CrossFade("Assault_fire",0.01f);
			
			if(up)
			{
				CameraRecoil(init_rotation);
				target = gun_pos.TransformPoint((1.2f-recoil/100f)*new Vector3(0f,0.05f,0f));
				re_pos.position = Vector3.Lerp (re_pos.position, target, 50f*Time.deltaTime);	
				
				if(Vector3.Distance(re_pos.position, target) < 0.001f)
				{
					up = false;					
				}
			}
			else
			{
				target = gun_pos.position;
				re_pos.position = Vector3.Lerp (re_pos.position, target, 50f*Time.deltaTime);
				
				if(Vector3.Distance (re_pos.position,target) < 0.001f)
				{
					up = true;
				}
			}
			
			if(Time.time - fireRate > nextFireTime)
			{
				nextFireTime = Time.time - Time.deltaTime;
			}
			
			while( nextFireTime < Time.time && ammo > 0)
			{
				ammo--;
				nextFireTime +=fireRate;
			}			
			
			if(ammo <= 0) state  = State.Reloading;
			else if(Input.GetButtonUp("Fire1")) state = State.Idle;			
			
			break;
		case State.Idle:
			
			re_pos.position = gun_pos.position;
			target = gun_pos.position;
			
			animationComponent.CrossFade("Assault_idle");
			
			if(Input.GetButton("Fire1"))
			{
				up = true;
				state = State.Firing;
			}
			else if(ammo <= 0) state = State.Reloading;
			
			break;
		case State.Reloading:
			
			ammo = 40;
			animationComponent.Play("Assault_reload");
			
			if(animationComponent["Assault_reload"].time >= animationComponent["Assault_reload"].length - 0.05f)
			{
				state = State.Idle;
			}
			break;
		}
	}
	
	void CameraRecoil(float angle)
	{
		float distance = Vector3.Distance(re_pos.position, gun_pos.position);
		mouseLook.rotationY = Mathf.Lerp (angle, angle + (100f-recoil)*distance/5f,30f*Time.deltaTime);		
	}
}
