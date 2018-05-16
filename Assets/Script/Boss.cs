using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    private float inverseMoveTime;

    public int bossHp = 2000;
    public int bossDamage = 70;
    bool bossMove = true;//ture 이동가능 false 이동불가
    bool bossAttack = false; //true 공격가능 false 공격대기해야함
    bool patternAttack = false;
    bool monsterSummon = false;

    public GameObject[] monsters;
    public GameObject warning;
    public Text bossHpText;

    Vector2 playerPosition;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rd2D;
    private Animator animator;

    // Use this for initialization
    void Start () {
        bossHpText.text = "Boss : " + bossHp;
        playerPosition = GameInfo.instance.playerPosition;
        boxCollider = GetComponent<BoxCollider2D>();
        rd2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inverseMoveTime = 0.5f / moveTime;
    }

    void Update()
    {
        playerPosition = GameInfo.instance.playerPosition;
        if (!bossMove) return;

        RaycastHit2D hit;
        
        Vector2 start = transform.position;
        Vector2 end = new Vector2(0, 0);

        int dirX = (int)playerPosition.x - (int)transform.position.x;
        int dirY = (int)playerPosition.y - (int)transform.position.y;

        if(!patternAttack)
        {
            StartCoroutine(ThunderAttack());
            patternAttack = true;
        }

        if(!monsterSummon)
        {
            StartCoroutine(MonsterSummon());
            monsterSummon = true;
        }

        if (DetectingPlayer(dirX, dirY)) return;
        else if (Mathf.Abs(dirX) >= Mathf.Abs(dirY))
        {
            end = dirX > float.Epsilon ? start + new Vector2(1, 0) : start + new Vector2(-1, 0);
        }
        else if ((Mathf.Abs(dirX) < Mathf.Abs(dirY)))
        {
            end = dirY > float.Epsilon ? start + new Vector2(0, 1) : start + new Vector2(0, -1);
        }
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            Move(end);
            bossMove = false;
        }

    }


    void Move(Vector3 end)
    {
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
        yield return new WaitForSeconds(1f);
        bossMove = true;
    }

    void BossAttack(float dirX, float dirY)
    {
        RaycastHit2D hit;
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(dirX, dirY);
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform.tag == "Player")
        {
            if (!bossAttack)
            {
                animator.SetTrigger("BossAttackWait");
                StartCoroutine(WaitAttack(start, end, hit));
            }
        }
    }

    IEnumerator WaitAttack(Vector2 start, Vector2 end, RaycastHit2D hit)
    {
        bossAttack = true;
        bossMove = false;
        yield return new WaitForSeconds(1f);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        if (hit.transform == null)
        {
            animator.SetTrigger("BossAttackfail");
            bossAttack = false;
            bossMove = true;
        }
        else if (hit.transform.tag == "Player")
        {
            animator.SetTrigger("BossAttack");
            Player player = hit.transform.GetComponent<Player>();
            player.PlayerHit(bossDamage,this.gameObject);
            bossAttack = false;
            bossMove = true;
        }
    }

    bool DetectingPlayer(int dirX, int dirY)
    {
        if (dirX == -2)
        {
            if (dirY == 0 || dirY == -1)
            {
                Debug.Log(dirY == 0 || dirY == -1 && dirX == -2);
                BossAttack(-1.5f, 0);
                return true;
            }
        }
        else if (dirX == 1)
        {
            if (dirY == 0 || dirY == -1)
            {
                Debug.Log(dirX + " 2 " + dirY);
                BossAttack(1.5f, 0);
                return true;
            }
        }
        else if (dirY == 1)
        {
            if (dirX == 0 || dirX == -1)
            {
                Debug.Log(dirX + " 3 " + dirY);
                BossAttack(0, 1.5f);
                return true;
            }
        }
        else if (dirY == -2)
        {
            if (dirX == 0 || dirX == -1)
            {
                Debug.Log(dirX + " 4 " + dirY);
                BossAttack(0, -1.5f);
                return true;
            }
        }

        return false;
    }
    IEnumerator ThunderAttack()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject thunder = warning;
            GameObject temp = Instantiate(thunder, playerPosition, Quaternion.identity) as GameObject;
            yield return new WaitForSeconds(0.8f);
        }
        patternAttack = false;
    }

    IEnumerator MonsterSummon()
    {
        GameObject summon = monsters[Random.Range(0, monsters.Length)];
        GameObject temp = Instantiate(summon, new Vector3(8.5f, 0.5f), Quaternion.identity) as GameObject;
        GameInfo.instance.monsterKill++;
        yield return new WaitForSeconds(15f);
        monsterSummon = false;
    }

    public void BossHit(int damage)
    {
        bossHp -= damage;
        bossHpText.text = "Boss : " + bossHp;
        if (bossHp <= 0)
        {
            gameObject.SetActive(false);
            GameInfo.instance.SetStar();
            GameInfo.instance.bossKill++;
        }
        
    }

    

}
