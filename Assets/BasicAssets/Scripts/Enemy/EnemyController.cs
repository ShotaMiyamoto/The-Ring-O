using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Character
{
    [SerializeField] private GameObject follower = default; //追う対象
    public bool SetCanMove { set { base.canMove = value; } }

    [SerializeField] private float waitTime = 1f;　//待ち時間

    private float timeElapsed = 0f; //対象と接触してからの経過時間

    private bool canCount = false; //カウントできるかどうか

    public override void Update()
    {
        //Playerと自分の位置の差分から移動方向を設定
        if (!GameManager.Instance.IsBattleEnded)
        {
            moveDir.x = follower.transform.position.x - transform.position.x;
            moveDir.z = follower.transform.position.z - transform.position.z;
            base.Update();
        }

        //プレイヤーに接触して動き出すまでの待ち時間を計測
        if (canCount)
        {
            timeElapsed += Time.deltaTime;
            
            if(timeElapsed > waitTime)
            {
                canMove = true; //移動許可
                canCount = false; //カウント停止
            }
        }

    }

    public override void Win()
    {
        base.canMove = false;
        base.Win(); //0がプレイヤー番号 1が敵の番号 
    }

    public override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            base.OnCollisionEnter(other);
            canMove = false; //動きを停止
            canCount = true; //計測開始
            timeElapsed = 0f; //待ち経過時間リセット
        }
    }

}
