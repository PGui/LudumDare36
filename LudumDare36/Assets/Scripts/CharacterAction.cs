using UnityEngine;
using System.Collections;

public class CharacterAction : MonoBehaviour {

    public GameObject ObjectToShoot;
    private Vector2 ShootDirection;
    public float Force = 100.0f;
    public float CooldownDuration = 0.2f;
    private bool CanShoot = true;
    public GameObject Camera;


    private ScreenshakeMgr SSMgr = null;
    private CharacterMovement CharacMvt= null;
    private FreezeFrameMgr FreezeFrameMgr = null;

	// Use this for initialization
	void Start () {
        
        ShootDirection = new Vector2(1.0f, 0.0f);
        if(Camera)
        {
            SSMgr = Camera.GetComponent<ScreenshakeMgr>();
            FreezeFrameMgr = Camera.GetComponent<FreezeFrameMgr>();
        }

        CharacMvt = GetComponent<CharacterMovement>();
        
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetButton("Fire1"))
        {
            CharacMvt.IsShooting = true;
            if (CanShoot)
            {
                StartCoroutine("Cooldown");
                CanShoot = false;
                ShootDirection = ShootDirection.normalized;
                GameObject Projectile = GameObject.Instantiate(ObjectToShoot, this.transform.position, Quaternion.identity) as GameObject;
                Projectile.GetComponent<Rigidbody2D>().AddForce(ShootDirection * Force, ForceMode2D.Impulse);
                SSMgr.StartShake(0.3f, 0.8f, 8.0f);
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            CharacMvt.IsShooting = false;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(CooldownDuration);
        CanShoot = true;
    }
}
