using UnityEngine;
using System.Collections;

public class SpawnerMgr : MonoBehaviour {


    public float DurationBetweenTwoSpawn = 10.0f;
    float Elapsed = 0.0f;
    bool CanSpawn = true;
    public Camera CameraComponent;

    public bool DebugSpawnEnnemy = false;

    public enum EPattern
    {
        SIN_RIGHT_TO_LEFT,
        RANDOMPOINT,
        COUNT,
    }

    public GameObject[] SpawnableEnnemies;


	// Use this for initialization
	void Start () {
        CameraComponent = GetComponent<Camera>();

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
