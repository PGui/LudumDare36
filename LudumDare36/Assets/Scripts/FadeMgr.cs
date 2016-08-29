using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeMgr : MonoBehaviour {

	private float FadeValue = 1.0f;
	private float GoalValue = 0.0f;
	private float FadeSpeed = 0.03f;

	private SpriteRenderer Rend;

	public void FadeIn(float Speed)
	{
		GoalValue = 0.0f;
		FadeSpeed = Speed;
	}
 
	public void FadeOut(float Speed)
	{
		GoalValue = 1.0f;
		FadeSpeed = Speed;
	}

    // Use this for initialization
    void Start () {
		Rend = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		FadeValue = Mathf.Clamp01(FadeValue + (GoalValue-FadeValue) * FadeSpeed);
		if(FadeValue>0.01f)
		{
			if(Rend) {
				Rend.enabled = true;
				Color NewCol = Rend.color;
				NewCol.a = FadeValue;
				Rend.color = NewCol;
			}
		}
		else
		{
			if(Rend) {
				Rend.enabled = false;
			}
		}
    }

    //--------------------------------------------------------------

    //Singleton variable
	private static FadeMgr s_instance = null;

    //GameManager singleton declaration
	public static FadeMgr instance
    {
        get
        {
            //Get instance in current scene
            if (s_instance == null)
            {
				s_instance = FindObjectOfType(typeof(FadeMgr)) as FadeMgr;
            }

            return s_instance;
        }
    }
}
