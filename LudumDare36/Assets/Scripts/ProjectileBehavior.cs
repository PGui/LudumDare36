using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour {

    public float Speed = 90.0f;

	// Use this for initialization
	void Start ()
	{
		GameAudio.instance.PlaySFx(ESFx.Whoosh);

		transform.Rotate(Vector3.forward * Random.Range(0,360));
	}
	
	// Update is called once per frame
	void Update ()
	{
        transform.position += new Vector3(Time.deltaTime * Speed, 0, 0);
    }
}
