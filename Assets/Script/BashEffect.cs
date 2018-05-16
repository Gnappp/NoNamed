using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(EffectDestory());
    }

    IEnumerator EffectDestory()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
