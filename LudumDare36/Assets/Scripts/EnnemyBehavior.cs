using UnityEngine;
using System.Collections;

public class EnnemyBehavior : MonoBehaviour {


    public int ScoreToGive {get; set;}
    //private SpawnerMgr SpawnerMgr;
    private float Elapsed = 0.0f;
    public EPattern CurrentPattern;

    private Vector3 InitPosition;
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
        transform.position = InitPosition + AxisRight * y * Magnitude;
    }

    void RandomPoint()
    {
        transform.position = Vector3.Lerp(transform.position, RandomPointOnScreen, Time.deltaTime * Random.Range(1.0f, 5.0f));
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
