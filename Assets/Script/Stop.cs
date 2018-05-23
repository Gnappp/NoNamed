using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : MonoBehaviour {

    bool start = false;
    bool end = false;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!start)
        {
            start = true;
            Time.timeScale = 0;
            StartCoroutine(WaitSecond());
        }
        if(end)
            Time.timeScale = 1;
    }

    IEnumerator WaitSecond()
    {
        
        yield return new WaitForSeconds(3f);
        end = true;
    }
}
