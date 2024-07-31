using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SetSprite : MonoBehaviour {

	void Start () 
	{
		//string[] goName = gameObject.name.Split('_');
		GetComponent<Image>().sprite = Resources.Load <Sprite> ("Sprites/Ribbons/@1x/"+gameObject.name);
	}
	
}
