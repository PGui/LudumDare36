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
                    //MoveHead
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
}
