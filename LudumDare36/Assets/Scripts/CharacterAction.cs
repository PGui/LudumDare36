using UnityEngine;
using System.Collections;

public class CharacterAction : MonoBehaviour {

    public GameObject ObjectToShoot;
    private Vector2 ShootDirection;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetButtonDown("Fire1"))
        {
            ShootDirection = Vector3.Normalize((Input.mousePosition - this.transform.position));

            GameObject Projectile = GameObject.Instantiate(ObjectToShoot, this.transform.position, Quaternion.identity) as GameObject;

            Projectile.GetComponent<Rigidbody2D>().AddForce(ShootDirection * 100, ForceMode2D.Impulse);

        }
        

    }
}
