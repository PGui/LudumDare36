using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackMgr : MonoBehaviour {

    public enum EBackground
    {
        COUNTRY,
        SUBURB,
        URBAN,
    }

    public List<GameObject> Backgrounds;

    public GameObject Ground;
    public GameObject Transition;
    public GameObject Sky;

    public List<Color> GroundColors;
    public List<Color> TransitionColors;
    public List<Color> SkyColors;
    public List<Color> RetroColors;

    private Color GroundCurrent;
    private Color TransitionCurrent;
    private Color SkyCurrent;
    private Color RetroCurrent;

    private int RetroID;

    private EBackground CurrentBack = EBackground.COUNTRY;
    public EBackground NextBack = EBackground.COUNTRY;

    public void SetBack(EBackground NewBack)
    {
        NextBack = NewBack;
    }

    private Color GetCurrentColor(List<Color> List)
    {
        int Cur = (int)CurrentBack;
        if (Cur<List.Count)
        {
            return List[Cur];
        }
        return Color.white;
    }

    private Color UpdateColor(List<Color> List, GameObject Obj, Color Value, bool Force)
    {
        Color NewColor = GetCurrentColor(List);
        if (Force)
        {
            Value = NewColor;
        }
        else
        {
            Value = Color.Lerp(Value, NewColor, 0.03f);
        }
        if (Obj)
        {
            SpriteRenderer Rend = Obj.GetComponent<SpriteRenderer>();
            if (Rend)
            {
                Rend.color = Value;
            }
        }
        return Value;
    }

    private void UpdateBack(bool Force = false)
    {
        if(Force || NextBack != CurrentBack)
        {
            CurrentBack = NextBack;
            for (int i = 0; i < Backgrounds.Count; ++i)
            {
                bool IsActive = i == (int)CurrentBack;
                GameObject CurBack = Backgrounds[i];
                if(CurBack)
                {
                    BackSpawner[] ChildList = CurBack.GetComponentsInChildren<BackSpawner>();
                    int ChildCount = ChildList.GetLength(0);
                    for (int j=0; j< ChildCount; ++j)
                    {
                        BackSpawner CurChild = ChildList[j];
                        if(CurChild)
                        {
                            CurChild.Enable = IsActive;
                            if (Force)
                            {
                                CurChild.InitialSpawn();
                            }
                        }
                    }
                }
            }
        }

        GroundCurrent = UpdateColor(GroundColors, Ground, GroundCurrent, Force);
        TransitionCurrent = UpdateColor(TransitionColors, Transition, TransitionCurrent, Force);
        SkyCurrent = UpdateColor(SkyColors, Sky, SkyCurrent, Force);
        RetroCurrent = UpdateColor(RetroColors, null, RetroCurrent, Force);
        Shader.SetGlobalColor(RetroID, RetroCurrent);

    }
    
    // Use this for initialization
    void Start () {
        RetroID = Shader.PropertyToID("RetroMul");
        UpdateBack(true);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateBack();
    }
}
