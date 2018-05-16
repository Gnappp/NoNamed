using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour {

    public GameObject lightning;
    bool first = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!first)
            StartCoroutine(Thunder());
	}

    IEnumerator Thunder()
    {
        first= true;
        yield return new WaitForSeconds(1f);
        GameObject thunder = lightning;
        GameObject temp = Instantiate(thunder, transform.position + new Vector3(0, 1), Quaternion.identity) as GameObject;
        //temp.transform.parent = this.transform;
        Destroy(this.gameObject);
    }
}
