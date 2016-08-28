using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public bool fixedWidth = false;
	public int width = 50;

	void Update ()
	{
		//Screen.height
		//Screen.width
		//GetComponent<Camera>().orthographicSize 
		//GetComponent<Camera>().aspect
		//GetComponent<Camera>().ResetAspect()

		//Force constant width
		if (fixedWidth)
		{
			GetComponent<Camera>().orthographicSize = (width / GetComponent<Camera>().aspect) / 2;
		}
	}
}
