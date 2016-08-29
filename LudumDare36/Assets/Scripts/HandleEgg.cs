using UnityEngine;
using System.Collections;



public class HandleEgg : MonoBehaviour {

    public EEggBonus Bonus;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Egg")
        {
            EEggBonus EggBonus = other.gameObject.GetComponent<EggBonus>().Bonus;
            gameObject.transform.parent.GetComponent<SnakeMovement>().SpawnBodyPart = true;
            gameObject.transform.parent.GetComponent<SnakeMovement>().SetBonus(EggBonus);
            GameObject.Destroy(other.gameObject);
        }
        else if (other.tag == "Ennemy")
        {
			if (other.gameObject.GetComponent<EnnemyBehavior>())
	            other.gameObject.GetComponent<EnnemyBehavior>().DestroyEnnemy(null);
        }
    }
}
