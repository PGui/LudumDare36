using UnityEngine;
using System.Collections;

public class FxFade : MonoBehaviour {

    public float LifeDuration = 1.0f;
    public float FadeDuration = 1.0f;
    public bool DestroyAfterFade = true;

    public AnimationCurve ScaleAlongLife;
    public bool AnimScale = false;

    private float LifeTime;
    private SpriteRenderer Rend;
    private Vector3 BaseScale;

    public float tmp=0;

	// Use this for initialization
	void Start () {
        LifeTime = 0.0f;
        Rend = GetComponent<SpriteRenderer>();
        BaseScale = gameObject.transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {

        float dt = Time.deltaTime;
        LifeTime += dt;

        float CurAlpha = Mathf.Clamp01(1.0f - (LifeTime - LifeDuration + FadeDuration) / FadeDuration);
        Color CurColor = Rend.color;
        CurColor.a = CurAlpha;
        Rend.color = CurColor;

        if(AnimScale)
        {
            float CurScale = ScaleAlongLife.Evaluate(LifeTime / LifeDuration);
            tmp = CurScale;
            gameObject.transform.localScale = BaseScale * CurScale;
        }

        if(DestroyAfterFade && CurAlpha<=0.0f)
        {
            Destroy(gameObject);
        }

    }
}
