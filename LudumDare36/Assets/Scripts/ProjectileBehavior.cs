using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour {

    public float Speed = 90.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.position += Time.deltaTime * Speed * transform.right;
    }
}
