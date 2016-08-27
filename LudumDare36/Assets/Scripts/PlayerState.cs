using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour {

    public enum EPlayerState
    {
        ALIVE,
        INVINCIBLE,
        DEAD
    }

    public float InitialLife = 100.0f;
    public bool InvicibilityFrame = false;
    public float InvicibilityFrameDuration = 0.5f;

    public int Score = 0;
    public GameObject UIScore;

    private Text ScoreText;

    public int KillEnnemyCount = 0;

    public EPlayerState CurrentState { get; set; }

    public GameObject UI;
    private Animator AnimatorUI;

	// Use this for initialization
	void Start () {
        CurrentState = EPlayerState.ALIVE;
        ScoreText = UIScore.GetComponent<Text>();
        ScoreText.text = Score.ToString();

        AnimatorUI = UI.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator CooldownInvincible()
    {
        yield return new WaitForSeconds(InvicibilityFrameDuration);
        CurrentState = EPlayerState.ALIVE;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "ennemyprojectile" && CurrentState == EPlayerState.ALIVE)
        {
            CurrentState = EPlayerState.INVINCIBLE;
            //RemoveLife
            StartCoroutine(CooldownInvincible());
        }
    }

    void AddScore(int ScoreToAdd)
    {
        Score += ScoreToAdd;
        ScoreText.text = Score.ToString();
        AnimatorUI.SetTrigger("AddScore");

        ++KillEnnemyCount;
        if(KillEnnemyCount == 10)
        {
            KillEnnemyCount = 0;
            AnimatorUI.SetTrigger("BigScore");
            Camera.main.GetComponent<ScreenshakeMgr>().StartShake(0.4f, 3.5f, 4.0f);
            Camera.main.GetComponent<FreezeFrameMgr>().FreezeFrame(0.5f, 0.2f, 0.2f);
        }
    }
}
