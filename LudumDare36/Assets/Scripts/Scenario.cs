﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scenario : MonoBehaviour
{
	//--------------------------------------------------------------

	public bool debugRoutine = false;

	public bool skipTutorial = false;
	public bool immediateBoss = false;

	public GameObject startArea;
	private Vector3 posStartArea;
	
	private Vector3 posSnake;
	private Vector3 posPlayer;

	public Text textTitle;
	public Text textInput;
	public Image imageAvatar;
	public Text textAvatar;
	public Text textFinalScore;
	public Text textCredits;

	//--------------------------------------------------------------
	
	public void PlayScenario()
	{
		//print("Scenario - Start");

		StartCoroutine(PlayScenarioRoutine());
	}

	public void PlayNextStep()
	{
		pauseRoutine = false;

		//print("Scenario - NextStep");
	}
	
	//--------------------------------------------------------------

	private bool pauseRoutine;
	private bool waitSpacebar = false;
	private bool restart = false;
	
	void Start ()
	{
		posStartArea = startArea.transform.position;
		posSnake = SnakeMovement.instance.Head.transform.position;
		posPlayer = PlayerState.instance.transform.position;

		if (!debugRoutine)
		{
			PlayScenario();
		}
	}
	
	void Update ()
	{
		if (Input.GetKey("escape"))
            Application.Quit();

		if (restart)
		{
			PlayScenario();
		}
		else if (!debugRoutine)
		{
			if (waitSpacebar && Input.GetButtonDown("Fire1"))
			{
				print("first spacebar");
				waitSpacebar = false;
				PlayNextStep();
			}
		}
		else
		{
			//DEBUG
			if (Input.GetButtonDown("Fire1"))
			{
				PlayNextStep();
			}

			if (Input.GetButtonDown("Fire2"))
			{
				PlayScenario();
			}
			//DEBUG
		}
	}

	IEnumerator PlayScenarioRoutine()
	{
		pauseRoutine = false;
		waitSpacebar = false;
		
		BackMgr.instance.SetBack(EBackground.COUNTRY);
		startArea.transform.position = posStartArea;
		
		SnakeMovement.instance.SetSnakeStarted(false);
		SnakeMovement.instance.Head.transform.position = posSnake;
		SnakeMovement.instance.Restart();

		PlayerState.instance.transform.position = posPlayer;

		textTitle.CrossFadeAlpha(0.0f, 0.0f, false);
		textInput.CrossFadeAlpha(0.0f, 0.0f, false);
		imageAvatar.CrossFadeAlpha(0.0f, 0.0f, false);
		textAvatar.CrossFadeAlpha(0.0f, 0.0f, false);
		textFinalScore.CrossFadeAlpha(0.0f, 0.0f, false);
		textCredits.CrossFadeAlpha(0.0f, 0.0f, false);

		textInput.color = new Color(245,245,245);
		textAvatar.color = new Color(245,245,245);

        PlayerState.instance.AllowPlayerMove(false);
        PlayerState.instance.AllowPlayerShoot(false);
        PlayerState.instance.InitialLife = 100.0f;
        PlayerState.instance.CurrentState = PlayerState.EPlayerState.ALIVE;
        PlayerState.instance.FirstShoot = true;
        PlayerState.instance.FirstMove = true;

        PlayerState.instance.Score = 0;

        GameObject Intro3310 = GameObject.Find("3310_Intro");
        GameObject IntroLight3310 = GameObject.Find("3310_IntroLight");
        GameObject LightPhone = GameObject.Find("LightPhone");
        SpriteRenderer IntroRend = Intro3310 ? Intro3310.GetComponent<SpriteRenderer>() : null;
		SpriteRenderer IntroLightRend = IntroLight3310 ? IntroLight3310.GetComponent<SpriteRenderer>() : null;
		SpriteRenderer LightPhoneRend = LightPhone ? LightPhone.GetComponent<SpriteRenderer>() : null;
        SpriteRenderer PlayerRend = PlayerState.instance.GetComponent<SpriteRenderer>();
        if (IntroRend) IntroRend.enabled = true;
		if (IntroLightRend) IntroLightRend.enabled = true;
		if (LightPhoneRend) LightPhoneRend.enabled = false;
        if (PlayerRend) PlayerRend.enabled = false;

		SpawnerMgr.instance.BlockSpawning = false;

		if (restart)
		{
			skipTutorial = false;
			immediateBoss = false;

			restart = false;
			FadeMgr.instance.FadeIn(0.03f);
		}

        //////// Tutorial ////////
        if (skipTutorial)
		{
			PlayerState.instance.AllowPlayerMove(true);
			PlayerState.instance.AllowPlayerShoot(true);
			iTween.MoveTo(startArea, iTween.Hash("position", new Vector3(-100,-40,0), "time", 8.0f));
            GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenA, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenB, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.Wind, true);
			
			if (IntroRend) IntroRend.enabled = false;
			if (IntroLightRend) IntroLightRend.enabled = false;
			if (LightPhoneRend) LightPhoneRend.enabled = false;
            if (PlayerRend) PlayerRend.enabled = true;
		}
		else
		{
			//Show Title
			textTitle.CrossFadeAlpha(1.0f, 1.5f, false);
			textCredits.CrossFadeAlpha(1.0f, 2.0f, false);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.Wind, true);
			yield return new WaitForSeconds(3.0f);

			//Ring and hide title
			AudioSource ringTone = GameAudio.instance.PlaySFxLoop(ESFx.Ring3310);

            if (IntroRend) IntroRend.enabled = false;
			if (LightPhoneRend) LightPhoneRend.enabled = true;

			//Show input
			waitSpacebar = true;
			textInput.text = "Press Spacebar to answer the phone...";
			textInput.CrossFadeAlpha(1.0f, 2.0f, false);
			yield return PauseRoutine();

			//Answer phone
			textInput.CrossFadeAlpha(0.0f, 1.0f, false);
			GameAudio.instance.StopSFxLoop(ringTone);
			yield return new WaitForSeconds(1.0f);
		
			//Dialogue
			textCredits.CrossFadeAlpha(0.0f, 1.0f, false);
			textAvatar.text = "Finally, my people need me again. Time to settle who's the best phone in town.";
			textAvatar.CrossFadeAlpha(1.0f, 1.0f, false);
			imageAvatar.CrossFadeAlpha(1.0f, 1.0f, false);
			yield return new WaitForSeconds(4.0f);
		
			//Hide Dialogue
			textTitle.CrossFadeAlpha(0.0f, 2.5f, false);
			textAvatar.CrossFadeAlpha(0.0f, 1.0f, false);
			imageAvatar.CrossFadeAlpha(0.0f, 1.0f, false);
			yield return new WaitForSeconds(1.0f);

			//Show input
			PlayerState.instance.AllowPlayerMove(true);
			PlayerState.instance.FirstMove = true;
			textInput.text = "Use Z/Q/S/D or W/A/S/D to start moving...";
			textInput.CrossFadeAlpha(1.0f, 1.5f, false);
			yield return PauseRoutine();

            //Exit start area
			if (IntroLightRend) IntroLightRend.enabled = false;
			if (LightPhoneRend) LightPhoneRend.enabled = false;
            if (PlayerRend) PlayerRend.enabled = true;

            textInput.CrossFadeAlpha(0.0f, 1.0f, false);
			iTween.MoveTo(startArea, iTween.Hash("position", new Vector3(-100,-40,0), "time", 8.0f));
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenA, true);
			yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		
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
			yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		}
		
		//////// Wave 1 ////////
		
		//SpawnerMgr.instance.SpawnWave(Random.Range(20,30), Random.Range(5.0f, 15.0f), (EPattern)Random.Range((int)EPattern.SIN_RIGHT_TO_LEFT, (int)EPattern.COUNT));
		//SpawnerMgr.instance.SpawnEggs(2, 10.0f);
		//BackMgr.instance.SetBack(EBackground.URBAN);
		//SpawnerMgr.instance.SpawnEggs(2, 10.0f);
		
		if (immediateBoss)
		{
			GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenA, true);
			GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenB, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.FightA, true);

			SnakeMovement.instance.SetSnakeStarted(true);
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 5.0f, 10, 4.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(5.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonA, 5.0f, 10, 2.0f, EPattern.RANDOMPOINT, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(5.0f);
				
			//Snake call
			SnakeMovement.instance.SetSnakeStarted(true);
			SpawnerMgr.instance.SpawnEggs(20, 5.0f);
			textAvatar.text = "Mighty Snake, answer my call !";
			textAvatar.CrossFadeAlpha(1.0f, 1.0f, false);
			textAvatar.color = new Color(0,0,0);
			yield return new WaitForSeconds(4.0f);
			
			textAvatar.CrossFadeAlpha(0.0f, 1.0f, false);
			imageAvatar.CrossFadeAlpha(0.0f, 1.0f, false);
			BackMgr.instance.SetBack(EBackground.SUBURB);
			GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenA, true);
			GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenB, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.FightA, true);
			yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
			
			yield return new WaitForSeconds(0.5f);

			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelC, 5.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelD, 5.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(12.0f);

			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 5.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelB, 8.0f, 30, 8.0f, EPattern.COS_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(4.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonB, 5.0f, 30, 5.0f, EPattern.RANDOMPOINT, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(8.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 5.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.TOP);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelC, 5.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.BOTTOM);
			yield return new WaitForSeconds(12.0f);
			
			BackMgr.instance.SetBack(EBackground.URBAN);
			yield return new WaitForSeconds(5.0f);

			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 3.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelB, 8.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelC, 5.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.TOP);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelD, 6.0f, 30, 8.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.BOTTOM);
			yield return new WaitForSeconds(5.0f);

			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonC, 1.0f, 30, 10.0f, EPattern.RANDOMPOINT, ESpawnLocation.RANDOM);
			yield return new WaitForSeconds(3.0f);

			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelB, 6.0f, 100, 20.0f, EPattern.COS_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelD, 6.0f, 100, 20.0f, EPattern.COS_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(4.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 6.0f, 60, 16.0f, EPattern.COS_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.TOP);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelB, 6.0f, 60, 16.0f, EPattern.COS_RIGHT_TO_LEFT, ESpawnLocation.BOTTOM);
			yield return new WaitForSeconds(4.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonD, 2.0f, 30, 10.0f, EPattern.RANDOMPOINT, ESpawnLocation.RANDOM);
			yield return new WaitForSeconds(6.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelB, 6.0f, 100, 20.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelD, 6.0f, 100, 20.0f, EPattern.SIN_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(4.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 6.0f, 60, 16.0f, EPattern.SIN_RIGHT_TO_LEFT_REVERSED, ESpawnLocation.TOP);
			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelB, 6.0f, 60, 16.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.BOTTOM);
			yield return new WaitForSeconds(4.0f);

			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonA, 2.0f, 10, 10.0f, EPattern.RANDOMPOINT, ESpawnLocation.TOP);
			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonB, 2.0f, 10, 10.0f, EPattern.RANDOMPOINT, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonC, 2.0f, 10, 10.0f, EPattern.RANDOMPOINT, ESpawnLocation.CENTER);
			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonD, 2.0f, 10, 10.0f, EPattern.RANDOMPOINT, ESpawnLocation.BOTTOM);
			yield return new WaitForSeconds(10.0f);
			
			yield return new WaitForSeconds(12.0f);
		}
		
		//////// Boss 1 ////////
		
		BackMgr.instance.SetBack(EBackground.NIGHT);
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.FightA, true);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.BossA, true);
		yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		
		yield return new WaitForSeconds(0.5f);

		SpawnerMgr.instance.SpawnBoss(new Vector3(46,8,0));
		yield return PauseRoutine();
		
		//////// Ending ////////
		
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.BossA, true);
		yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		
		//yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		
		yield return new WaitForSeconds(3.0f);

		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.Final, true);
		yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		
		BackMgr.instance.SetBack(EBackground.SUNSET);
		yield return new WaitForSeconds(GameAudio.instance.beatDuration);
		
		//yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());

		CharacterMovement.instance.PlayerAnimator.SetInteger("Direction", 0);
		PlayerState.instance.AllowPlayerMove(false);
		PlayerState.instance.AllowPlayerShoot(false);
		iTween.MoveTo(PlayerState.instance.gameObject, iTween.Hash("position", new Vector3(-15,-8,0), "time", 2.0f));
		textAvatar.text = "My work here is done. I hope next generation will bring some challenge...";
		textAvatar.CrossFadeAlpha(1.0f, 1.0f, false);
		textAvatar.color = new Color(0,0,0);
		yield return new WaitForSeconds(GameAudio.instance.beatDuration);
		
		yield return new WaitForSeconds(GameAudio.instance.beatDuration);
		//yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		//yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		
		CharacterMovement.instance.PlayerAnimator.SetInteger("Direction", 2);
		iTween.MoveTo(PlayerState.instance.gameObject, iTween.Hash("position", new Vector3(50,50,0), "time", 24.0f));
		textAvatar.CrossFadeAlpha(0.0f, 1.0f, false);
		FadeMgr.instance.FadeOut(0.015f);
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.Wind, true);
		yield return new WaitForSeconds(4.0f);

		textFinalScore.CrossFadeAlpha(1.0f, 1.0f, false);
		textFinalScore.text = "Score : " + PlayerState.instance.Score;
		yield return new WaitForSeconds(6.0f);
		
		textFinalScore.CrossFadeAlpha(0.0f, 1.0f, false);
		yield return new WaitForSeconds(4.0f);

		restart = true;
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

