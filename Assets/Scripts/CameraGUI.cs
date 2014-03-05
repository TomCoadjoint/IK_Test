using UnityEngine;
using System.Collections;

public class CameraGUI : MonoBehaviour {

	public Texture2D crosshair;
	
	void OnGUI() {		
	
		float xMin = (Screen.width / 2) - (crosshair.width / 2);
    	float yMin = (Screen.height / 2) - (crosshair.height / 2);
    	GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
	}
}
