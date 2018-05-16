using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameInfo: MonoBehaviour {

    public static GameInfo instance = null;

    public GameObject star;
    private Transform starSetup;

    public string[] scenes;
    int scenesNo = 0;

    //PlayerInfo
    public int playerHp = 200;
    public Vector2 playerPosition=new Vector2(0,0);
    public int bossKill = 1;
    public int monsterKill;
    public int finalityGauge = 0;

    

    

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
     }

    // Use this for initialization
    void Start ()
    {
        ScenesManage();
        monsterKill = bossKill * 3;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (monsterKill == 0)
        {
            Debug.Log("AllKill");
            monsterKill = bossKill * 3;
            SetStar();
        }
    }

    public void ScenesManage()
    {
        
        SceneManager.LoadScene(scenes[scenesNo]);
        scenesNo++;        
    }

    public void SetStar()
    {
        GameObject temp = Instantiate(star, new Vector3(8.5f, 0.5f), Quaternion.identity) as GameObject;
        temp.transform.SetParent(starSetup);
    }

   
}
