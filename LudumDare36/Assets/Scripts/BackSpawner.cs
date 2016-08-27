using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackSpawner : MonoBehaviour {

    public List<GameObject> ToSpawn;

    public float MinDist = 3.0f;
    public float MaxDist = 5.0f;

    public float RandHeight = 10.0f;
    public float Scaler = 1.0f;

    public bool Enable = true;

    private float NextSpawn;

    void ComputeNextSpawn() {
        NextSpawn += Random.Range(MinDist, MaxDist);
    }

    // Use this for initialization
    void Start () {

        if (Enable) {

            float x = -40.0f;
            while (x < transform.position.x)
            {
                Vector3 Pos = transform.position;
                SpawnItem(new Vector3(x, Pos.y + Random.Range(-RandHeight, RandHeight) * 0.5f, 0));

                x += Random.Range(MinDist, MaxDist);
            }
        }

        NextSpawn = 0.0f;
        ComputeNextSpawn();
    }
    void SpawnItem(Vector3 NewPos) {
        int Item = Random.Range(0, ToSpawn.Count);
        GameObject NewObj = Instantiate(ToSpawn[Item], NewPos, Quaternion.identity) as GameObject;
        NewObj.transform.localScale *= Scaler;
    }

	
	// Update is called once per frame
	void Update () {

        float speed = 0.3f;
        float horiz = 3.0f;
        float Paralax = 0.5f;

        float dt = Time.deltaTime;

        float relHeight = Mathf.Abs((transform.position.y - horiz) * Paralax);
        float curspeed = speed * relHeight * relHeight;

        NextSpawn -= dt*curspeed;
        if(NextSpawn<0.0f)
        {
            if (Enable && ToSpawn.Count > 0)
            {
                Vector3 Pos = transform.position;
                SpawnItem(new Vector3(Pos.x, Pos.y + Random.Range(-RandHeight, RandHeight)*0.5f, 0));
            }
            ComputeNextSpawn();
        }


    }
}
