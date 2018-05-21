using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    private float inverseMoveTime;

    public int monsterHp = 200;
    public int monsterDamage = 0;
    bool monsterMove = true;//ture 이동가능 false 이동불가
    bool monsterAttack = false; //true 공격가능 false 공격대기해야함
    bool configMove;//가로 : true, 세로 : false;

    Vector2 playerPosition;
    Vector3 beforePosition;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rd2D;
    private Animator animator;

    // Use this for initialization
    void Start () {
        playerPosition = GameInfo.instance.playerPosition;
        boxCollider = GetComponent<BoxCollider2D>();
        rd2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inverseMoveTime = 0.5f / moveTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!monsterMove||monsterAttack) return;

        if (Overlap())
        {
            transform.position = beforePosition;
            return;
        }

        RaycastHit2D hit;
        playerPosition = GameInfo.instance.playerPosition;
        Vector2 start = transform.position;
        Vector2 end=new Vector2(0,0);

        int dirX = (int)playerPosition.x - (int)transform.position.x;
        int dirY = (int)playerPosition.y - (int)transform.position.y;
        if (Mathf.Abs(dirX) == 1 && Mathf.Abs(dirY) == 0)
        {
            MonsterAttack(dirX, dirY);
            return;
        }
        else if (Mathf.Abs(dirX) == 0 && Mathf.Abs(dirY) == 1)
        {
            MonsterAttack(dirX, dirY);
            return;
        }
        else if (Mathf.Abs(dirX) >= Mathf.Abs(dirY))
        {
            end = dirX > float.Epsilon ? start + new Vector2(1, 0) : start + new Vector2(-1, 0);
            configMove = true;
        }
        else if (Mathf.Abs(dirX) <= Mathf.Abs(dirY))
        {
            end = dirY > float.Epsilon ? start + new Vector2(0, 1) : start + new Vector2(0, -1);
            configMove = false;
        }
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        
        if (hit.transform == null)
        {
            beforePosition = start;
            Move(end);
            monsterMove = false;
        }
        else if (hit.transform.tag == "Monster")
        {
            if(configMove)
            {
                end = dirY > float.Epsilon ? start + new Vector2(0, 1) : start + new Vector2(0, -1);
            }
            else if(!configMove)
            {
                end = dirX > float.Epsilon ? start + new Vector2(1, 0) : start + new Vector2(-1, 0);
            }
            beforePosition = start;
            Move(end);
            monsterMove = false;
        }
        
    }

    bool Overlap()
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
        monsterMove = true;
    }

    void MonsterAttack(int dirX,int dirY)
    {
        RaycastHit2D hit;
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(dirX, dirY);
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        
        if (hit.transform.tag=="Player")
        {
            if (!monsterAttack)
            {
                animator.SetTrigger("MonsterAttackWait");
                StartCoroutine(WaitAttack(start, end, hit));
            }
        }
    }

    IEnumerator WaitAttack(Vector2 start,Vector2 end,RaycastHit2D hit)
    {
        monsterAttack = true;
        monsterMove = false;
        yield return new WaitForSeconds(1f);
        
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        if (hit.transform == null || hit.transform.tag == "Monster" || hit.transform.tag == "Boos")
        {
            animator.SetTrigger("MonsterAttackfail");
            monsterAttack = false;
            monsterMove = true;
        }
        else if (hit.transform.tag == "Player")
        {
            animator.SetTrigger("MonsterAttack");
            Player player = hit.transform.GetComponent<Player>();
            player.PlayerHit(monsterDamage, this.gameObject);
            monsterAttack = false;
            monsterMove = true;
        }
        
    }

    public void MonsterHit(int damage)
    {
        monsterHp -= damage;
        if (monsterHp <= 0)
        {
            GameInfo.instance.monsterKill--;
            Destroy(this.gameObject);
            
        }
    }
}
