using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_odd : MonoBehaviour
{

    SpriteRenderer render;
    public Sprite trap;
    public Sprite tile;
    Vector2 playerPos;
    bool positionCheck;

    // Use this for initialization
    public void Start()
    {
        for (int i = 0; i < this.transform.GetChildCount(); i++)
        {
            render = this.transform.Find("Trap" + i).GetComponentInChildren<SpriteRenderer>();
            render.sprite = trap;
        }
        positionCheck = true;
        playerPos = new Vector2(0.5f,4.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPos != GameInfo.instance.playerPosition && positionCheck)
        {
            Debug.Log(playerPos + "   " + GameInfo.instance.playerPosition);
            for (int i = 0; i < this.transform.GetChildCount(); i++)
            {
                render = this.transform.Find("Trap" + i).GetComponentInChildren<SpriteRenderer>();
                render.sprite = tile;
            }
            positionCheck = false;
        }
    }

}
