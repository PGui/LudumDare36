using UnityEngine;
using System.Collections;


public enum EEnemyType
{
	JewelA,
	JewelB,
	JewelC,
	JewelD,
	BonbonA,
	BonbonB,
	BonbonC,
	BonbonD,
}

public enum EPattern
{
    SIN_RIGHT_TO_LEFT,
	SIN_RIGHT_TO_LEFT_REVERSED,
    COS_RIGHT_TO_LEFT,
	COS_RIGHT_TO_LEFT_REVERSED,
    RANDOMPOINT,
    COUNT,
}

public enum ESpawnLocation
{
	RANDOM,
	CENTER,
	TOP,
	BOTTOM,
}


public class SpawnerMgr : MonoBehaviour {


    public float DurationBetweenTwoSpawn = 10.0f;
    //float Elapsed = 0.0f;
    //bool CanSpawn = true;
    public Camera CameraComponent;

    public bool DebugSpawnEnnemy = false;

    public float TimeBetweenTwoEggs = 10.0f;

    public GameObject[] SpawnableEnnemies;
    public GameObject JewelA;
    public GameObject JewelB;
    public GameObject JewelC;
    public GameObject JewelD;
    public GameObject BonbonA;
    public GameObject BonbonB;
    public GameObject BonbonC;
    public GameObject BonbonD;
	public GameObject Boss;
    public GameObject Egg;
    public GameObject FXEggSpawn;

    // Use this for initialization
    void Start () {
        CameraComponent = GetComponent<Camera>();
        //SpawnEggs(20, 10);
    }
	
	// Update is called once per frame
	void Update () {
	
        /*if(CanSpawn && DebugSpawnEnnemy)
        {
            CanSpawn = false;
            StartCoroutine(ManageSpawn());
            SpawnWave(Random.Range(20,30), Random.Range(5.0f, 15.0f), (EPattern)Random.Range((int)EPattern.SIN_RIGHT_TO_LEFT, (int)EPattern.COUNT));
        }*/
	}

    IEnumerator ManageSpawn()
    {
        yield return new WaitForSeconds(DurationBetweenTwoSpawn);
        //CanSpawn = true;
    }

	public GameObject SpawnBoss(Vector3 SpawnPosition)
	{
		GameObject NewBoss = GameObject.Instantiate(Boss, SpawnPosition, Quaternion.identity) as GameObject;

		return NewBoss;
	}

    IEnumerator SpawnEnnemies(EEnemyType type, float moveSpeed, float TimeBetweenTwoEnnemies , int EnnemyAmout, EPattern Pattern, Vector3 SpawnPosition)
    {
        int EnnemiesSpawned = 0;
        while(EnnemiesSpawned != EnnemyAmout)
        {
            //GameObject GoToSpawn = SpawnableEnnemies[Random.Range(0, SpawnableEnnemies.Length - 1)];
			GameObject GoToSpawn;
			switch (type)
			{
				default:
				case EEnemyType.JewelA: GoToSpawn = JewelA; break;
				case EEnemyType.JewelB: GoToSpawn = JewelB; break;
				case EEnemyType.JewelC: GoToSpawn = JewelC; break;
				case EEnemyType.JewelD: GoToSpawn = JewelD; break;
				case EEnemyType.BonbonA: GoToSpawn = BonbonA; break;
				case EEnemyType.BonbonB: GoToSpawn = BonbonB; break;
				case EEnemyType.BonbonC: GoToSpawn = BonbonC; break;
				case EEnemyType.BonbonD: GoToSpawn = BonbonD; break;
			}

            GameObject NewEnnemy = GameObject.Instantiate(GoToSpawn, SpawnPosition, Quaternion.identity) as GameObject;
            NewEnnemy.GetComponent<EnnemyBehavior>().CurrentPattern = Pattern;
            NewEnnemy.GetComponent<EnnemyBehavior>().MoveSpeed = moveSpeed;
            ++EnnemiesSpawned;
            yield return new WaitForSeconds(TimeBetweenTwoEnnemies);
        }

    }

    public void SpawnWave(EEnemyType type, float moveSpeed, int EnnemyAmout, float SpawnDuration, EPattern Pattern, ESpawnLocation location)
    {
        int EnnemiesPerSecond = EnnemyAmout / (int)SpawnDuration;
        float TimeBetweenTwoEnnemies = 1.0f / (float)EnnemiesPerSecond;
        Vector3 StartPosition;

		switch (location)
		{
			default:
			case ESpawnLocation.CENTER:
                StartPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, 10));
                break;
			case ESpawnLocation.TOP:
                StartPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.75f, 10));
                break;
			case ESpawnLocation.BOTTOM:
                StartPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.25f, 10));
                break;
			case ESpawnLocation.RANDOM:
                StartPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, Random.Range(0.0f,1.0f), 10));
                break;
		}

        StartCoroutine(SpawnEnnemies(type, moveSpeed, TimeBetweenTwoEnnemies, EnnemyAmout, Pattern, StartPosition));

        /*switch (Pattern)
        {
            case EPattern.SIN_RIGHT_TO_LEFT:
			case EPattern.SIN_RIGHT_TO_LEFT_REVERSED:
                break;
            case EPattern.RANDOMPOINT:
                StartCoroutine(SpawnEnnemies(TimeBetweenTwoEnnemies, EnnemyAmout, Pattern, StartPosition));
                break;
            default:
                break;
        }*/
    }

    IEnumerator SpawnEggsCouroutine(float TimeBetweenTwoEggs, int EggsAmout)
    {
		float spawnTime = 1.0f;

        int EggsSpawned = 0;
        while (EggsSpawned != EggsAmout)
        {
            Vector3 SpawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.3f, 0.8f), Random.Range(0.1f, 0.9f), 10));
            GameObject FXEggSpawned = GameObject.Instantiate(FXEggSpawn, SpawnPosition, Quaternion.identity) as GameObject;
            //while(FXEggSpawned)
            //{
            //    yield return null;
            //}

            yield return new WaitForSeconds(spawnTime);

            //GameObject NewEgg = GameObject.Instantiate(Egg, SpawnPosition, Quaternion.identity) as GameObject;
            GameObject.Instantiate(Egg, SpawnPosition, Quaternion.identity);
            ++EggsSpawned;
            yield return new WaitForSeconds(Mathf.Max(0.0f, TimeBetweenTwoEggs - spawnTime));
        }

    }

    public void SpawnEggs(int EggsAmount, float SpawnDuration)
    {
        //int EggsPerSecond = EggsAmount / (int)SpawnDuration;
        //float TimeBetweenTwoEggs = 1.0f / (float)EggsPerSecond;
        
        StartCoroutine(SpawnEggsCouroutine(SpawnDuration, EggsAmount));
    }

    //Singleton variable
    private static SpawnerMgr s_instance = null;

    //GameManager singleton declaration
    public static SpawnerMgr instance
    {
        get
        {
            //Get instance in current scene
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(SpawnerMgr)) as SpawnerMgr;
            }

            return s_instance;
        }
    }


}
