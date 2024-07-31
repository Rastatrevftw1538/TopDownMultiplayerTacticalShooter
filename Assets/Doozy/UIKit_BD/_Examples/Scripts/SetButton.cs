using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SetButton : MonoBehaviour {
	
	void Start () 
	{
		string[] goName = gameObject.name.Split('_');

		SpriteState sprState = new SpriteState();

		string highlitedStateName = "";
		string pressedStateName = "";

		for(int i = 0; i < goName.Length -1; i++)
		{
			highlitedStateName += goName[i] + "_";
			pressedStateName += goName[i] + "_";
		}

		highlitedStateName += "Hover";
		pressedStateName += "Pressed";

		print (highlitedStateName);

		sprState.highlightedSprite = Resources.Load <Sprite> ("Sprites/Buttons/@1x/" + highlitedStateName) as Sprite;
		sprState.pressedSprite = Resources.Load <Sprite> ("Sprites/Buttons/@1x/" + pressedStateName) as Sprite;

		GetComponent<Button>().spriteState = sprState;
	}
	
}
