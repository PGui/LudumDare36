using UnityEngine;
using System.Collections;

public class BossBehavior : MonoBehaviour
{
    public int ScoreToGive {get; set;}
    //private SpawnerMgr SpawnerMgr;
    private float Elapsed = 0.0f;

	private float Life = 100.0f;
	private bool Invincible = false;
	private bool IsDying = false;
	private float DeathTimer = 0.0f;
	private float DeathSub = 0.0f;

	private float WaveTimer = 2.0f;

	private float Dammage = 2.0f;

	[HideInInspector]
    public float MoveSpeed = 5.0f;

    public GameObject DestructionParticles;

    private Vector3 FirstGoal;

	private float LastHitDuration = 0.0f;

    // Use this for initialization
    void Start ()
	{
        ScoreToGive = 100;

		FirstGoal = Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0.5f, 10));
    }

	IEnumerator CooldownInvincible()
	{
		yield return new WaitForSeconds(1.0f);
		Invincible = false;
	}

	void Update () {
		Elapsed += Time.deltaTime;

		Vector3 NewPos = Vector3.Lerp(transform.position, FirstGoal, Mathf.Min(1.0f, Elapsed*0.1f));
		NewPos.y = Mathf.Sin(Elapsed * 0.5f) *9.0f;
		transform.position = NewPos;

		Quaternion NewRot = new Quaternion();
		NewRot.eulerAngles = new Vector3(0,0,Mathf.Sin(Elapsed * 2.0f)*35.0f-10.0f);
		transform.localRotation = NewRot;

		SpriteRenderer Rend = gameObject.GetComponent<SpriteRenderer>();
		if(Rend)
		{
			LastHitDuration = Mathf.Max(LastHitDuration - Time.deltaTime, 0.0f);
			Rend.color = Color.Lerp(Color.white, Color.red, Mathf.Clamp01(LastHitDuration));
		}

		if(IsDying)
		{
			float DeathDuration = 5.0f;
			DeathTimer += Time.deltaTime;
			if(Rend)
			{
				Rend.color = Color.Lerp(Color.white, Color.red, Mathf.Clamp01(DeathTimer/2.0f));
			}
			if(DeathTimer > DeathDuration) {
				DestroyEnnemy(gameObject, true);

				EnnemyBehavior[] Enemies = GameObject.FindObjectsOfType<EnnemyBehavior>();
				int EnemyCount = Enemies.GetLength(0);
				for(int i=0; i<EnemyCount; ++i) {
					EnnemyBehavior CurEnemy = Enemies[i];
					if(CurEnemy) {
						CurEnemy.DestroyEnnemy(null, true);
					}
				}

				return;
			}

			DeathSub -= Time.deltaTime;
			if(DeathSub<0.0f) {
				DeathSub = 0.05f;
				EnnemyBehavior CurEnemy = GameObject.FindObjectOfType<EnnemyBehavior>();
				if(CurEnemy) {
					CurEnemy.DestroyEnnemy(null, true);
				}
			}
		}
		else
		{
			WaveTimer -= Time.deltaTime;
			if(WaveTimer<=0.0f) {
				float WaveDur = Random.Range(5.0f,10.0f);
				WaveTimer = WaveDur;

				SpawnerMgr.instance.SpawnWave(EEnemyType.JewelC, 5.0f, 30, WaveDur, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
				SpawnerMgr.instance.SpawnWave(EEnemyType.JewelD, 5.0f, 30, WaveDur, EPattern.SIN_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.CENTER);
			}
		}
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
		if (!Invincible && !IsDying && coll.gameObject.tag == "Projectile")
        {
			Life = Mathf.Max(0.0f,Life - Dammage);

			LastHitDuration = 1.0f;

			if(Life<=0.0f) {
				IsDying = true;
				Scenario.instance.PlayNextStep();
			}
			else
			{
				Invincible = true;
				StartCoroutine(CooldownInvincible());
			}
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
