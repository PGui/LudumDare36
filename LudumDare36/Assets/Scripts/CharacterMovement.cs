using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {



    public float Speed = 20.0f;
    public float Inertia = 2.5f;
    public Vector2 MaxVelocity = new Vector2(100,100);
    public AnimationCurve AccelerationCurve;
    public AnimationCurve DecelerationCurve;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        float HorizontalAxis = Input.GetAxis("Horizontal");
        float VerticalAxis = Input.GetAxis("Vertical");

        float AccelerationX = 0.0f;
        float AccelerationY = AccelerationCurve.Evaluate(VerticalAxis);


        if (HorizontalAxis > 0.1f)
        {
            AccelerationX = AccelerationCurve.Evaluate(HorizontalAxis);
        }
        else if (HorizontalAxis < -0.1f)
        {
            AccelerationX = -1.0f * AccelerationCurve.Evaluate(-1.0f * HorizontalAxis);
        }

        
        if (VerticalAxis > 0.1f)
        {
            AccelerationY = AccelerationCurve.Evaluate(VerticalAxis);
        }
        else if (VerticalAxis < -0.1f)
        {
            AccelerationY = -1.0f * AccelerationCurve.Evaluate(-1.0f * VerticalAxis);
        }

        this.transform.position = new Vector3(transform.position.x + AccelerationX * Speed * Time.deltaTime, transform.position.y + Speed * AccelerationY * Time.deltaTime, this.transform.position.z);
    }
}
