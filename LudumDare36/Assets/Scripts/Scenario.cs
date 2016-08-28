using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scenario : MonoBehaviour
{
	//--------------------------------------------------------------

	public bool debugRoutine = false;

	public bool skipTutorial = false;

	public GameObject startArea;

	public Text textTitle;
	public Text textInput;
	public Image imageAvatar;
	public Text textAvatar;

	//--------------------------------------------------------------
	
	public void PlayScenario()
	{
		print("Scenario - Start");

		StartCoroutine(PlayScenarioRoutine());
	}

	public void PlayNextStep()
	{
		pauseRoutine = false;

		print("Scenario - NextStep");
	}
	
	//--------------------------------------------------------------

	private bool pauseRoutine;
	private bool waitSpacebar = false;
	
	void Start ()
	{
		if (!debugRoutine)
		{


			ResetScenario();
			PlayScenario();
		}
	}
	
	void Update ()
	{
		if (!debugRoutine)
		{
			if (waitSpacebar && Input.GetButtonDown("Fire1"))
			{
				waitSpacebar = false;
				PlayNextStep();
			}
		}
		else
		{
			if (Input.GetButtonDown("Fire1"))
			{
				PlayNextStep();
			}

			if (Input.GetButtonDown("Fire2"))
			{
				ResetScenario();
				PlayScenario();
			}
		}
	}

	public void ResetScenario()
	{
		pauseRoutine = false;
		waitSpacebar = false;

		textTitle.CrossFadeAlpha(0.0f, 0.0f, false);
		textInput.CrossFadeAlpha(0.0f, 0.0f, false);
		imageAvatar.CrossFadeAlpha(0.0f, 0.0f, false);
		textAvatar.CrossFadeAlpha(0.0f, 0.0f, false);
		textInput.color = new Color(245,245,245);

        PlayerState.instance.AllowPlayerMove(false);
        PlayerState.instance.AllowPlayerShoot(false);
        PlayerState.instance.InitialLife = 100.0f;
        PlayerState.instance.CurrentState = PlayerState.EPlayerState.ALIVE;
        PlayerState.instance.FirstShoot = true;
        PlayerState.instance.FirstMove = true;
    }

	IEnumerator PlayScenarioRoutine()
	{
		//////// Tutorial ////////
		if (skipTutorial)
		{
			PlayerState.instance.AllowPlayerMove(true);
			PlayerState.instance.AllowPlayerShoot(true);
			iTween.MoveTo(startArea, iTween.Hash("position", new Vector3(-100,-40,0), "time", 8.0f));
            GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenA, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenB, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.Wind, true);
		}
		else
		{
			//Show Title
			textTitle.CrossFadeAlpha(1.0f, 2.5f, false);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.Wind, true);
			yield return new WaitForSeconds(3.0f);

			//Ring and hide title
			textTitle.CrossFadeAlpha(0.0f, 2.5f, false);
			AudioSource ringTone = GameAudio.instance.PlaySFxLoop(ESFx.Ring3310);

            GameObject LightPhone = GameObject.Find("LightPhone");
            if(LightPhone)
            {
                SpriteRenderer Rend = LightPhone.GetComponent<SpriteRenderer>();
                if(Rend)
                {
                    Color Tmp = Rend.color;
                    Tmp.a = 1.0f;
                    Rend.color = Tmp;
                }
            }

            yield return new WaitForSeconds(4.0f);
		
			//Show input
			waitSpacebar = true;
			textInput.text = "Press Spacebar to answer phone...";
			textInput.CrossFadeAlpha(1.0f, 1.0f, false);
			yield return PauseRoutine();

			//Answer phone
			textInput.CrossFadeAlpha(0.0f, 1.0f, false);
			GameAudio.instance.StopSFxLoop(ringTone);
			yield return new WaitForSeconds(1.0f);
		
			//Dialogue
			textAvatar.text = "It's time to show the world who's the boss.";
			textAvatar.CrossFadeAlpha(1.0f, 1.0f, false);
			imageAvatar.CrossFadeAlpha(1.0f, 1.0f, false);
			yield return new WaitForSeconds(3.0f);
		
			//Hide Dialogue
			textAvatar.CrossFadeAlpha(0.0f, 1.0f, false);
			imageAvatar.CrossFadeAlpha(0.0f, 1.0f, false);
			yield return new WaitForSeconds(1.0f);

			//Show input
			PlayerState.instance.AllowPlayerMove(true);
			PlayerState.instance.FirstMove = true;
			textInput.text = "Use Z/Q/S/D or W/A/S/D to start moving...";
			textInput.CrossFadeAlpha(1.0f, 1.0f, false);
			yield return PauseRoutine();

			//Exit start area
			textInput.CrossFadeAlpha(0.0f, 1.0f, false);
			iTween.MoveTo(startArea, iTween.Hash("position", new Vector3(-100,-40,0), "time", 4.0f));
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenA, true);
			yield return new WaitForSeconds(3.0f);
		
			//Show input
			PlayerState.instance.AllowPlayerShoot(true);
			PlayerState.instance.FirstShoot = true;
			textInput.text = "Press Spacebar to attack...";
			textInput.CrossFadeAlpha(1.0f, 1.0f, false);
			textInput.color = new Color(0,0,0);
			yield return PauseRoutine();

			//Awaken
			textInput.CrossFadeAlpha(0.0f, 1.0f, false);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenB, true);
			yield return new WaitForSeconds(4.0f);
		}
		
		//////// Wave 1 ////////

		//Fight wave

		//SpawnerMgr.instance.SpawnWave(Random.Range(20,30), Random.Range(5.0f, 15.0f), (EPattern)Random.Range((int)EPattern.SIN_RIGHT_TO_LEFT, (int)EPattern.COUNT));
		//SpawnerMgr.instance.SpawnEggs(2, 10.0f);

		SpawnerMgr.instance.SpawnWave(30, 10.0f, EPattern.SIN_RIGHT_TO_LEFT);
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenA, true);
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenB, true);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.FightA, true);
		yield return new WaitForSeconds(14.0f);
		
		SpawnerMgr.instance.SpawnWave(30, 10.0f, EPattern.RANDOMPOINT);
		yield return new WaitForSeconds(12.0f);
		
		BackMgr.instance.SetBack(EBackground.URBAN);
		SpawnerMgr.instance.SpawnEggs(2, 10.0f);
		SpawnerMgr.instance.SpawnWave(50, 10.0f, EPattern.SIN_RIGHT_TO_LEFT);
		SpawnerMgr.instance.SpawnWave(50, 10.0f, EPattern.SIN_RIGHT_TO_LEFT_REVERSED);
		SpawnerMgr.instance.SpawnWave(50, 10.0f, EPattern.RANDOMPOINT);
		yield return new WaitForSeconds(3.0f);

		//////// Boss 1 ////////

		//Fight boss
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.FightA, false);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.BossA, false);
		yield return PauseRoutine();
	}

	IEnumerator PauseRoutine()
	{
		pauseRoutine = true;
		while (pauseRoutine)
			yield return null;
	}
	
	//--------------------------------------------------------------
	
	//Singleton variable
    private static Scenario s_instance = null;

    //GameManager singleton declaration
    public static Scenario instance
    {
        get
        {
            //Get instance in current scene
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(Scenario)) as Scenario;
            }

            return s_instance;
        }
    }
}
