using UnityEngine;
using System.Collections;

public class ScreenshakeMgr : MonoBehaviour {

    private float Speed = 5.0f;
    private Vector3 originalCamPos;
    private bool Shaking = false;

    // Use this for initialization
    void Start () {
            originalCamPos =this.transform.position;
    }
	

    public void StartShake(float Duration, float Magnitude, float Speed)
    {
        StartCoroutine(Shake(Duration, Magnitude, Speed));
    }


    IEnumerator Shake(float Duration, float Magnitude, float Speed)
    {
        float Elapsed = 0.0f;
        float RandomStart = Random.Range(-1000.0f, 1000.0f);

        while (Elapsed < Duration)
        {
            Elapsed += Time.deltaTime;

            float percentComplete = Elapsed / Duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            float Alpha = RandomStart + Speed * percentComplete;
            // map value to [-1, 1]
            float X = Mathf.PerlinNoise(0.0f, Alpha) * 2.0f - 1.0f ;
            float Y = Mathf.PerlinNoise(Alpha, 0.0f) * 2.0f - 1.0f;
            X *= Magnitude * damper;
            Y *= Magnitude * damper;

            this.transform.position = new Vector3(X, Y, originalCamPos.z);
            
            yield return null;
        }
        Debug.Log("Cocuocu");
        this.transform.position = originalCamPos;
    }
}
