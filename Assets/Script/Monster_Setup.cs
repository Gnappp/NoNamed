using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Setup : MonoBehaviour {

    public GameObject[] monsterStore;

    private int ver = 7;
    private int hor = 7;
    private List<Vector3> gridPositions = new List<Vector3>();



    // Use this for initialization
    void Start () {
        MonsterSetup();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void PositionSetup()
    {
        gridPositions.Clear();

        for (int x = 1; x < ver; x++)
        {
            for (int y = 1; y < hor; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    public void MonsterSetup()
    {
        PositionSetup();

        for (int i = 0; i < GameInfo.instance.bossKill*3; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject monster = monsterStore[Random.Range(0, monsterStore.Length)];
            GameObject temp = Instantiate(monster, randomPosition + new Vector3(0.5f, 0.5f), Quaternion.identity) as GameObject;
            temp.transform.parent=this.transform;
            Debug.Log(randomPosition);
        }
    }
}
