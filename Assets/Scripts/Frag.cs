using UnityEngine;
using System.Collections;

public class Frag : MonoBehaviour {

	public Animation animationComponent;
		
	private float reloadTime = 0.3f;
	private float throwTime = 1f;

	private bool firing = false;
	private bool reloading = false;
	
	void Start() {		
	}
	
	void Update() {
		
		if(Input.GetButtonDown("Fire1"))
		{
			StartCoroutine(Fire());
			
		}
		else if(!reloading || !firing)
		{
			animationComponent["Frag_idle"].speed = 0.25f;
			animationComponent.CrossFadeQueued("Frag_idle",0.1f,QueueMode.CompleteOthers);
		}
		
		if(firing)
		{
			StartCoroutine(Fire());			
		}
		
		if(reloading)
		{
			StartCoroutine(Reload ());
		}
	}
		
	private IEnumerator Fire() {
		
		if(firing)
		{
			animationComponent.CrossFade("Frag_throw");
			yield break;
		}
		else
		{
			firing = true;
			yield return new WaitForSeconds(throwTime);	
			StartCoroutine(Reload());
		}			
	}
	
	private IEnumerator Reload() {
		
		if(reloading)
		{
			animationComponent.CrossFade("Frag_reload");
			yield break;
		}
		else
		{
			reloading = true;
			yield return new WaitForSeconds(reloadTime);
			firing = false;
			reloading = false;
		}
		
				
	}
	
}
