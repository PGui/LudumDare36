using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EBackground
{
    COUNTRY,
    SUBURB,
    URBAN,
    SUNSET,
    NIGHT,
}

public class BackMgr : MonoBehaviour {


    public List<GameObject> Backgrounds;
    
    public List<Color> GroundColors;
    public List<Color> TransitionColors;
    public List<Color> SkyColors;
    public List<Color> RetroColors;

    private Color GroundCurrent;
    private Color TransitionCurrent;
    private Color SkyCurrent;
    private Color RetroCurrent;

    private int RetroID;
    private int GlobalID;
    private int SkyID;

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

        GroundCurrent = UpdateColor(GroundColors, null, GroundCurrent, Force);
        TransitionCurrent = UpdateColor(TransitionColors, null, TransitionCurrent, Force);
        SkyCurrent = UpdateColor(SkyColors, null, SkyCurrent, Force);
        RetroCurrent = UpdateColor(RetroColors, null, RetroCurrent, Force);

        Shader.SetGlobalColor(GlobalID, GroundCurrent);
        Shader.SetGlobalColor(SkyID, SkyCurrent);
        Shader.SetGlobalColor(RetroID, RetroCurrent);

    }
    
    // Use this for initialization
    void Start () {
        RetroID = Shader.PropertyToID("RetroMul");
        GlobalID = Shader.PropertyToID("GlobalColor");
        SkyID = Shader.PropertyToID("SkyColor");
        UpdateBack(true);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateBack();
    }

    //--------------------------------------------------------------

    //Singleton variable
    private static BackMgr s_instance = null;

    //GameManager singleton declaration
    public static BackMgr instance
    {
        get
        {
            //Get instance in current scene
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(BackMgr)) as BackMgr;
            }

            return s_instance;
        }
    }
}
