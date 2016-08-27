using UnityEngine;
using System.Collections;

public class BackScrollSprite : MonoBehaviour {

    public bool IsGround = true;

    public float HeightOffset = 0;
    
    private Renderer rend;

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {

        float speed = 0.3f;
        float horiz = 3.0f;
        float Paralax = 0.5f;

        float dt = Time.deltaTime;
        
        Vector3 pos = transform.position;

        float relHeight = Mathf.Abs((pos.y - horiz - HeightOffset)*Paralax);
        float curspeed = speed * relHeight * relHeight;

        pos.x -= dt * curspeed;
        pos.z = (pos.y - HeightOffset)*0.01f;
        transform.position = pos;

        float maxX = pos.x + rend.bounds.extents.x;
        if(maxX<-40)
        {
            Destroy(gameObject);
        }

    }
}
