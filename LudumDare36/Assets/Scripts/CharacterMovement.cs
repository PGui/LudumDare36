using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {



    public float Speed = 20.0f;
    public AnimationCurve AccelerationCurve;
    public float Inertia = 350.0f;

    private float AccelerationX = 0.0f;
    private float AccelerationY = 0.0f;

    public bool IsShooting { get; set; }
    public float ShootingDiviser = 2.0f;
    public Animator PlayerAnimator { get; set; }

    private Vector3 PreviousPosition;

    private PlayerState PlayerState;

    public bool CanMove = false;

    // Use this for initialization
    void Start () {
        IsShooting = false;
        PlayerAnimator = GetComponent<Animator>();

        PreviousPosition = this.transform.position;

        PlayerState = FindObjectOfType<PlayerState>().GetComponent<PlayerState>();
    }
	
	// Update is called once per frame
	void Update ()
    {

        if (PlayerState.CurrentState == PlayerState.EPlayerState.DEAD || !CanMove)
            return;

        var bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));

        var cameraRect = new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y);


        float HorizontalAxis = Input.GetAxis("Horizontal");
        float VerticalAxis = Input.GetAxis("Vertical");

        if(Mathf.Abs(HorizontalAxis) + Mathf.Abs(VerticalAxis) > 0 && PlayerState.instance.FirstMove)
        {
            PlayerState.instance.FirstMove = false;
            Scenario.instance.PlayNextStep();
        }
        
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

        this.transform.position += (IsShooting ? Speed/ShootingDiviser : Speed) * Time.deltaTime * new Vector3(AccelerationX, AccelerationY);

        if(this.transform.position.x > cameraRect.x+ cameraRect.width)
        {
            this.transform.position = new Vector3(cameraRect.x + cameraRect.width, this.transform.position.y, transform.position.z);
        }

        if (this.transform.position.x < cameraRect.x)
        {
            this.transform.position = new Vector3(cameraRect.x, this.transform.position.y, transform.position.z);
        }

        if (this.transform.position.y > cameraRect.y + cameraRect.height)
        {
            this.transform.position = new Vector3(this.transform.position.x, cameraRect.y + cameraRect.height, transform.position.z);
        }

        if (this.transform.position.y < cameraRect.y)
        {
            this.transform.position = new Vector3(this.transform.position.x, cameraRect.y, transform.position.z);
        }


        Vector3 Direction = (PreviousPosition - this.transform.position).normalized;

        if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y) && Mathf.Abs(HorizontalAxis) > 0.0f)
        {
            //Priority left right
            if(Direction.x > 0.0f)
            {
                PlayerAnimator.SetInteger("Direction", 1);
            }
            else
            {
                PlayerAnimator.SetInteger("Direction",2);
            }
        }
        else if (Mathf.Abs(Direction.x) < Mathf.Abs(Direction.y) && Mathf.Abs(VerticalAxis) > 0.0f)
        {
            if (Direction.y > 0.0f)
            {
                PlayerAnimator.SetInteger("Direction", 4);
            }
            else
            {
                PlayerAnimator.SetInteger("Direction", 3);
            }
        }
        else
        {
            PlayerAnimator.SetInteger("Direction", 0);
        }

        PreviousPosition = this.transform.position;

    }

	
    //Singleton variable
    private static CharacterMovement s_instance = null;

    //GameManager singleton declaration
    public static CharacterMovement instance
    {
        get
        {
            //Get instance in current scene
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(CharacterMovement)) as CharacterMovement;
            }

            return s_instance;
        }
    }

}
