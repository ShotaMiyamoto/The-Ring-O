using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteManager : MonoBehaviour
{
    private bool canRotate = false;
    private bool doSlowDown = false;

    [SerializeField] private float rotateSpeed = 10f;
    private float coefficient = 0f; //減速するための係数
    private float rotatedAngle = 0f; //回転したアングル

    [SerializeField] private GameObject startButton = default;
    [SerializeField] private GameObject stopButton = default;

    private float targetRot = 0f; //止める角度
    private int roundCount = 0;　//一周回転した回数
    private int roundCountWhenStopped = 0; //ストップをかけられたときの回転数

    [SerializeField] private GameObject closeButton = default;

    private int prizeID; //ゲットしたアイテム番号
    [SerializeField] private GameObject getItemPanel = default;
    [SerializeField] private Image prizeImage = default;
    public Sprite moneyImage = default;
    public Text rewardText = default;
    public Sprite[] commonBodyColorImages = default;
    public Sprite[] commonCostumeImages = default;
    public Sprite[] rareBodyColorImages = default;
    public Sprite[] rareCostumeImages = default;

    /// <summary>
    /// アイテム情報を保持する辞書
    /// </summary>
    private Dictionary<int, float> itemInfo;


    /// <summary>
    /// アイテムの発生確率を保持する辞書
    /// </summary>
    private Dictionary<int, float> itemDropDict;

    [SerializeField] private MoneyManager moneyManager;

    private GameDataStorageManager gdsm = default;

    // Start is called before the first frame update
    void Start()
    {
        gdsm = GameDataStorageManager.Instance;
        SetActivateStopButton(false);
        SetActivateStartButton(true);
        closeButton.SetActive(false);
        getItemPanel.SetActive(false); //獲得パネルを非表示
        if(moneyManager == null)
        {
            moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>(); //MoneyManagerを参照
        }
        //TestChoose();
    }

    // Update is called once per frame
    void Update()
    {
        if (canRotate)
        {
            if(rotateSpeed > 2f)
            {
                if (doSlowDown && roundCount > 3)
                {
                    //Debug.Log("減速中");
                    rotateSpeed *= coefficient;
                    DoRotate();
                }
                else
                {
                    //ストップボタンが押されるまで回転し続ける
                    DoRotate();
                }
            }
            else
            {
                //ストップがかかってから1周回る
                if(roundCount > roundCountWhenStopped + 2)
                {
                    //ターゲット角度設定
                    Quaternion targetAngle = Quaternion.Euler(new Vector3(0, 0, targetRot));
                    Quaternion currentAngle = this.transform.rotation;

                    if (Quaternion.Angle(currentAngle, targetAngle) <= 1)
                    {
                        Debug.Log("ターゲット角度に到着");
                        transform.rotation = targetAngle;
                        closeButton.SetActive(true); //Closeボタン表示
                        canRotate = false;
                        Invoke("GetReward", 0.5f);
                    }
                    else
                    {
                        //Debug.Log("ターゲット角度に移動中");
                        DoRotate();
                    }
                }
                else
                {
                    DoRotate();
                }
            }
        }
    }

    public void DoRotate()
    {
        transform.Rotate(0, 0, rotateSpeed);

        //周回計測
        rotatedAngle += rotateSpeed;
        if (rotatedAngle > 360)
        {
            roundCount++;
            //Debug.Log(roundCount + "週目");
            rotatedAngle = 0f;
        }
    }

    public void StartOnClick()
    {
        canRotate = true;
        SetActivateStartButton(false);
        SetActivateStopButton(true);
    }

    public void StopOnClick()
    {
        GetItem(); //抽選開始して止める角度を代入する
        doSlowDown = true;
        coefficient = Random.Range(0.92f, 0.98f);
        roundCountWhenStopped = roundCount;
        SetActivateStopButton(false);
    }

    public void SetActivateStartButton(bool value)
    {
        startButton.SetActive(value);
    }

    public void SetActivateStopButton(bool value)
    {
        stopButton.SetActive(value);
    }



    private void InitializeDicts()
    {
        //Dictionary初期化

        itemInfo = new Dictionary<int, float>();
        itemInfo.Add(0, 315f); //1番目　ノーマル：+20円
        itemInfo.Add(1,  45f); //2番目　ノーマル：+30円
        itemInfo.Add(2, 225f); //3番目　ノーマル：+50円 
        itemInfo.Add(3, 135f); //4番目　もうワンチャンス
        itemInfo.Add(4, 270f); //5番目　ノーマル：コモンBodyColor
        itemInfo.Add(5,  90f); //6番目　ノーマル：コモンCostume
        itemInfo.Add(6, 180f); //7番目　レア：レアBodyColor　
        itemInfo.Add(7,   0f); //8番目  レア：レアCostume


        itemDropDict = new Dictionary<int, float>();

        itemDropDict.Add(0, 20f); //1番目　ノーマル：+20円
        itemDropDict.Add(1, 18f); //2番目　ノーマル：+30円
        itemDropDict.Add(2, 16f); //3番目　ノーマル：+50円
        itemDropDict.Add(3, 14f); //4番目　もうワンチャンス
        itemDropDict.Add(4, 12f); //5番目　ノーマル：コモンBodyColor
        itemDropDict.Add(5, 10f); //6番目　ノーマル：コモンCostume
        itemDropDict.Add(6,  7f); //7番目　レア：レアBodyColor　
        itemDropDict.Add(7,  3f); //8番目  レアCostume

    }

    private void GetItem()
    {
        //辞書初期化
        InitializeDicts();

        //アイテムの抽選と対応する角度の取得
        prizeID = Choose();
        targetRot = itemInfo[prizeID];
        Debug.Log(itemInfo[prizeID] + "の角度に当選");
    }

    private int Choose()
    {
        // 確率の合計値を格納
        float total = 0;

        // 敵ドロップ用の辞書からドロップ率を合計する
        foreach (KeyValuePair<int, float> elem in itemDropDict)
        {
            total += elem.Value;
        }

        // Random.valueでは0から1までのfloat値を返すので
        // そこにドロップ率の合計を掛ける
        float randomPoint = Random.value * total;

        // randomPointの位置に該当するキーを返す
        foreach (KeyValuePair<int, float> elem in itemDropDict)
        {
            if (randomPoint < elem.Value)
            {
                return elem.Key;
            }
            else
            {
                randomPoint -= elem.Value;
            }
        }
        return 0;
    }

    private void GetReward()
    {
        int itemID = 0; //保存されているアイテムの番号
        int randomNum; //ランダムで抽選される番号

        switch (prizeID)
        {
            case 0: //1番目　ノーマル：+20円
                prizeImage.sprite = moneyImage;
                rewardText.text = ("+20 COINS");

                if (moneyManager != null)
                {
                    moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + 20);
                    Debug.Log("20円当選！");
                }
                else
                {
                    Debug.Log("MoneyManagerが無い");
                }
                
                break;

            case 1: //2番目　ノーマル：+30円
                prizeImage.sprite = moneyImage;
                rewardText.text = ("+30 COINS");
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + 30);
                    Debug.Log("30円当選！");

                break;

            case 2: //3番目　ノーマル：+50円
                prizeImage.sprite = moneyImage;
                rewardText.text = ("+50 COINS");
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + 50);
                Debug.Log("50円当選！");

                break;

            case 3: //4番目　もうワンチャンス
                SetActivateStartButton(true);

                Debug.Log("もう一度ルーレットスタート！");
                break;

            case 4: //5番目　ノーマル：コモンBodyColor
                randomNum = Random.Range(0, commonBodyColorImages.Length);
                prizeImage.sprite = commonBodyColorImages[randomNum];
                rewardText.text = ("New Body Color!");

                switch (randomNum)
                {
                    case 0:
                        itemID = 12; 
                        break;

                    case 1:
                        itemID = 13;
                        break;

                    case 2:
                        itemID = 14;
                        break;

                    case 3:
                        itemID = 15;
                        break;

                    case 4:
                        itemID = 16;
                        break;

                    case 5:
                        itemID = 17;
                        break;

                    default:
                        Debug.Log("そんなアイテムない");
                        break;
                }

                Debug.Log("コモンBodyColor :" + itemID + "番当選！");

                if (!gdsm.CheckBodyColorUnlocked(itemID))
                {
                    gdsm.UnlockBodyColor(itemID);
                }
                else
                {
                    Debug.Log("すでに持ってるBodyColor");
                }

                break;

            case 5: //6番目　ノーマル：コモンCostume
                randomNum = Random.Range(0, commonCostumeImages.Length);
                prizeImage.sprite = commonCostumeImages[randomNum];
                rewardText.text = ("New Costume!");

                itemID = 0;
                switch (randomNum)
                {
                    case 0:
                        itemID = 8;
                        break;

                    case 1:
                        itemID = 9;
                        break;

                    case 2:
                        itemID = 10;
                        break;

                    case 3:
                        itemID = 11;
                        break;

                    default:
                        Debug.Log("そんなアイテムない");
                        break;
                }

                Debug.Log("コモンCostume :" + itemID + "番当選！");

                if (!gdsm.CheckCostumeUnlocked(itemID))
                {
                    gdsm.UnlockCostume(itemID);
                }
                else
                {
                    Debug.Log("すでに持ってるCostume");
                }

                break;
            
            case 6: //7番目　レア：レアBodyColor　
                randomNum = Random.Range(0, rareBodyColorImages.Length);
                prizeImage.sprite = rareBodyColorImages[randomNum];
                rewardText.text = ("New Body Color!");

                itemID = 0;
                switch (randomNum)
                {
                    case 0:
                        itemID = 18;
                        break;

                    case 1:
                        itemID = 19;
                        break;

                    case 2:
                        itemID = 20;
                        break;

                    case 3:
                        itemID = 21;
                        break;

                    case 4:
                        itemID = 22;
                        break;

                    case 5:
                        itemID = 23;
                        break;

                    default:
                        Debug.Log("そんなアイテムない");
                        break;
                }

                Debug.Log("レアBodyColor :" + itemID + "番当選！");

                if (!gdsm.CheckBodyColorUnlocked(itemID))
                {
                    gdsm.UnlockBodyColor(itemID);
                }
                else
                {
                    Debug.Log("すでに持ってるBodyColor");
                }

                break;

            case 7: //8番目  レアCostume
                randomNum = Random.Range(0, rareCostumeImages.Length);
                prizeImage.sprite = rareCostumeImages[randomNum];
                rewardText.text = ("New Costume!");
                itemID = 0;
                switch (randomNum)
                {
                    case 0:
                        itemID = 12;
                        break;

                    case 1:
                        itemID = 13;
                        break;

                    case 2:
                        itemID = 14;
                        break;

                    case 3:
                        itemID = 15;
                        break;

                    default:
                        Debug.Log("そんなアイテムない");
                        break;
                }

                Debug.Log("レアCostume :" + itemID + "番当選！");

                if (!gdsm.CheckCostumeUnlocked(itemID))
                {
                    gdsm.UnlockCostume(itemID);
                }
                else
                {
                    Debug.Log("すでに持ってるCostume");
                }

                break;

            default:
                Debug.Log("そんな当選番号はない！");
                break;
        }


        if(prizeID != 3)
        {
            getItemPanel.SetActive(true); //獲得パネルを表示
        }
            
    }

    private void TestChoose()
    { 
        InitializeDicts();

        int j;
        int[] nums = { 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < 1000; i++)
        {
            j = Choose();
            nums[j]++;
        }

    
        Debug.Log("1番は　" + nums[1] + "回当たった");
        Debug.Log("2番は　" + nums[2] + "回当たった");
        Debug.Log("3番は　" + nums[3] + "回当たった");
        Debug.Log("4番は　" + nums[4] + "回当たった");
        Debug.Log("5番は　" + nums[5] + "回当たった");
        Debug.Log("6番は　" + nums[6] + "回当たった");
        Debug.Log("7番は　" + nums[7] + "回当たった");
    }
}
