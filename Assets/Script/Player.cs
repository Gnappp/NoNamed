using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private int playerHp = 200;
    private int playerAttackDamage = 20;
    private int playerGaleAttack = 50;
    private int playerCounterAttack = 70;
    private int playerBashAttack = 100;
    public int finalityGauge;
    public int finalityCount;

    public float hyperTimeSecond = 1f;
    bool playerTurn = true;
    bool trapIgnore = false;
    bool hyperTime = false;
    bool finalityMode = false;

    private Vector3 startPosition;
    private Vector3 beforePosition;
    public GameObject galeEffect;
    public GameObject bashEffect;
    public GameObject finalityButton;

    public Text hpText;
    public Text finalityText;

    private Animator animator;
    private Animator finalityAnimator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rd2D;
    private float inverseMoveTime;
    Quaternion derection = Quaternion.Euler(0, 0, 0);
    Quaternion front = Quaternion.Euler(0, 0, 0);
    Quaternion back = Quaternion.Euler(0, 0, 180);
    Quaternion left = Quaternion.Euler(0, 0, 90);
    Quaternion right = Quaternion.Euler(0, 0, -90);

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        GameObject finality = finalityButton;
        GameObject temp = Instantiate(finality, new Vector3(4.5f,-0.5f), front) as GameObject;
        finalityAnimator = temp.GetComponent<Animator>();

        playerHp = GameInfo.instance.playerHp;
        hpText.text = "HP : " + playerHp;

        GameInfo.instance.playerPosition = gameObject.transform.position;

        finalityGauge = GameInfo.instance.finalityGauge;
        finalityText.text = "Finality : " + finalityGauge;
        //finalityGauge = 1000;
        startPosition = gameObject.transform.position;
        boxCollider = GetComponent<BoxCollider2D>();
        rd2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        inverseMoveTime = 1f / moveTime;
    }

    private void OnDisable()
    {
        GameInfo.instance.playerHp = playerHp;
        GameInfo.instance.finalityGauge = finalityGauge;
    }

    // Update is called once per frame
    void Update()
    {
        if(finalityGauge>=1000 && !finalityMode)
        {
            finalityGauge = 1000;
            finalityText.text = "Finality : " + finalityGauge;
            Debug.Log("FF");
            finalityAnimator.SetTrigger("Active");
        }
        
        else if (finalityMode&&finalityCount==0)
        {
            finalityMode = false;
            animator.SetTrigger("EndFinality");
        }
        if(finalityMode)
            finalityAnimator.SetTrigger("Disable");
        if (!playerTurn || hyperTime) return;

        RaycastHit2D hit = new RaycastHit2D();
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (OverlapPlayer())
        {
            transform.position = beforePosition;
            return;
        } //겹치기

        if (horizontal != 0)
        {
            vertical = 0;
        } //대각선 방지

        if (horizontal != 0 || vertical != 0)
        {
            PlayerRotation(horizontal, vertical);
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(horizontal, vertical);
            boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, blockingLayer);
            boxCollider.enabled = true;
            if (hit.transform == null)
            {
                beforePosition = start;
                Move(end);
                playerTurn = false;
            }
            else if (hit.transform.tag == "Trap")
            {
                if (!trapIgnore)
                {
                    playerHp -= 50;
                    hpText.text = "HP : " + playerHp;
                    StartCoroutine(TrapIgnore());
                    rd2D.MovePosition(startPosition);
                }

            }

        } //움직임 및 트랩

        if (Input.GetKeyUp(KeyCode.X) && !hyperTime) 
        {
            if (GameInfo.instance.skillAttack)
            {
                if (!finalityMode)
                    animator.SetTrigger("Attack");
                else if (finalityMode)
                    animator.SetTrigger("FinalityAttack");
                DamageMonster(out hit);
                if (hit.transform == null)
                    return;
                if (hit.transform.tag == "Monster")
                {
                    Debug.Log("Hit");
                    Monster mob = hit.transform.GetComponent<Monster>();
                    mob.MonsterHit(playerAttackDamage);
                    if (!finalityMode)
                    {
                        finalityGauge += playerAttackDamage;
                        finalityText.text = "Finality : " + finalityGauge;
                    }
                    else if (finalityMode)
                    {
                        mob.MonsterHit(playerAttackDamage);
                        finalityCount--;
                    }
                }
                else if (hit.transform.tag == "Boss")
                {
                    Debug.Log("Hit");
                    Boss mob = hit.transform.GetComponent<Boss>();
                    mob.BossHit(playerAttackDamage);
                    if (!finalityMode)
                    {
                        finalityGauge += playerAttackDamage;
                        finalityText.text = "Finality : " + finalityGauge;
                    }
                    else if (finalityMode)
                    {
                        mob.BossHit(playerAttackDamage);
                        finalityCount--;
                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            if (GameInfo.instance.skillCount)
            {
                if (!hyperTime)
                    StartCoroutine(HyperTime());
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if(GameInfo.instance.skillGale)
                GaleAttack();
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            if(GameInfo.instance.skillBash)
                 BashAttack();
        }
        else if(finalityGauge>=1000&&Input.GetKeyUp(KeyCode.R))
        {
            if (GameInfo.instance.skillFinality)
            {
                finalityMode = true;
                finalityAnimator.SetTrigger("Disable");
                Debug.Log("cC");
                finalityGauge = 0;
                finalityText.text = "Finality : " + finalityGauge;
                animator.SetTrigger("FinalityMode");
                finalityCount = 7;
            }
        }
    }

    void GaleAttack()
    {
        RaycastHit2D hit;
        Vector3 start = transform.position;
        DamageMonster(out hit);

        if (hit.transform == null) return;
        if (hit.transform.tag == "Monster")
        {
            Gale(start, 2, hit);
            Monster mob = hit.transform.GetComponent<Monster>();
            mob.MonsterHit(playerGaleAttack);
            if (!finalityMode)
            {
                finalityGauge += playerGaleAttack;
                finalityText.text = "Finality : " + finalityGauge;
            }
        }

        else if (hit.transform.tag == "Boss")
        {
            Gale(start, 3, hit);
            Boss mob = hit.transform.GetComponent<Boss>();
            mob.BossHit(playerGaleAttack);
            if (!finalityMode)
            {
                finalityGauge += playerGaleAttack;
                finalityText.text = "Finality : " + finalityGauge;
            }
        }

    }

    void WallIdentify(Vector3 start, Vector3 end, out bool galePossible)
    {

        RaycastHit2D hit;
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        if (hit.transform.tag == "Wall")
        {
            galePossible = false;
        }
        galePossible = true;
    }

    void Gale(Vector3 start, int end, RaycastHit2D hit)
    {
        bool galePossible = true;
        if (derection == front)
        {
            WallIdentify(start, start + new Vector3(0, end), out galePossible);
            if (galePossible)
            {
                transform.position += new Vector3(0, end);
                transform.rotation = back;
                derection = back;
                GameObject gale = galeEffect;
                GameObject temp = Instantiate(gale, start + new Vector3(0, 1), transform.rotation) as GameObject;
                if (hit.transform.tag == "Boss")
                {
                    GameObject temp2 = Instantiate(gale, start + new Vector3(0, 2), transform.rotation) as GameObject;
                }
            }
        }
        else if (derection == back)
        {
            WallIdentify(start, start + new Vector3(0, -end), out galePossible);
            if (galePossible)
            {
                transform.position += new Vector3(0, -end);
                transform.rotation = front;
                derection = front;
                GameObject gale = galeEffect;
                GameObject temp = Instantiate(gale, start + new Vector3(0, -1), transform.rotation) as GameObject;
                if (hit.transform.tag == "Boss")
                {
                    GameObject temp2 = Instantiate(gale, start + new Vector3(0, -2), transform.rotation) as GameObject;
                }
            }
        }
        else if (derection == right)
        {
            WallIdentify(start, start + new Vector3(end, 0), out galePossible);
            if (galePossible)
            {
                transform.position += new Vector3(end, 0);
                transform.rotation = left;
                derection = left;
                GameObject gale = galeEffect;
                GameObject temp = Instantiate(gale, start + new Vector3(1, 0), transform.rotation) as GameObject;
                if (hit.transform.tag == "Boss")
                {
                    GameObject temp2 = Instantiate(gale, start + new Vector3(2, 0), transform.rotation) as GameObject;
                }
            }
        }
        else if (derection == left)
        {
            WallIdentify(start, start + new Vector3(-end, 0), out galePossible);
            if (galePossible)
            {
                transform.position += new Vector3(-end, 0);
                transform.rotation = right;
                derection = right;
                GameObject gale = galeEffect;
                GameObject temp = Instantiate(gale, start + new Vector3(-1, 0), transform.rotation) as GameObject;
                if (hit.transform.tag == "Boss")
                {
                    GameObject temp2 = Instantiate(gale, start + new Vector3(-2, 0), transform.rotation) as GameObject;
                }
            }
        }

    }

    void BashAttack()
    {
        RaycastHit2D hit1 = new RaycastHit2D();
        

        Vector3 start = transform.position;
        if (derection == front)
        {
            MonsterIdentify(start, start + new Vector3(0, 1), out hit1);
            if (hit1.transform.tag == "Monster")
                Bash(ref hit1, start + new Vector3(0, 1), new Vector3(0, 1));
        }
        else if (derection == back)
        {
            MonsterIdentify(start, start + new Vector3(0, -1), out hit1);
            if (hit1.transform.tag == "Monster")
                Bash(ref hit1, start + new Vector3(0, -1), new Vector3(0, -1));
        }
        else if (derection == right)
        {
            MonsterIdentify(start, start + new Vector3(1, 0), out hit1);
            if (hit1.transform.tag == "Monster")
                Bash(ref hit1, start + new Vector3(1, 0), new Vector3(1, 0));
        }
        else if (derection == left)
        {
            MonsterIdentify(start, start + new Vector3(-1, 0), out hit1);
            if (hit1.transform.tag == "Monster")
                Bash(ref hit1, start + new Vector3(-1, 0), new Vector3(-1, 0));
        }

    }

    void Bash(ref RaycastHit2D hit, Vector3 start, Vector3 plus)
    {
        RaycastHit2D checkBlock = new RaycastHit2D();
        Monster mob = hit.transform.GetComponent<Monster>();
        BoxCollider2D hitBoxCollider = mob.gameObject.GetComponent<BoxCollider2D>();
        hitBoxCollider.enabled = false;
        checkBlock = Physics2D.Linecast(start, start + plus, blockingLayer);
        hitBoxCollider.enabled = true;
        if (checkBlock.transform == null)
        {
            mob.transform.position = start + plus;
            start = start + plus;
            while (true)
            {
                hitBoxCollider.enabled = false;
                checkBlock = Physics2D.Linecast(start, start + plus, blockingLayer);
                hitBoxCollider.enabled = true;
                if (checkBlock.transform == null)
                {
                    mob.transform.position = start+plus;
                    GameObject bash = bashEffect;
                    GameObject temp = Instantiate(bash, start, transform.rotation) as GameObject;
                }
                else if (checkBlock.transform != null)
                    break;
                start = start + plus;
            }
        }
        mob.MonsterHit(playerBashAttack);
        if (!finalityMode)
        {
            finalityGauge += playerBashAttack;
            finalityText.text = "Finality : " + finalityGauge;
        }

    }




    void DamageMonster(out RaycastHit2D hit)
    {
        Vector3 start = transform.position;
        if (derection == front)
        {
            MonsterIdentify(start, start + new Vector3(0, 1), out hit);
            return;
        }
        else if (derection == back)
        {
            MonsterIdentify(start, start + new Vector3(0, -1), out hit);
            return;
        }
        else if (derection == right)
        {
            MonsterIdentify(start, start + new Vector3(1, 0), out hit);
            return;
        }
        else if (derection == left)
        {
            MonsterIdentify(start, start + new Vector3(-1, 0), out hit);
            return;
        }

        hit = new RaycastHit2D();
    }

    void MonsterIdentify(Vector3 start, Vector3 end, out RaycastHit2D mobTag)
    {
        boxCollider.enabled = false;
        mobTag = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

    }

    void PlayerRotation(int hori, int veri)
    {

        if (hori == 0)
            derection = veri > 0 ? front : back;
        else if (veri == 0)
            derection = hori > 0 ? right : left;
        gameObject.transform.rotation = derection;
    }

    void Move(Vector3 end)
    {
        GameInfo.instance.playerPosition = end;
        StartCoroutine(SmoothMove(end));
    }

    IEnumerator SmoothMove(Vector3 end)
    {
        float Distance = (transform.position - end).sqrMagnitude;
        while (Distance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(rd2D.position, end, inverseMoveTime * Time.deltaTime);
            rd2D.MovePosition(newPostion);
            Distance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        playerTurn = true;
    }

    IEnumerator TrapIgnore()
    {
        trapIgnore = true;
        yield return new WaitForSeconds(0.5f);
        trapIgnore = false;
    }

    IEnumerator HyperTime()
    {
        hyperTime = true;
        animator.SetTrigger("DefenceOn");
        yield return new WaitForSeconds(hyperTimeSecond);
        if (!finalityMode)
            animator.SetTrigger("DefenceOff");
        else if (finalityMode)
            animator.SetTrigger("FinalityMode");
        hyperTime = false;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Star")
        {
            GameInfo.instance.ScenesManage();
        }
        else if (other.tag == "Secret")
        {
            Debug.Log("Secret");
            GameInfo.instance.SetStar();
            other.gameObject.SetActive(false);
        }
    }

    bool OverlapPlayer()
    {
        RaycastHit2D hit;
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(transform.position, transform.position, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
            return false;
        else if (hit.transform.tag == "Monster" || hit.transform.tag == "Boss")
            return true;

        return false;
    }

    public void PlayerHit(int damage, GameObject who)
    {
        if (who.transform.tag == "Thunder")
        {
            playerHp -= damage;
            hpText.text = "HP : " + playerHp;
            return;
        }
        else if (!hyperTime)
        {
            playerHp -= damage;
            hpText.text = "HP : " + playerHp;
            return;
        }
        else if (hyperTime)
        {
            StopCoroutine(HyperTime());
            hyperTime = false;
            LookMonster(who.transform.position - transform.position);
            if (!finalityMode)
            {
                animator.SetTrigger("Attack");
            }
            else if (finalityMode)
            {
                animator.SetTrigger("FinalityAttack");
            }
            
            if (who.transform.tag == "Monster")
            {
                Monster mob = who.transform.GetComponent<Monster>();
                mob.MonsterHit(playerCounterAttack);
                finalityGauge += playerCounterAttack;
                finalityText.text = "Finality : " + finalityGauge;
            }
            else if (who.transform.tag == "Boss")
            {
                Boss mob = who.transform.GetComponent<Boss>();
                mob.BossHit(playerCounterAttack);
                finalityGauge += playerCounterAttack;
                finalityText.text = "Finality : " + finalityGauge;
            }
        }
    }

    void LookMonster(Vector2 pos)
    {
        if (pos == new Vector2(1, 0))
            derection = right;
        else if (pos == new Vector2(-1, 0))
            derection = left;
        else if (pos == new Vector2(0, 1))
            derection = front;
        else if (pos == new Vector2(0, -1))
            derection = back;
        transform.rotation = derection;
    }

}
