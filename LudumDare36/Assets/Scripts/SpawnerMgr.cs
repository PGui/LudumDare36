using UnityEngine;
using System.Collections;

public class SpawnerMgr : MonoBehaviour {


    public float DurationBetweenTwoSpawn = 10.0f;
    float Elapsed = 0.0f;
    bool CanSpawn = true;
    public Camera CameraComponent;

    public enum EPattern
    {
        SIN_RIGHT_TO_LEFT,
    }

    public GameObject[] SpawnableEnnemies;


	// Use this for initialization
	void Start () {
        CameraComponent = GetComponent<Camera>();

    }
	
	// Update is called once per frame
	void Update () {
	
        if(CanSpawn)
        {
            CanSpawn = false;
            StartCoroutine(ManageSpawn());
            SpawnWave(Random.Range(20,40), Random.Range(5.0f, 15.0f), EPattern.SIN_RIGHT_TO_LEFT);
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
        switch (Pattern)
        {
            case EPattern.SIN_RIGHT_TO_LEFT:
                Vector3 StartPosition = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, 10));
                StartCoroutine(SpawnEnnemies(TimeBetweenTwoEnnemies, EnnemyAmout, Pattern, StartPosition));
                break;
            default:
                break;
        }
        
    }


}
