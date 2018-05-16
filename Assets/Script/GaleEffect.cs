using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaleEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        StartCoroutine(EffectDestory());
	}

    IEnumerator EffectDestory()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
