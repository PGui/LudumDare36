using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour {

    public float TimeBetweenTwoMove = 0.5f;
    bool CanMove = true;

    public Transform Head;
    public GameObject BodyToSpawn;
    private Vector3 Target;
    float SizeSnakePart = 0.0f;

    public bool SpawnBodyPart { get; set; }

    private bool ShieldBonusOn = false;

    public float DurationSpeedBonus = 8.0f;

    public bool TargetChanged = false;

    // Use this for initialization
    void Start ()
    {
        SpawnBodyPart = false;
        SizeSnakePart = Head.GetComponent<SpriteRenderer>().bounds.size.x;
        Transform ClosestEgg = GetClosestEgg(GameObject.FindGameObjectsWithTag("Egg"));
        if (ClosestEgg)
        {
            Target = ClosestEgg.position;
        }
        else
        {
            Target = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 10));
        }

    }
	
	// Update is called once per frame
	void Update () {
	    if(CanMove)
        {
            TargetChanged = false;
            CanMove = false;
            StartCoroutine(Move());
            int offsetX = (int)((Target.x - Head.transform.position.x) / SizeSnakePart);
            int offsetY = (int)((Target.y - Head.transform.position.y) / SizeSnakePart);

            //Debug.Log(offsetX + "," + offsetY);
            if (offsetX == 0 && offsetY == 0)
            {
                Transform ClosestEgg = GetClosestEgg(GameObject.FindGameObjectsWithTag("Egg"));
                if (ClosestEgg)
                {
                    Target = ClosestEgg.position;
                }
                else
                {
                    Target = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 10));
                }
                TargetChanged = true;
                return;
            }

            bool CanMoveX = Mathf.Abs(offsetX) > 0;
            bool CanMoveY = Mathf.Abs(offsetY) > 0;
            List<Transform> BodyParts = GetSnakeParts();

            if(SpawnBodyPart)
            {
                SpawnBodyPart = false;
                GameObject newBody = GameObject.Instantiate(BodyToSpawn, BodyParts[BodyParts.Count - 1].position, Quaternion.identity) as GameObject;
                newBody.transform.parent = this.transform;
            }

            if (CanMoveX && CanMoveY)
            {
                if((Random.Range(0, 2) == 1))
                {
                    for(int i = BodyParts.Count - 1; i >= 1; --i)
                    {
                        BodyParts[i].transform.position = BodyParts[i - 1].transform.position;
                    }

                    Head.transform.position = new Vector3(Head.transform.position.x + Mathf.Sign(offsetX) * SizeSnakePart, Head.transform.position.y, Head.transform.position.z);

                }
                else
                {
                    for (int i = BodyParts.Count - 1; i >= 1; --i)
                    {
                        BodyParts[i].transform.position = BodyParts[i - 1].transform.position;
                    }

                    //MoveHead
                    Head.transform.position = new Vector3(Head.transform.position.x, Head.transform.position.y + Mathf.Sign(offsetY) * SizeSnakePart, Head.transform.position.z);
                }
            }
            else if(CanMoveX && !CanMoveY)
            {
                
                for (int i = BodyParts.Count - 1; i >= 1; --i)
                {
                    BodyParts[i].transform.position = BodyParts[i - 1].transform.position;
                }
                Head.transform.position = new Vector3(Head.transform.position.x + Mathf.Sign(offsetX) * SizeSnakePart, Head.transform.position.y, Head.transform.position.z);
            }
            else if (!CanMoveX && CanMoveY)
            {
                
                for (int i = BodyParts.Count - 1; i >= 1; --i)
                {
                    BodyParts[i].transform.position = BodyParts[i - 1].transform.position;
                }
                Head.transform.position = new Vector3(Head.transform.position.x, Head.transform.position.y + Mathf.Sign(offsetY) * SizeSnakePart, Head.transform.position.z);
            }
        }

	}

    List<Transform> GetSnakeParts()
    {
        List<Transform> Children = new List<Transform>();

        foreach (Transform child in transform)
        {
            //child is your child transform
            Children.Add(child);
        }


        return Children;
    }

    IEnumerator Move()
    {

        yield return new WaitForSeconds(TimeBetweenTwoMove);
        CanMove = true;
    }

    
    
    Transform GetClosestEgg(GameObject[] Eggs)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in Eggs)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        return bestTarget;
    }

    public void SetBonus(EEggBonus Bonus)
    {
        switch(Bonus)
        {
            case EEggBonus.DASH:
                break;
            case EEggBonus.SHIELD:
                StartCoroutine(ShieldBonus());
                break;
            case EEggBonus.SPEED:
                StartCoroutine(SpeedBonus());
                break;
            default:
                StartCoroutine(SpeedBonus());
                break;
        }
    }

    IEnumerator SpeedBonus()
    {
        TimeBetweenTwoMove /= 2.0f;
        yield return new WaitForSeconds(DurationSpeedBonus);
        TimeBetweenTwoMove *= 2.0f;
    }

    IEnumerator ShieldBonus()
    {
        //ShieldBonusOn = true;
        //Vector3 CurrentHeadViewport = Camera.main.WorldToViewportPoint(Head.transform.position);
        //Target = Camera.main.ViewportToWorldPoint(new Vector3(CurrentHeadViewport.x, 0.95f, 10));
        //Debug.DrawLine(Target, Target + Target * 0.5f,Color.magenta, 5.0f);
        //Debug.Log(Target + "," + Head.transform.position);


        //while(!TargetChanged)
        //{
        //    yield return null;
        //}

        //CurrentHeadViewport = Camera.main.WorldToViewportPoint(Head.transform.position);
        //Target = Camera.main.ViewportToWorldPoint(new Vector3(CurrentHeadViewport.x + 2.5f, 0.95f, 10));
        //Debug.Log(Target + "," + Head.transform.position +"dhdilfdui");
        //Debug.DrawLine(Target, Target + Target * 0.5f, Color.green, 5.0f);
        //while (!TargetChanged)
        //{
        //    yield return null;
        //}

        //CurrentHeadViewport = Camera.main.WorldToViewportPoint(Head.transform.position);
        //Target = Camera.main.ViewportToWorldPoint(new Vector3(CurrentHeadViewport.x, 0.1f, 10));
        //Debug.Log(Target + "," + Head.transform.position + "qsddfdf");
        //Debug.DrawLine(Target, Target + Target * 0.5f, Color.yellow, 5.0f);
        //while (!TargetChanged)
        //{
        //    yield return null;
        //}

        yield return null;

    }
}


