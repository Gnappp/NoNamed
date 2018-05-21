using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{

    public GameObject tutorialMonster;
    public Text tutorialText;


    // Use this for initialization
    void Start()
    {
        GameInfo.instance.finalityGauge = 1000;
        TutorialStart();
    }

    // Update is called once per frame
    void Update()
    {
       

    }

    void TutorialStart()
    {
        Time.timeScale = 0;
        tutorialText.text = "튜토리얼 입니다. 스킵하시려면 아래 스킵버튼을 눌러주세요\n 다음 x";
        WaitNext();
        AttackTutorial();
        BashTutorial();
        GaleTurorial();
        CounterTurorial();
        FinalityTurorial();
    }

    void FinalityTurorial()
    {
        Time.timeScale = 0;
        tutorialText.text = "빛나리는 r입니다. 100의 데미지입니다. 기본공격만 가능하며\n게이지를 1000을 채웠을때 사용가능합니다\n" +
            "빛나리 상태일때는 게이지가 올라가지 않습니다. 몬스터를 잡아보세요\n시작 x";

        Time.timeScale = 1;
        SummonMonster();
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = true; ;
        //MonsterCheck();
    }

    void CounterTurorial()
    {
        Time.timeScale = 0;
        tutorialText.text = "카운터은 c입니다. 70의 데미지와 몬스터의 공격을 막고 반격합니다. 몬스터를 잡아보세요\n시작 x";

        Time.timeScale = 1;
        SummonMonster();
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = true;
        GameInfo.instance.skillFinality = false;
        //MonsterCheck();
    }

    void GaleTurorial()
    {
        Time.timeScale = 0;
        tutorialText.text = "질풍참은 space입니다. 50의 데미지와 몬스터 뒤로 가거나 \n 몬스터가 뒤에 있는경우 뒤로 빠져나갑니다. 몬스터를 잡아보세요\n시작 x";

        Time.timeScale = 1;
        SummonMonster();
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = true;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = false;
        //MonsterCheck();
    }

    void BashTutorial()
    {
        Time.timeScale = 0;
        tutorialText.text = "배쉬는 z입니다. 100의 데미지와 몬스터를 끝까지 밀어 버립니다. 몬스터를 잡아보세요\n시작 x";

        Time.timeScale = 1;
        SummonMonster();
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = true;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = false;
        //MonsterCheck();
    }

    void AttackTutorial()
    {
        Time.timeScale = 0;
        tutorialText.text = "기본 공격은 x입니다. 20의 데미지와 빠른공격속도입니다. 몬스터를 잡아보세요\n시작 x";

        Time.timeScale = 1;
        SummonMonster();
        GameInfo.instance.skillAttack = true;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = false;
        //MonsterCheck();
    }

    void SummonMonster()
    {
        GameObject monster = tutorialMonster;
        GameObject temp = Instantiate(monster, new Vector3(4.5f, 4.5f), Quaternion.identity) as GameObject;
        temp.transform.parent = this.transform;

    }

    void MonsterCheck()
    {
        while (GameObject.Find("MonsterTutorial"))
        {
           
        }
    }

    void WaitNext()
    {
        while (Input.GetKeyUp(KeyCode.X))
        {
            
        }
    }
}
