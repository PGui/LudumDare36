using UnityEngine;
using System.Collections;

public class FreezeFrameMgr : MonoBehaviour {

    // Use this for initialization

 

    void Awake () {


    }


    // Update is called once per frame
    void Update () {
	
	}

    public void FreezeFrame(float Duration, float TimeScale, float DelayBeforeFreeze = 0.0f)
    {
        StartCoroutine(FreezeFrameCoroutine(Duration, TimeScale, DelayBeforeFreeze));
    }

    IEnumerator FreezeFrameCoroutine(float Duration, float TimeScale, float DelayBeforeFreeze)
    {
        yield return new WaitForSeconds(DelayBeforeFreeze);

        float Elapsed = 0.0f;
        Time.timeScale = TimeScale;
        while(Elapsed < Duration)
        {
            Elapsed += Time.deltaTime;

            yield return null;
        }

        Time.timeScale = 1.0f;
    }
}
