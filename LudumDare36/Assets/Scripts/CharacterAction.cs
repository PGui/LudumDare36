using UnityEngine;
using System.Collections;

public class CharacterAction : MonoBehaviour {

    public GameObject ObjectToShoot;
    private Vector2 ShootDirection;
    public float Force = 100.0f;
    public float CooldownDuration = 0.2f;
    private bool CanShoot = true;


	// Use this for initialization
	void Start () {
        ShootDirection = new Vector2(1.0f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetButton("Fire1") && CanShoot)
        {
            StartCoroutine("Cooldown");
            CanShoot = false;
            ShootDirection = ShootDirection.normalized;
            GameObject Projectile = GameObject.Instantiate(ObjectToShoot, this.transform.position, Quaternion.identity) as GameObject;
            Projectile.GetComponent<Rigidbody2D>().AddForce(ShootDirection * Force, ForceMode2D.Impulse);

        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(CooldownDuration);
        CanShoot = true;
    }
}
