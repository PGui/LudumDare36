using UnityEngine;
using System.Collections;

public class Lifetime : MonoBehaviour {

    public float LifetimeValue = 5.0f;

	// Use this for initialization
	void Start () {
        GameObject.Destroy(this.gameObject, LifetimeValue);
    }
}
