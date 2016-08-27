﻿using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {



    public float Speed = 20.0f;
    public Vector2 MaxVelocity = new Vector2(100,100);
    public AnimationCurve AccelerationCurve;
    public float Inertia = 350.0f;

    private float AccelerationX = 0.0f;
    private float AccelerationY = 0.0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        float HorizontalAxis = Input.GetAxis("Horizontal");
        float VerticalAxis = Input.GetAxis("Vertical");

        

        if (HorizontalAxis > 0.1f && Input.GetAxisRaw("Horizontal") != 0)
        {
            AccelerationX = AccelerationCurve.Evaluate(HorizontalAxis);
        }
        else if (HorizontalAxis < -0.1f && Input.GetAxisRaw("Horizontal") != 0)
        {
            AccelerationX = -1.0f * AccelerationCurve.Evaluate(-1.0f * HorizontalAxis);
        }
        else
        {
            AccelerationX = Mathf.Lerp(AccelerationX, 0.0f, Inertia * Time.deltaTime);
        }

        
        if (VerticalAxis > 0.1f && Input.GetAxisRaw("Vertical") != 0)
        {
            AccelerationY = AccelerationCurve.Evaluate(VerticalAxis);
        }
        else if (VerticalAxis < -0.1f && Input.GetAxisRaw("Vertical") != 0)
        {
            AccelerationY = -1.0f * AccelerationCurve.Evaluate(-1.0f * VerticalAxis);
        }
        else
        {
            AccelerationY = Mathf.Lerp(AccelerationY, 0.0f, Inertia * Time.deltaTime);
        }

        this.transform.position += Speed * Time.deltaTime * new Vector3(AccelerationX, AccelerationY);
    }
}
