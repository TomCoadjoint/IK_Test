using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	
	public Animation animationComponent;
	
	private int ammo = 1;
	
	private float nextFireTime = 0f;
	private float fireRate = 1f;
	private float reloadTime = 3f;
	
	private bool reloading = false;
	private bool firing = false;
	
	void Update() {
		
		if(Input.GetButtonDown("Fire1") && (Time.time>nextFireTime) && !reloading)
		{
			Firing ();
			firing = true;
		}
		else if((Input.GetButtonDown("Reload") && !firing) || ammo <=0)
		{			
			StartCoroutine(Reloading());
		}
		else animationComponent.CrossFadeQueued("Rocket_idle",0f,QueueMode.CompleteOthers);
				
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
				animationComponent.Rewind("Rocket_fire");
				animationComponent.Play("Rocket_fire");			
				
				ammo--;
				nextFireTime += fireRate;
			}
		}
	}

	private IEnumerator Reloading()
	{
		if(reloading)
		{
			animationComponent.CrossFade("Rocket_reload");
			yield break;
		}
		else
		{
			reloading = true;
			yield return new WaitForSeconds(reloadTime);
			ammo = 1;
			reloading = false;
		}
	}
	
}
