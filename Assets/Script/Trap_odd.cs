using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_odd : MonoBehaviour {

    SpriteRenderer render;
    public Sprite trap;
    public Sprite tile;

    
    // Use this for initialization
    void Start()
    {
        render = (SpriteRenderer)gameObject.GetComponent<SpriteRenderer>();
        render.sprite = trap;
        StartCoroutine(Boom(render));
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    IEnumerator Boom(SpriteRenderer render)
    {
            yield return new WaitForSeconds(3f);
            render.sprite = tile;
    }
}
