﻿using UnityEngine;
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
    //private FreezeFrameMgr FreezeFrameMgr = null;

    private Animator PlayerAnimator;

    private PlayerState CurrentPlayerState;

    public GameObject ArmsFront;
    public GameObject ArmsBack;
    public bool AllowShoot = false;



    // Use this for initialization
    void Start () {
        
        ShootDirection = new Vector2(1.0f, 0.0f);
        if(Camera)
        {
            SSMgr = Camera.GetComponent<ScreenshakeMgr>();
            //FreezeFrameMgr = Camera.GetComponent<FreezeFrameMgr>();
        }

        CharacMvt = GetComponent<CharacterMovement>();
        PlayerAnimator = GetComponent<Animator>();

        CurrentPlayerState = FindObjectOfType<PlayerState>().GetComponent<PlayerState>();
    }
	
	// Update is called once per frame
	void Update () {

        if (CurrentPlayerState.CurrentState == PlayerState.EPlayerState.DEAD || !AllowShoot)
		{
	        PlayerAnimator.SetBool("Shooting", false);
            ArmsFront.SetActive(false);
            ArmsBack.SetActive(false);
            return;
		}

        if(Input.GetButton("Fire1"))
        {
            CharacMvt.IsShooting = true;
            ArmsFront.SetActive(true);
            ArmsBack.SetActive(true);

            if (CanShoot)
            {
                if(PlayerState.instance.FirstShoot)
                {
                    PlayerState.instance.FirstShoot = false;
                    Scenario.instance.PlayNextStep();
                }

                StartCoroutine("Cooldown");
                CanShoot = false;
                ShootDirection = ShootDirection.normalized;
				GameObject.Instantiate(ObjectToShoot, this.transform.position, Quaternion.identity);
                //GameObject Projectile = GameObject.Instantiate(ObjectToShoot, this.transform.position, Quaternion.identity) as GameObject;
                //Projectile.GetComponent<Rigidbody2D>().AddForce(ShootDirection * Force, ForceMode2D.Impulse);
                SSMgr.StartShake(0.3f, 0.8f, 2.0f);
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {

            CharacMvt.IsShooting = false;
            ArmsFront.SetActive(false);
            ArmsBack.SetActive(false);
        }

        PlayerAnimator.SetBool("Shooting", CharacMvt.IsShooting);

        //if(CharacMvt.PlayerAnimator.GetInteger("Direction") == 1 && CharacMvt.IsShooting)
        //{
        //    float offsetX = 5.0f;
        //    Vector3 newArmBackPosWithOffset = new Vector3(ArmsBack.transform.position.x - offsetX, ArmsBack.transform.position.y, ArmsBack.transform.position.z);
        //    Vector3 newArmFrontPosWithOffset = new Vector3(ArmsFront.transform.position.x - offsetX, ArmsFront.transform.position.y, ArmsFront.transform.position.z);
        //}
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(CooldownDuration);
        CanShoot = true;
    }
}
