using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scenario : MonoBehaviour
{
	//--------------------------------------------------------------

	public bool debugRoutine = false;

	public bool skipTutorial = false;
	public bool immediateBoss = false;

	public GameObject startArea;

	public Text textTitle;
	public Text textInput;
	public Image imageAvatar;
	public Text textAvatar;

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
		if (!debugRoutine)
		{
			ResetScenario();
			PlayScenario();
		}
	}
	
	void Update ()
	{
		if (restart)
		{
			restart = false;
			ResetScenario();
			PlayScenario();
		}
		else if (!debugRoutine)
		{
			if (waitSpacebar && Input.GetButtonDown("Fire1"))
			{
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
				ResetScenario();
				PlayScenario();
			}
			//DEBUG
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
		textAvatar.color = new Color(245,245,245);

        PlayerState.instance.AllowPlayerMove(false);
        PlayerState.instance.AllowPlayerShoot(false);
        PlayerState.instance.InitialLife = 100.0f;
        PlayerState.instance.CurrentState = PlayerState.EPlayerState.ALIVE;
        PlayerState.instance.FirstShoot = true;
        PlayerState.instance.FirstMove = true;
    }

	IEnumerator PlayScenarioRoutine()
	{
        GameObject LightPhone = GameObject.Find("LightPhone");
        GameObject Intro3310 = GameObject.Find("3310_Intro");
        SpriteRenderer IntroRend = Intro3310 ? Intro3310.GetComponent<SpriteRenderer>() : null;
        if (LightPhone) LightPhone.SetActive(false);
        if (Intro3310) Intro3310.SetActive(true);
        if (IntroRend) IntroRend.enabled = true;
        SpriteRenderer PlayerRend = PlayerState.instance.GetComponent<SpriteRenderer>();
        if (PlayerRend) PlayerRend.enabled = false;

		SpawnerMgr.instance.BlockSpawning = false;

        //////// Tutorial ////////
        if (skipTutorial)
		{
			PlayerState.instance.AllowPlayerMove(true);
			PlayerState.instance.AllowPlayerShoot(true);
			iTween.MoveTo(startArea, iTween.Hash("position", new Vector3(-100,-40,0), "time", 8.0f));
            GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenA, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenB, true);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.Wind, true);
			
            if (Intro3310) Intro3310.SetActive(false);
            if (PlayerRend) PlayerRend.enabled = true;
            if (LightPhone) LightPhone.SetActive(false);

		}
		else
		{
			//Show Title
			textTitle.CrossFadeAlpha(1.0f, 1.5f, false);
			GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.Wind, true);
			yield return new WaitForSeconds(3.0f);

			//Ring and hide title
			AudioSource ringTone = GameAudio.instance.PlaySFxLoop(ESFx.Ring3310);

            if (LightPhone) LightPhone.SetActive(true);
            if (IntroRend) IntroRend.enabled = false;

            //yield return new WaitForSeconds(4.0f);
		
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
            if (Intro3310) Intro3310.SetActive(false);
            if (PlayerRend) PlayerRend.enabled = true;
            if (LightPhone) LightPhone.SetActive(false);

            textInput.CrossFadeAlpha(0.0f, 1.0f, false);
			//ScreenshakeMgr.instance.StartShake(1.0f, 3.0f, 10.0f);
			//ScreenshakeMgr.instance.StartShake(0.3f, 0.8f, 2.0f);
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
		
		yield return new WaitForSeconds(0.5f);

		if(!immediateBoss)
		{

			SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 5.0f, 10, 4.0f, EPattern.SIN_RIGHT_TO_LEFT, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(5.0f);
			
			SpawnerMgr.instance.SpawnWave(EEnemyType.BonbonA, 5.0f, 10, 2.0f, EPattern.RANDOMPOINT, ESpawnLocation.CENTER);
			yield return new WaitForSeconds(5.0f);
				
			//Snake call
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
			//SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 20, 3.0f, EPattern.RANDOMPOINT, ESpawnLocation.RANDOM);
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
		}
		
		//////// Boss 1 ////////
		/// 
		///

		// Clear Enemies
		EnnemyBehavior[] Enemies = GameObject.FindObjectsOfType<EnnemyBehavior>();
		int EnemyCount = Enemies.GetLength(0);
		for(int i=0; i<EnemyCount; ++i) {
			EnnemyBehavior CurEnemy = Enemies[i];
			if(CurEnemy) {
				CurEnemy.DestroyEnnemy(null, false);
			}
		}
		SpawnerMgr.instance.BlockSpawning = true;
		
		BackMgr.instance.SetBack(EBackground.NIGHT);
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.FightA, true);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.BossA, true);
		yield return new WaitForSeconds(GameAudio.instance.GetTimeUntilBeat());
		
		yield return new WaitForSeconds(0.5f);

		SpawnerMgr.instance.BlockSpawning = false;

		//Fight boss

		PlayerState.instance.FirstShoot = false;
		PlayerState.instance.FirstMove = false;
		waitSpacebar = false;
		SpawnerMgr.instance.SpawnBoss(new Vector3(46,8,0));

		yield return PauseRoutine();

		immediateBoss = false;

		//SpawnerMgr.instance.SpawnWave(EEnemyType.JewelA, 20, 3.0f, EPattern.RANDOMPOINT, ESpawnLocation.RANDOM);
		//GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.FightA, true);
		//GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.BossA, true);
		//yield return PauseRoutine();
		
		//////// Ending ////////
		
		BackMgr.instance.SetBack(EBackground.SUNSET);

		yield return new WaitForSeconds(15.0f);

		BackMgr.instance.SetBack(EBackground.COUNTRY);

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
