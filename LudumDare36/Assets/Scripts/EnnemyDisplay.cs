using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnnemyDisplay : MonoBehaviour {

    public List<Color> BaseColors;
    public List<Sprite> BaseSprites;

    public int ColorIndex = 0;
    public bool RandomColor = true;
    
    public int SpriteIndex = 0;
    public bool RandomSprite = true;

    private GameObject BackLight;
    private GameObject HighLight;

    private float RandOff;

    private SpriteRenderer Rend;

    // Use this for initialization
    void Start () {
        Rend = GetComponent<SpriteRenderer>();

        RandOff = Random.Range(0.0f, 10.0f);

        Transform BackTr = transform.FindChild("BackLight");
        BackLight = BackTr ? BackTr.gameObject : null;
        Transform HighTr = transform.FindChild("HighLight");
        HighLight = HighTr ? HighTr.gameObject : null;

        if (RandomColor) ColorIndex = Random.Range(0, BaseColors.Count);
        if (RandomSprite) SpriteIndex = Random.Range(0, BaseSprites.Count);

        if (Rend)
        {
            if (SpriteIndex < BaseSprites.Count) Rend.sprite = BaseSprites[SpriteIndex];
            if (ColorIndex<BaseColors.Count) Rend.color = BaseColors[ColorIndex];
        }
        if(BackLight)
        {
            SpriteRenderer BackRend = BackLight.GetComponent<SpriteRenderer>();
            if (BackRend && ColorIndex < BaseColors.Count) BackRend.color = BaseColors[ColorIndex] * new Color(0.2f,0.2f,0.2f);
        }
    }
	
	// Update is called once per frame
	void Update () {

        float CurTime = Time.time;
        /*if(BackLight)
        {
            float Angle = CurTime*5.0f + RandOff*2.0f;
            float Dist = Mathf.Sin(Angle*0.2f) * 0.2f + 0.5f;
            BackLight.transform.localPosition = new Vector3(Mathf.Cos(Angle)*Dist, Mathf.Sin(Angle)*Dist, 0.0f);
        }*/
        if (HighLight)
        {
            float Angle = CurTime * 0.7f + RandOff;
            float Dist = Mathf.Sin(Angle * 0.2f) * 0.2f + 0.5f;
            HighLight.transform.localPosition = new Vector3(Mathf.Cos(Angle) * Dist, Mathf.Sin(Angle) * Dist + 0.4f, 0.0f);
        }

    }
}
