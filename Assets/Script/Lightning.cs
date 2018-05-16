using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Lightning : MonoBehaviour {

    Animator anim;

    public LayerMask blockingLayer;
    private BoxCollider2D boxCollider;

    public int thunderDamage = 80;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit2D hit;
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(transform.position - new Vector3(0, 1), transform.position - new Vector3(0, 1), blockingLayer);
        boxCollider.enabled = true;
        if (CheckEndAnimation())
        {
            if (hit.transform == null) Destroy(this.gameObject);
            else if (hit.transform.tag == "Player")
            {
                Debug.Log("벼락!");
                Player player = hit.transform.GetComponent<Player>();
                player.PlayerHit(thunderDamage,this.gameObject);
                Destroy(this.gameObject);
            }
            
        }
	}

    bool CheckEndAnimation()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Lightning") &&

            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f;
        
    }
   
}
