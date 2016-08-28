using UnityEngine;
using System.Collections;


public enum EPattern
{
    SIN_RIGHT_TO_LEFT,
    RANDOMPOINT,
    COUNT,
}


public class SpawnerMgr : MonoBehaviour {


    public float DurationBetweenTwoSpawn = 10.0f;
    float Elapsed = 0.0f;
    bool CanSpawn = true;
    public Camera CameraComponent;

    public bool DebugSpawnEnnemy = false;

    public float TimeBetweenTwoEggs = 10.0f;

    public GameObject[] SpawnableEnnemies;
    public GameObject Egg;
    public GameObject FXEggSpawn;

    // Use this for initialization
    void Start () {
        CameraComponent = GetComponent<Camera>();
        //SpawnEggs(20, 10);
    }
	
	// Update is called once per frame
	void Update () {
	
        if(CanSpawn && DebugSpawnEnnemy)
        {
            CanSpawn = false;
            StartCoroutine(ManageSpawn());
            SpawnWave(Random.Range(20,30), Random.Range(5.0f, 15.0f), (EPattern)Random.Range((int)EPattern.SIN_RIGHT_TO_LEFT, (int)EPattern.COUNT));
        }
	}

    IEnumerator ManageSpawn()
    {
        yield return new WaitForSeconds(DurationBetweenTwoSpawn);
        CanSpawn = true;
    }

    IEnumerator SpawnEnnemies(float TimeBetweenTwoEnnemies , int EnnemyAmout, EPattern Pattern, Vector3 SpawnPosition)
    {
        int EnnemiesSpawned = 0;
        while(EnnemiesSpawned != EnnemyAmout)
        {
            GameObject GoToSpawn = SpawnableEnnemies[Random.Range(0, SpawnableEnnemies.Length - 1)];
            GameObject NewEnnemy = GameObject.Instantiate(GoToSpawn, SpawnPosition, Quaternion.identity) as GameObject;
            NewEnnemy.GetComponent<EnnemyBehavior>().CurrentPattern = Pattern;
            ++EnnemiesSpawned;
            yield return new WaitForSeconds(TimeBetweenTwoEnnemies);
        }

    }

    public void SpawnWave(int EnnemyAmout, float SpawnDuration, EPattern Pattern)
    {
        int EnnemiesPerSecond = EnnemyAmout / (int)SpawnDuration;
        float TimeBetweenTwoEnnemies = 1.0f / (float)EnnemiesPerSecond;
        Vector3 StartPosition;
        switch (Pattern)
        {
            case EPattern.SIN_RIGHT_TO_LEFT:
                StartPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, 10));
                StartCoroutine(SpawnEnnemies(TimeBetweenTwoEnnemies, EnnemyAmout, Pattern, StartPosition));
                break;
            case EPattern.RANDOMPOINT:
                StartPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, Random.Range(0.0f,1.0f), 10));
                StartCoroutine(SpawnEnnemies(TimeBetweenTwoEnnemies, EnnemyAmout, Pattern, StartPosition));
                break;
            default:
                break;
        }
        
    }

    IEnumerator SpawnEggsCouroutine(float TimeBetweenTwoEggs, int EggsAmout)
    {
        int EggsSpawned = 0;
        while (EggsSpawned != EggsAmout)
        {
            Vector3 SpawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.3f, 0.8f), Random.Range(0.1f, 0.9f), 10));
            GameObject FXEggSpawned = GameObject.Instantiate(FXEggSpawn, SpawnPosition, Quaternion.identity) as GameObject;
            while(FXEggSpawned)
            {
                yield return null;
            }

            GameObject NewEgg = GameObject.Instantiate(Egg, SpawnPosition, Quaternion.identity) as GameObject;
            ++EggsSpawned;
            yield return new WaitForSeconds(TimeBetweenTwoEggs);
        }

    }

    public void SpawnEggs(int EggsAmount, float SpawnDuration)
    {
        int EggsPerSecond = EggsAmount / (int)SpawnDuration;
        float TimeBetweenTwoEggs = 1.0f / (float)EggsPerSecond;
        
        StartCoroutine(SpawnEggsCouroutine(TimeBetweenTwoEggs, EggsAmount));
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
