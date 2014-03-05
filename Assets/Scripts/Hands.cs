using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {
	
	public Transform hitCube;
	public Transform bashCube;
	
	private enum Fight
	{
		Idle,
		Hitting
	}
	
	private enum Hitting
	{
		Punch,
		Chop
	}
	
	public Animation animationComponent;
	
	private Fight fight;
	private Hitting hitting;
	private float timer = 0f;
	
	IEnumerator Start(){
		
		fight = Fight.Idle;
		
		animation.Stop();
		animation.wrapMode = WrapMode.Loop;

		animationComponent["Hands_Idle3"].layer = 1;
				
		animationComponent["Hands_Idle1"].layer = 1;
		animationComponent["Hands_Idle1"].speed = 0.5f;
		animationComponent["Hands_Idle1"].wrapMode = WrapMode.Once;
		animationComponent["Hands_Idle1"].blendMode = AnimationBlendMode.Additive;
		
		animationComponent["Hands_Idle2"].layer = 2;
		animationComponent["Hands_Idle2"].weight = 1f;
		animationComponent["Hands_Idle2"].wrapMode = WrapMode.Once;
		
		animationComponent["Hands_Idle4"].layer = 1;
		
		animationComponent["Hands_Idle5"].layer = 1;
		
		animationComponent["Hands_Idle_AfterPunch1"].layer = 2;
		animationComponent["Hands_Idle_AfterPunch1"].wrapMode = WrapMode.Once;
		
		animationComponent["Hands_Idle_AfterPunch2"].layer = 2;
		animationComponent["Hands_Idle_AfterPunch2"].wrapMode = WrapMode.Once;
		
		animationComponent["Hands_Punch1"].layer = 2;
		animationComponent["Hands_Punch1"].wrapMode = WrapMode.Once;
		
		animationComponent["Hands_Punch2"].layer = 2;
		animationComponent["Hands_Punch2"].wrapMode = WrapMode.Once;
			
		animationComponent.CrossFade("Hands_Idle2");
		
		//Problem with not hitting properly sometimes is because still crossfading to Idle3
		
		while(true)
		{
			bashCube.renderer.material.color = Color.grey;
			
			switch(fight)
			{
			case Fight.Idle:
				
				animationComponent.CrossFade("Hands_Idle3",0.25f);
				
				if(Input.GetButtonDown("Fire1"))
				{
					fight = Fight.Hitting;
					hitting = Hitting.Punch;
					
					animationComponent.CrossFade("Hands_Idle4");
					animationComponent["Hands_Punch1"].speed = 0f;
					animationComponent.CrossFade("Hands_Punch1");
				}
				else if(Input.GetButtonDown("Fire2"))
				{					
					fight = Fight.Hitting;
					hitting = Hitting.Chop;
					
					animationComponent.CrossFade("Hands_Idle5");
					animationComponent["Hands_Punch2"].speed = 0f;
					animationComponent.CrossFade("Hands_Punch2");
				}					
				
				break;

			case Fight.Hitting:
				
				RaycastHit hit;
				
				switch(hitting)
				{
				case Hitting.Punch:					
										
					if(Physics.Raycast(hitCube.position,hitCube.forward,out hit, 0.25f))
					{
						bashCube.renderer.material.color = Color.red;
						animationComponent.CrossFade("Hands_Idle_AfterPunch1",0.3f);
					}

					if(animationComponent["Hands_Punch1"].weight > 0.99f) animationComponent["Hands_Punch1"].speed = 1f; 
					
					timer += Time.deltaTime;
		
					if(Input.GetButtonDown("Fire1"))
					{
						timer = 0;
						Debug.DrawRay(hitCube.position,hitCube.forward,Color.blue);
						animationComponent.CrossFadeQueued("Hands_Punch1", 0.1f,QueueMode.PlayNow);						
					}
					
					if(Input.GetButtonDown("Fire2"))
					{
						hitting = Hitting.Chop;
					
						animationComponent.CrossFade("Hands_Idle5");
						animationComponent["Hands_Punch2"].speed = 0f;
						animationComponent.CrossFade("Hands_Punch2");
					}					
					
					if(!animationComponent.IsPlaying("Hands_Punch1"))
					{
						if(timer >= animationComponent["Hands_Punch1"].length) timer = 0;
												
						if(timer <= 0.5f)
						{
							timer += Time.deltaTime;
							}
						else fight = Fight.Idle;
					}	
					
					break;
				case Hitting.Chop:
					
					Debug.DrawRay(hitCube.position,-hitCube.right,Color.blue);
		
					if(Physics.Raycast(hitCube.position,-hitCube.right,out hit, 0.25f))
					{
						bashCube.renderer.material.color = Color.red;
						animationComponent.CrossFade("Hands_Idle_AfterPunch2",0.3f);
					}

					if(animationComponent["Hands_Punch2"].weight > 0.99f) animationComponent["Hands_Punch2"].speed = 1f; 
					
					timer += Time.deltaTime;
		
					if(Input.GetButtonDown("Fire2"))
					{
						timer = 0;
						
						animationComponent.CrossFadeQueued("Hands_Punch2", 0.1f,QueueMode.PlayNow);						
					}
					
					if(Input.GetButtonDown("Fire1"))
					{
						hitting = Hitting.Punch;
					
						animationComponent.CrossFade("Hands_Idle4");
						animationComponent["Hands_Punch1"].speed = 0f;
						animationComponent.CrossFade("Hands_Punch1");
					}
					
					if(!animationComponent.IsPlaying("Hands_Punch2"))
					{
						if(timer >= animationComponent["Hands_Punch2"].length) timer = 0;
												
						if(timer <= -0.2f)
						{
							timer += Time.deltaTime;
							}
						else fight = Fight.Idle;
					}
					
					break;
				}
				
				break;
			}
			yield return 0;
		}
	
	}
}
