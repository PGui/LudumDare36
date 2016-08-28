using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scenario : MonoBehaviour
{
	//--------------------------------------------------------------

	public GameObject startArea;

	public Text textTitle;
	public Text textInput;
	public Image imageAvatar;
	public Text textAvatar;

	//--------------------------------------------------------------
	
	public void PlayScenario()
	{
		print("Scenario - Start");

		step = 0;
		
		StartCoroutine(PlayScenarioRoutine());
	}

	public void PlayNextStep()
	{
		pauseRoutine = false;
		step += 1;

		print("Scenario - NextStep ("+ step +")");
	}
	
	//--------------------------------------------------------------

	private bool pauseRoutine;
	private int step;
	
	void Start ()
	{
	}
	
	void Update ()
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

	public void ResetScenario()
	{
		textTitle.CrossFadeAlpha(0.0f, 0.0f, false);
		textInput.CrossFadeAlpha(0.0f, 0.0f, false);
		imageAvatar.CrossFadeAlpha(0.0f, 0.0f, false);
		textAvatar.CrossFadeAlpha(0.0f, 0.0f, false);
	}

	IEnumerator PlayScenarioRoutine()
	{
		//////// Tutorial ////////

		//Show Title
		textTitle.CrossFadeAlpha(1.0f, 2.5f, false);
		yield return new WaitForSeconds(3.0f);

		//Ring and hide title
		textTitle.CrossFadeAlpha(0.0f, 2.5f, false);
		AudioSource ringTone = GameAudio.instance.PlaySFxLoop(ESFx.Ring3310);
		yield return new WaitForSeconds(4.0f);
		
		//Show input
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
		textInput.text = "Press Movement to start moving...";
		textInput.CrossFadeAlpha(1.0f, 1.0f, false);
		yield return PauseRoutine();

		//Exit start area
		textInput.CrossFadeAlpha(0.0f, 1.0f, false);
		iTween.MoveTo(startArea, iTween.Hash("position", new Vector3(-100,-100,0), "time", 8.0f));
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenA, false);
		yield return new WaitForSeconds(3.0f);
		
		//Show input
		textInput.text = "Press Attack to start attack...";
		textInput.CrossFadeAlpha(1.0f, 1.0f, false);
		yield return PauseRoutine();

		//Awaken
		textInput.CrossFadeAlpha(0.0f, 1.0f, false);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenB, true);
		yield return new WaitForSeconds(4.0f);
		
		//////// Wave 1 ////////

		//Fight wave
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenA, true);
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenB, true);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.FightA, true);
		yield return PauseRoutine();
		
		//////// Boss 1 ////////

		//Fight boss
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.FightA, true);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.BossA, true);
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
