using UnityEngine;
using System.Collections;

public class Scenario : MonoBehaviour
{
	//--------------------------------------------------------------

	public GameObject startArea;

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
			PlayScenario();
		}
	}

	IEnumerator PlayScenarioRoutine()
	{
		//Exit start area
		iTween.MoveTo(startArea, iTween.Hash("position", new Vector3(-100,-100,0), "time", 8.0f));
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenA, false);
		yield return PauseRoutine();
		
		//Awaken
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.AwakenB, true);
		yield return PauseRoutine();
		
		//Fight wave
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenA, true);
		GameAudio.instance.StopLayerOnBeatSync(EAudioLayer.AwakenB, true);
		GameAudio.instance.PlayLayerOnBeatSync(EAudioLayer.FightA, true);
		yield return PauseRoutine();
		
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
