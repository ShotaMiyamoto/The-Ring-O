using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Action用

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    //＝＝＝＝＝＝＝＝＝＝＝＝移動処理系＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] protected bool canMove = false; //サブクラスでも変更できるようにprotectedを使う
    [SerializeField] protected float accel = default;
    [SerializeField] protected float maxSpeed = default;
    protected Vector3 moveDir = new Vector3(0, 0, 0);

    protected Rigidbody rb;
    protected CapsuleCollider col; //地面の接地判定用
    [SerializeField] private Transform center = default;　//オブジェクトの中心点（体のMeshオブジェクト）


    //＝＝＝＝＝＝＝＝＝＝＝＝アニメーション処理系＝＝＝＝＝＝＝＝＝＝＝
    private Animator animator;

    //＝＝＝＝＝＝＝＝＝＝＝＝勝敗処理系＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private GameObject ragDoll = default; //負けた時に生成されるラグドール
    [SerializeField] private GameObject explosion = default; //負けたときに生成される爆発パーティクル
    [SerializeField] private float waitTimeForChangeCamera = 2f; //勝った時にカメラを遷移させるまでの待ち時間
    [SerializeField] private float waitTimeForIncreaseBattleCount = 2f; //カメラ遷移が終わった後にGameManagerへ勝数を反映するまでの待ち時間


    //＝＝＝＝＝＝＝＝＝＝＝＝衝突処理系＝＝＝＝＝＝＝＝＝＝＝
    private bool isTouching = false; //接触中かどうか
    private float speed = 0f;　//Forceを決めるためのSpeed値
    private float force = 5f; //速度によって変化する突き飛ばす力
    [SerializeField] private float maxForce = 8.5f; //突き飛ばす力の最大値
    [SerializeField] private float minForce = 3f;　//突き飛ばす力の最小値
    [SerializeField] private float magnif = 1.2f;　//突き飛ばす力の倍率
    [SerializeField] private GameObject hitEffectPrefab = default;
    [SerializeField] private GameObject hitHardEffectPrefab = default;


    //＝＝＝＝＝＝＝＝＝＝＝＝Costume処理用＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] protected GameObject[] costumes = default; //バトル用のコスチューム
    [SerializeField] protected SkinnedMeshRenderer bodyMesh = default; //マテリアルを適用するBodyMesh
    protected GameDataStorageManager gdsm; //GameDataStorageManagerのインスタンス
    [SerializeField] private bool doRandom = false; //見た目をランダム化するか否か
    [SerializeField] private bool canAllRandom = false; //見た目をランダム選択するときに、所持アイテムだけか全アイテムから選ぶかのフラグ


    //＝＝＝＝＝＝＝＝＝＝＝＝勝敗判定処理系＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private int cameraNum = 1; //負けたときに映すカメラ番号 1はプレイヤー（マルチだと左側）　2は敵（マルチだと右側）
    [SerializeField] private int charaNum = 0; 

    private void Start()
    {
        //＝＝＝＝＝＝＝＝＝＝＝＝移動処理系＝＝＝＝＝＝＝＝＝＝＝
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = center.position; //重心の設定

        //＝＝＝＝＝＝＝＝＝＝＝＝アニメーション処理系＝＝＝＝＝＝＝＝＝＝＝
        animator = GetComponent<Animator>();

        //＝＝＝＝＝＝＝＝＝＝＝＝Costume処理系＝＝＝＝＝＝＝＝＝＝＝
        gdsm = GameDataStorageManager.Instance;

        //tagによって処理差別化
        string tag = this.tag;

        switch (tag)
        {
            case "Player":
                if (doRandom)
                {
                    canAllRandom = false;
                    RandomizeAppear(); //所持アイテムのなかからランダムで設定
                }
                else
                {
                    SetPlayerAppear(); //設定しているアイテムを適用
                }
                break;

            case "Enemy":
                canAllRandom = true;
                RandomizeAppear();
                break;

            case "LeftPlayer":
                Debug.Log("パネルでの選択によって決定");
                break;

            case "RightPlayer":
                Debug.Log("パネルでの選択によって決定");
                break;

            default:
                SetPlayerAppear();
                Debug.Log("そんなタグ無いからプレイヤーの見た目を反映");
                break;
        }
    }


    /// <summary>
    /// プレイヤーが選択している情報を適用
    /// </summary>
    public virtual void SetPlayerAppear()
    {
        bodyMesh.material = gdsm.GetMatInfo(gdsm.GetSelectedBodyColorNum); //選択している値を設定
        costumes[gdsm.GetSelectedCostumeNum].SetActive(true); //選択している値を設定
    }


    /// <summary>
    /// 見た目ランダム化
    /// </summary>
    public void RandomizeAppear()
    {
        if (gdsm.RoundCount == 0)
        {
            //一回目

            int bodyColorNum;
            int costumeNum;

            //全アイテムから抽選するか、所持アイテムから抽選するか
            if (canAllRandom)
            {
                //全アイテムから抽選（Enemyのみ実行可）
                bodyColorNum = UnityEngine.Random.Range(0, gdsm.GetBColorMatLength); //全アイテムの中から選択
                costumeNum = UnityEngine.Random.Range(0, costumes.Length); //全アイテムの中から選択
            }
            else
            {
                //所持アイテムから抽選（プレイヤーでも実行可）
                int i = UnityEngine.Random.Range(0, gdsm.GetUnlockedBodyColorList.Count); //所持アイテムのリストの長さ中から選択
                int j = UnityEngine.Random.Range(0, gdsm.GetUnlockedCostumeList.Count); //所持アイテムのリストの長さから選択
                bodyColorNum = gdsm.GetUnlockedBodyColorList[i];　//抽選に出た番号の値を代入
                costumeNum = gdsm.GetUnlockedCostumeList[j];　//抽選に出た番号の値を代入
            }


            bodyMesh.material = gdsm.GetMatInfo(bodyColorNum); //ランダムで出た値を設定
            costumes[costumeNum].SetActive(true); //コスチュームをランダムで設定

            if(this.tag == "Player")
            {
                //プレイヤーなら所持しているコスチュームからランダム、あるいは選択したアイテム番号を保存
                gdsm.ChangeSelectedBodyColor(bodyColorNum);
                gdsm.ChangeSelectedCostume(costumeNum);
            }
            else
            {
                //プレイヤーでないならランダムデータとして設定
                gdsm.SetRandomBodyColorDic(this.name, bodyColorNum); //シーンロードで消えないようにデータ保存
                gdsm.SetRandomCostumeDic(this.name, costumeNum);　//シーンロードで消えないようにデータ保存
            }

            Debug.Log("ランダム外見データを代入");
        }
        else
        {
            //二回目以降
            if (this.tag == "Player")
            {
                bodyMesh.material = gdsm.GetMatInfo(gdsm.GetSelectedBodyColorNum);
                costumes[gdsm.GetSelectedCostumeNum].SetActive(true);
                Debug.Log("プレイヤーの設定した見た目同期");
            }
            else
            {
                bodyMesh.material = gdsm.GetRandomBodyColorDicMaterial(this.name); //ランダムで出た値を取得して設定
                costumes[gdsm.GetRandomCostumeDicNum(this.name)].SetActive(true); //コスチュームをランダムで設定
                Debug.Log("ランダムの見た目を同期");
            }
        }
    }

    /// <summary>
    /// PlayerとEnemyでそれぞれこのUpdateをoverrideしてmoveDirのxとzに数値を代入していく
    /// </summary>
    public virtual void Update()
    {
        //回転処理（PlayerはJoystickの方向へ、EnemyはPlayerの方向を見る）
        if(moveDir.x != 0 || moveDir.z != 0)
        {
            Vector3 lookDir = new Vector3(moveDir.x, 0, moveDir.z);
            transform.localRotation = Quaternion.LookRotation(lookDir);
        }


        //アニメーション処理
        if (rb.velocity.magnitude > 0.01f)
        {
            //アニメーション処理(走りアニメーション)
            animator.SetFloat("Speed_f", 1f);
        }
        else
        {
            //アニメーション処理(停止アニメーション)
            animator.SetFloat("Speed_f", 0f);
        }

        //衝突処理
        speed = rb.velocity.magnitude;
        force = Mathf.Clamp(speed * magnif, minForce, maxForce);
    }

    /// <summary>
    /// 移動実行（プレイヤーとEnemyによって方向代入の仕方が変わる）
    /// </summary>
    public virtual void FixedUpdate()
    {
        //地面に接している。動く許可はされているか。バトルは終わっていないか。
        if (IsGrounded() && canMove && !GameManager.Instance.IsBattleEnded)
        {
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDir * accel);
            }
        }
    }


    /// <summary>
    /// 接地判定
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded() 
    {
        if (Physics.CheckSphere(this.transform.position + new Vector3(0, 0.2f, 0), 0.2f, LayerMask.GetMask("Ground"))) //足元に半径0.2fのSphereを作って接地判定（半径分上にずらして球の下部が地面に接するようにする）
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (isEnable == false  || col == null)
    //        return;

    //    ray = new Ray(this.transform.position, Vector3.down);

    //    var isHit = Physics.CheckSphere(this.transform.position + new Vector3(0, 0.2f, 0), 0.2f, LayerMask.GetMask("Ground"));
    //    if (isHit)
    //    {
    //        Debug.Log("True");
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(transform.position + new Vector3(0,0.2f,0), 0.2f);
    //    }
    //    else
    //    {
    //        Debug.Log("False");
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(transform.position + new Vector3(0,0.2f,0), 0.2f);
    //    }
    //}

    /// <summary>
    /// cameraNumはカメラ
    /// </summary>
    /// <param name="cameraNum"></param>
    /// <param name="winner"></param>
    public virtual void Win()
    {
        //移動の無効化
        canMove = false;
        GameManager.Instance.IsBattleEnded = true;

        //速度リセット
        rb.velocity = new Vector3(0, 0, 0);

        //ステージ中央に場所移動
        this.transform.position = new Vector3(0, 0, 0);
        this.transform.rotation = new Quaternion(0, 0, 0, 0);

        //停止アニメーションに設定
        animator.SetFloat("Speed_f", 0f);

        //ダンス開始
        animator.SetLayerWeight(animator.GetLayerIndex("Idles"), 0f); //腕組みレイヤーのウエイトを無くす
        animator.SetTrigger("canDance");
        animator.SetInteger("Dance_int", UnityEngine.Random.Range(0,6));


        //勝った方にカメラの遷移
        StartCoroutine(DelayMethod(waitTimeForChangeCamera ,() => 
        {
            int cameraNum = 3; //WinnerCameraの番号
            CameraManager.Instance.ChangeLookAt(cameraNum, this.gameObject);
            CameraManager.Instance.ChangeCamera(cameraNum);

            //勝利数追加と勝ち星アニメーションの再生
            StartCoroutine(DelayMethod(waitTimeForIncreaseBattleCount, () =>
            {
                switch (GameStateManager.Instance.StateProperty)
                {
                    case GameStateManager.GameState.Battle_Single:
                        GameManager.Instance.IncreaseSingleBattleCount(charaNum);
                        break;

                    case GameStateManager.GameState.Battle_Multi:
                        GameManager.Instance.IncreaseMultiBattleCount(charaNum);
                        break;

                    default:
                        Debug.Log("そんなステートでは勝利数は増えない");
                        break;
                }
                
            }));

        }));

    }

    public virtual void Lose()
    {
        //爆発パーティクルとラグドールの生成
        Instantiate(explosion, this.gameObject.transform.position, explosion.transform.rotation);
        GameObject obj = Instantiate(ragDoll, this.gameObject.transform.position + new Vector3(0, 1.5f, 0), ragDoll.transform.rotation);

        switch (charaNum)
        {
            case 0://プレイヤー側
                obj.name = this.name;
                obj.GetComponent<RugdollController>().SyncAppear(0);
                break;
            case 1:　//敵側
                obj.name = this.name;
                obj.GetComponent<RugdollController>().SyncAppear(1);
                break;
            default:
                Debug.Log("そんなカメラ番号はない");
                break;
        }

        //負けたほうに一時的にカメラを切り替える
        CameraManager.Instance.ChangeCamera(cameraNum);
        CameraManager.Instance.ChangeLookAt(cameraNum, obj);
        CameraManager.Instance.ChangeFollow(cameraNum, obj);

        //時間をスローにする
        GameManager.Instance.TimeSlower(0.2f, 3f);

        //オブジェクト破棄
        Destroy(this.gameObject);
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    /// <summary>
    /// EnemyがoverrideしてWaitメソッドを呼べるようにする
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "LeftPlayer" || other.gameObject.tag == "RightPlayer")
        {
            if (!isTouching)
            {
                isTouching = true; //接触フラグ有効化
                //Debug.Log(this.name +":  " + "speed:" + speed + "  force:" + force);
                Vector3 dir = other.gameObject.transform.position - this.transform.position;
                other.gameObject.GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.VelocityChange); //質量無視のImpulse
                Debug.Log("Force : " + force + "  Name : " + this.gameObject.name);

                if (force > maxForce - 0.3f) //強打なら赤いエフェクト
                {
                    Instantiate(hitHardEffectPrefab, other.contacts[0].point, hitHardEffectPrefab.transform.rotation);
                }
                else
                {
                    Instantiate(hitEffectPrefab, other.contacts[0].point, hitEffectPrefab.transform.rotation);
                }

                ////スペシャル関係
                //specialController.SetSpecialPoint(force);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "LeftPlayer" || collision.gameObject.tag == "RightPlayer")
        {
            isTouching = false;　//接触フラグ無効化
        }
    }
}
