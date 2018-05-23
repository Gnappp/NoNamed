using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{

    public GameObject tutorialMonster;
    public Text tutorialText;

    bool nextTutorial = true;
    bool chatCheck = false;
    bool monsterSummon = false;

    public Image img;

    int chatNo = 0;

    // Use this for initialization
    void Start()
    {
        GameInfo.instance.finalityGauge = 1000;
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
        TutorialStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (nextTutorial)
        {
            TutorialChat();
            nextTutorial = false;
            chatCheck = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (chatCheck)
            {
                chatCheck = false;
                tutorialText.text = " ";
                img.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }

        if (monsterSummon)
        {
            GameObject temp = null;
            temp = GameObject.FindWithTag("Monster");
            if (temp != null) return;
            else if (temp == null)
            {
                nextTutorial = true;
                monsterSummon = false;
            }
        }

    }
    void TutorialChat()
    {
        switch (chatNo)
        {
            case 0:
                TutorialStart();
                chatNo++;
                break;
            case 1:
                AttackTutorial();
                chatNo++;
                break;
            case 2:
                CounterTurorial();
                chatNo++;
                break;
            case 3:
                GaleTurorial();
                chatNo++;
                break;
            case 4:
                BashTutorial();
                chatNo++;
                break;
            case 5:
                FinalityTurorial();
                chatNo++;
                break;
            case 6:
                TutorialEnd();
                chatNo++;
                GameInfo.instance.SetStar();
                break;
        }
    }

    void TutorialStart()
    {
        img.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "튜토리얼 입니다. 이동은 방향키이며 대각선 이동은 안됩니다.\n 다음 Ctrl";
        StartCoroutine(MoveTime());
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = false;
    }

    void AttackTutorial()
    {
        img.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "기본 공격은 x입니다. 20의 데미지와 빠른공격속도입니다. 몬스터를 잡아보세요\n시작 Ctrl";

        SummonMonster();
        GameInfo.instance.skillAttack = true;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = false;
    }

    void CounterTurorial()
    {
        img.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "카운터은 c입니다. 70의 데미지와 몬스터의 공격을 막고 반격합니다. 몬스터를 잡아보세요\n시작 Ctrl";

        SummonMonster();
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = true;
        GameInfo.instance.skillFinality = false;
    }

    void GaleTurorial()
    {
        img.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "질풍참은 space입니다. 50의 데미지와 몬스터 뒤로 가거나 \n 몬스터가 뒤에 있는경우 뒤로 빠져나갑니다. 몬스터를 잡아보세요\n시작 Ctrl";

        SummonMonster();
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = true;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = false;
    }

    void BashTutorial()
    {
        img.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "배쉬는 z입니다. 100의 데미지와 몬스터를 끝까지 밀어 버립니다. 몬스터를 잡아보세요\n시작 Ctrl";

        SummonMonster();
        GameInfo.instance.skillAttack = false;
        GameInfo.instance.skillBash = true;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = false;
    }

    void FinalityTurorial()
    {
        img.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "빛나리는 r입니다. 100의 데미지입니다. 기본공격만 가능하며\n게이지를 1000을 채웠을때 사용가능합니다\n" +
            "빛나리 상태일때는 게이지가 올라가지 않습니다. 몬스터를 잡아보세요\n시작 Ctrl";

        SummonMonster();
        GameInfo.instance.skillAttack = true;
        GameInfo.instance.skillBash = false;
        GameInfo.instance.skillGale = false;
        GameInfo.instance.skillCount = false;
        GameInfo.instance.skillFinality = true; 
    }

    void TutorialEnd()
    {
        img.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "튜토리얼이 종료되었습니다.\n별을 먹어 다음 스테이지로 가세요\n시작 Ctrl";

        GameInfo.instance.skillAttack = true;
        GameInfo.instance.skillBash = true;
        GameInfo.instance.skillGale = true;
        GameInfo.instance.skillCount = true;
        GameInfo.instance.skillFinality = true;
    }

    void SummonMonster()
    {
        GameObject monster = tutorialMonster;
        GameObject temp = Instantiate(monster, new Vector3(4.5f, 4.5f), Quaternion.identity) as GameObject;
        temp.transform.parent = this.transform;
        monsterSummon = true;
        GameInfo.instance.monsterKill++;
    }

    IEnumerator MoveTime()
    {
        yield return new WaitForSeconds(5f);
        nextTutorial = true;
    }


}
