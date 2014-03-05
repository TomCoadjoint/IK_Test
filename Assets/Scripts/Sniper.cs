using UnityEngine;
using System.Collections;

public class Sniper : MonoBehaviour {

	public Animation animationComponent;
	public GUITexture scope;
	
	private bool firing = false;
	private bool reloading = false;
	private bool inScope = false;
	private bool aiming = false;

	private float fireRate = 0.75f;
	private float nextFireTime = 0f;
	private float reloadTime = 3.4f;

	private int maxAmmo = 6;
	private int ammo;

	void Start() {
		
		ammo = maxAmmo;
	}

	void Update () {
				
		if(Input.GetButtonDown("Fire1") && (Time.time>nextFireTime) && !reloading)
		{
			Firing ();
			firing = true;
		}
		else if((Input.GetButtonDown("Reload") && !firing) || ammo <=0)
		{			
			StartCoroutine(Reloading());
		}
		else if(Input.GetButtonDown("Fire2") && !firing && !reloading)
		{			
			animationComponent.Play("Sniper_scope");
			inScope = true;
		}		

		if(inScope)
		{
			if(!animationComponent.IsPlaying("Sniper_scope") && !aiming)
			{
				aiming = true;				
				scope.enabled = true;
			}
			
			if(aiming)
			{
				Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView,5f,10f*Time.deltaTime);

				if(Input.GetButtonDown("Fire2"))
				{
					Camera.main.fieldOfView = 60f;
					scope.enabled = false;
					aiming = false;
					inScope = false;
				}				
			}
		}
		else animationComponent.CrossFadeQueued("Sniper_idle",0f,QueueMode.CompleteOthers);
				
		if(nextFireTime > Time.time){			
			firing = false;
		}
	}	
	
	private void Firing()
	{
		if(ammo <= 0)
		{
			return;
		}
		else
		{
			if (Time.time - fireRate > nextFireTime)
			nextFireTime = Time.time - Time.deltaTime;
	
			while( nextFireTime < Time.time && ammo != 0)
			{
				if(!inScope)
				{
					animationComponent.Rewind("Sniper_fire");
					animationComponent.Play("Sniper_fire");			
				}
				
				ammo--;
				nextFireTime += fireRate;
			}
		}
	}
	
	private IEnumerator Reloading()
	{
		if(reloading)
		{
			animationComponent.CrossFade("Sniper_reload");
			yield break;
		}
		else
		{
			reloading = true;
			yield return new WaitForSeconds(reloadTime);
			ammo = maxAmmo;
			reloading = false;
		}
	}
}
