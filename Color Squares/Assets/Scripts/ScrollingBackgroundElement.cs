using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackgroundElement : MonoBehaviour {

    public float speed;
    public Transform endPoint;
    public Transform startPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector2(Time.deltaTime * speed, -Time.deltaTime * speed));
        if(transform.position.x> endPoint.position.x)
        {
            transform.position = startPoint.position;
        }
	}
}
