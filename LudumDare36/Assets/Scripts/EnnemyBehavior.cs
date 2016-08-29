using UnityEngine;
using System.Collections;

public class EnnemyBehavior : MonoBehaviour
{
    public int ScoreToGive {get; set;}
    //private SpawnerMgr SpawnerMgr;
    private float Elapsed = 0.0f;

    private Vector3 InitPosition;

	[HideInInspector]
    public EPattern CurrentPattern;

	[HideInInspector]
    public float MoveSpeed = 5.0f;

    public float Frequency = 20.0f;  // Speed of sine movement
    public float Magnitude = 0.5f;   // Size of sine movement

    public GameObject DestructionParticles;

    private Vector3 AxisRight;

    private Vector3 RandomPointOnScreen;

    // Use this for initialization
    void Start ()
	{
        //SpawnerMgr = Object.FindObjectOfType<SpawnerMgr>().GetComponent< SpawnerMgr>();
        InitPosition = this.transform.position;

        AxisRight = transform.up;
        ScoreToGive = 100;

        RandomPointOnScreen = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.4f,0.9f), Random.Range(0.1f, 0.9f), 10));
    }
	
	// Update is called once per frame
	void Update () {
        Elapsed += Time.deltaTime;

        switch(CurrentPattern)
        {
            case EPattern.SIN_RIGHT_TO_LEFT:
                DoSinRightToLeft(false);
                break;
            case EPattern.SIN_RIGHT_TO_LEFT_REVERSED:
                DoSinRightToLeft(true);
                break;
            case EPattern.COS_RIGHT_TO_LEFT:
                DoCosRightToLeft(false);
                break;
            case EPattern.COS_RIGHT_TO_LEFT_REVERSED:
                DoCosRightToLeft(true);
                break;
            case EPattern.RANDOMPOINT:
                RandomPoint();
                break;
			case EPattern.LASER:
				DoLaser(false);
				break;
			case EPattern.SPIRAL:
				DoSpiral(false);
				break;
            default:
                DoSinRightToLeft(false);
                break;
        }
    }

    void DoSinRightToLeft(bool reverseY)
    {
        InitPosition += -transform.right * Time.deltaTime * MoveSpeed;
		float y = Mathf.Sin(Elapsed * Frequency);
		if (reverseY) y = -y;
        transform.position = InitPosition + AxisRight * y * Magnitude;
    }

    void DoCosRightToLeft(bool reverseY)
    {
        InitPosition += -transform.right * Time.deltaTime * MoveSpeed;
		float y = Mathf.Cos(Elapsed * Frequency);
		if (reverseY) y = -y;
        transform.position = InitPosition + AxisRight * y * (Magnitude * 0.5f);
    }

    void RandomPoint()
    {
        transform.position = Vector3.Lerp(transform.position, RandomPointOnScreen, Time.deltaTime * Random.Range(1.0f, 5.0f));
    }

	void DoLaser(bool reverseY)
	{
		float x = -Mathf.Abs(Mathf.Cos(Time.time * Frequency));
		float y = Mathf.Sin(Time.time * Frequency)*0.3f;
		if (reverseY) y = -y;
		float Dist = (Magnitude * MoveSpeed * Time.deltaTime + Mathf.Sin(Elapsed*10.0f));
		transform.position += transform.right * x * Dist + AxisRight * y * Dist;
	}

	void DoSpiral(bool reverseY)
	{
		float Angle = Time.time * Frequency + Mathf.Sin(Elapsed*1.0f);
		float x = -Mathf.Abs(Mathf.Cos(Angle));
		float y = Mathf.Sin(Angle)*0.3f;
		if (reverseY) y = -y;
		float Dist = (Magnitude * MoveSpeed * Elapsed);
		transform.position = InitPosition + transform.right * x * Dist + AxisRight * y * Dist;
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Projectile")
        {
            DestroyEnnemy(coll.gameObject, true);
        }
    }

    public void DestroyEnnemy(GameObject other, bool AddScore = false)
    {
        if (DestructionParticles)
        {
            GameObject.Instantiate(DestructionParticles, this.transform.position, Quaternion.identity);
			
			GameAudio.instance.PlaySFx(ESFx.Explode);
        }

        if(AddScore)
            FindObjectOfType<PlayerState>().SendMessage("AddScore", ScoreToGive);
        GameObject.Destroy(this.gameObject);
        if(other)
            GameObject.Destroy(other);
    }

}
