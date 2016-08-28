using UnityEngine;
using System.Collections;

public enum EEggBonus
{
    SPEED,
    SHIELD,
    DASH,
    COUNT
}

public class EggBonus : MonoBehaviour {

    public EEggBonus Bonus;

	// Use this for initialization
	void Start ()
    {
        Bonus = (EEggBonus)Random.Range((int)EEggBonus.SPEED, (int)EEggBonus.COUNT);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
