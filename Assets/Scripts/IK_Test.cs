using UnityEngine;
using System.Collections;

public class IK_Test : MonoBehaviour {
	
	public Transform elbow_piv;
	public Transform wrist_piv;
	public Transform cube_follow;
	
	private float e_angle_init;
	private float e_angle;
	private float s_angle;
	private float s_angle_init;
	private float s_angle_global;
	private float e_angle_global;
	private float forearm_length;
	private float upper_length;
	
	
	void Awake() {
		
		float cube_dist = Vector3.Magnitude(cube_follow.position - transform.position);
		forearm_length = Vector3.Magnitude(elbow_piv.position - wrist_piv.position);
		upper_length = Vector3.Magnitude(transform.position - elbow_piv.position);		
		
		e_angle_init = Vector3.Angle(elbow_piv.position-wrist_piv.position,elbow_piv.position-transform.position);
		s_angle_init = Vector3.Angle(transform.position-cube_follow.position,transform.position - elbow_piv.position);
		e_angle = Mathf.Rad2Deg*Mathf.Acos(-(cube_dist*cube_dist - forearm_length*forearm_length - upper_length*upper_length)/(2f*forearm_length*upper_length));
		s_angle = Mathf.Rad2Deg*Mathf.Acos(-(forearm_length*forearm_length-upper_length*upper_length - cube_dist*cube_dist)/(2*upper_length*cube_dist));

		s_angle_global = transform.eulerAngles.z;
		e_angle_global = elbow_piv.eulerAngles.z;
	}
	
	void FixedUpdate() {
				
		Quaternion target1 = Quaternion.Euler(0,0,e_angle_global + e_angle - e_angle_init- s_angle_init + s_angle);
		Quaternion target2 = Quaternion.Euler(0,0,s_angle_global - s_angle_init + s_angle);
		elbow_piv.rotation = Quaternion.Lerp(elbow_piv.rotation, target1, Time.deltaTime*2f);
		transform.rotation = Quaternion.Lerp (transform.rotation, target2, Time.deltaTime*3f);
		
		float cube_dist = Vector3.Magnitude(cube_follow.position - transform.position);
		forearm_length = Vector3.Magnitude(elbow_piv.position - wrist_piv.position);
		upper_length = Vector3.Magnitude(transform.position - elbow_piv.position);		
		
		e_angle_init = Vector3.Angle(elbow_piv.position-wrist_piv.position,elbow_piv.position-transform.position);
		s_angle_init = Vector3.Angle(transform.position-cube_follow.position,transform.position - elbow_piv.position);
		e_angle = Mathf.Rad2Deg*Mathf.Acos(-(cube_dist*cube_dist - forearm_length*forearm_length - upper_length*upper_length)/(2f*forearm_length*upper_length));
		s_angle = Mathf.Rad2Deg*Mathf.Acos(-(forearm_length*forearm_length-upper_length*upper_length - cube_dist*cube_dist)/(2*upper_length*cube_dist));

		s_angle_global = transform.eulerAngles.z;
		e_angle_global = elbow_piv.eulerAngles.z;

	}
	
	void Update () {
		

	}
}
