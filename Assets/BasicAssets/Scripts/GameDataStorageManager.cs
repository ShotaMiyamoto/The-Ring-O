using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDataStorageManager : SingletonMonoBehaviour<GameDataStorageManager>
{
    /// <summary>
    /// 生成されているかどうかをstaticな変数に格納する。
    /// </summary>
    private static bool created = false;


    //===========================広告処理関連の変数================================

    /// <summary>
    /// 何回バトルしたか（3回に一度インタースティシャル広告を再生する）
    /// </summary>
    [SerializeField] private int battleCount = 0;

    public int GetBattleCount { get { return battleCount; } }

    private Admob admob = default;

    //===========================対戦データ関連の変数================================


    /// <summary>
    /// 敵の勝利数
    /// </summary>
    private int character2WinCount = 0;
    public int Character2WinCount  //敵勝利数プロパティ
    { 
      set { character2WinCount = value; } 
      get { return character2WinCount; }
    } 

    /// <summary>
    /// プレイヤーの勝利数
    /// </summary>
    private int character1WinCount = 0;

    public int Character1WinCount //プレイヤー勝利数プロパティ
    { 
      set { character1WinCount = value; } 
      get { return character1WinCount; }
    }

    /// <summary>
    /// ラウンド数
    /// </summary>
    private int roundCount = 0;

    public int RoundCount //ラウンド数プロパティ
    {
        set { roundCount = value; }
        get { return roundCount; }
    }


    /// <summary>
    /// 敵のスペシャルポイント
    /// </summary>
    private float enemySpecialPoint = 0f;

    public float EnemySpecialPoint { set { enemySpecialPoint += value; } } //敵スペシャルポイントセッター

    /// <summary>
    /// プレイヤーのスペシャルポイント
    /// </summary>
    private float playerSpecialPoint = 0f;

    public float PlayerSpecialPoint { set { playerSpecialPoint = value; } } //プレイヤースペシャルポイントセッター



    //===========================CostumeとBodyColor関連の値================================

    /// <summary>
    /// ランダムで設定する見た目情報（BodyColor）
    /// </summary>
    private Dictionary<string, int> randomBodyColorNumDic = new Dictionary<string, int>();


    /// <summary>
    /// ランダムで設定する見た目情報（Costume）
    /// </summary>
    private Dictionary<string, int> randomCostumeNumDic = new Dictionary<string, int>();


    /// <summary>
    /// BodyColorマテリアル
    /// </summary>
    [SerializeField] private Material[] bodyColors = default;

    public int GetBColorMatLength { get { return bodyColors.Length; } }

    /// <summary>
    /// 対戦時に使用されるアイコン集(Costume)
    /// </summary>
    [SerializeField] private Sprite[] icons_Costume = default;

    private int GetIconCosLength { get { return icons_Costume.Length; } }

    /// <summary>
    /// 対戦時に使用されるアイコン集(BodyColor)
    /// </summary>
    [SerializeField] private Sprite[] icons_BodyColor = default;

    private int GetIconBColorLength { get { return icons_BodyColor.Length; } }




    //===========================セーブするデータ関連の変数================================

    /// <summary>
    /// 開放されている変換後のCostume番号  KEY:"UnlockedCostumeList" Int
    /// </summary>
    [SerializeField] private List<int> unLockedCostumeList = new List<int>();

    /// <summary>
    /// リストのゲッター
    /// </summary>
    public List<int> GetUnlockedCostumeList { get { return unLockedCostumeList; } }

    /// <summary>
    /// 開放されている変換後のCostumeリスト。リストをすべて文字列に変換して格納する KEY:"UnlockedCostumeList" String
    /// </summary>
    [SerializeField] private string unLockedCostumeListString = "0";


    /// <summary>
    /// 開放されている変換前のBodyColorリスト KEY:"UnlockedBodyColorList" Int
    /// </summary>
    [SerializeField] private List<int> unLockedBodyColorList = new List<int>();


    /// <summary>
    /// リストのゲッター
    /// </summary>
    public List<int> GetUnlockedBodyColorList { get { return unLockedBodyColorList; } }


    /// <summary>
    /// 開放されている変換後のBodyColorリスト。リストをすべて文字列に変換して格納する KEY:"UnlockedBodyColorList" String
    /// </summary>
    [SerializeField] private string unLockedBodyColorListString = "0";


    /// <summary>
    /// 選択中のCostume番号　KEY:SelectedCostumeNum Int
    /// </summary>
    [SerializeField] private int selectedCostumeNum = 0;
    
    //ゲッター
    public int GetSelectedCostumeNum { get { return selectedCostumeNum; } } 


    /// <summary>
    /// 選択中のBodyColor番号 KEY:SelectedBodyColorNum Int
    /// </summary>
    [SerializeField] private int selectedBodyColorNum = 0;

    //ゲッター
    public int GetSelectedBodyColorNum { get { return selectedBodyColorNum; } }




    /// <summary>
    /// 所持金 KEY:"Money" Int
    /// </summary>
    [SerializeField] private int money = 0;

    public int GetMoney { get { return money; } }


    /// <summary>
    /// シングルバトルモードでの最高連勝数 KEY:"BestConsecutiveWins" Int
    /// </summary>
    [SerializeField] private int bestConsecutiveWins = 0;

    /// <summary>
    /// 最高連勝数のプロパティ
    /// </summary>
    public int BestConsecutiveWinsProperty
    {
        get { return bestConsecutiveWins; }
        set { bestConsecutiveWins = value; }
    }

    /// <summary>
    /// キー一覧
    /// 所持金：Money
    /// 所持しているBodyColorList：UnlockedBodyColorList
    /// 所持しているCostumeList：UnlockedCostumeList
    /// 選択中のBodyColor番号：SelectedBodyColorNum
    /// 選択中のCostume番号：SelectedCostumeNum
    /// 連勝数：BestConsecutiveWins
    /// </summary>


    //===========================マルチバトル用================================

    //左プレイヤー

    private int selectingBColNum_Left = 0;
    public int SelecBColNumLeft { set { selectingBColNum_Left = value; }  get { return selectingBColNum_Left; } }

    private int selectingCosNum_Left = 0;
    public int SelecCosNumLeft { set { selectingCosNum_Left = value; }  get { return selectingCosNum_Left; } }


    //右プレイヤー

    private int selectingBColNum_Right = 0;
    public int SelecBColNumRight { set { selectingBColNum_Right = value; }  get { return selectingBColNum_Right; } }

    private int selectingCosNum_Right = 0;
    public int SelecCosNumRight { set { selectingCosNum_Right = value; }  get { return selectingCosNum_Right; } }



    //===========================セーブアイコン================================
    public Animator saveIconAnim = default;



    protected override void Awake()
    {

        //===================生成確認=======================
        base.Awake();

        if (!created)
        {
            DontDestroyOnLoad(this.gameObject); //シーンを切り替えても指定オブジェクトを残す
            created = true; //生成
            Debug.Log("作られてないので新しく作りますね" + name);
        }
        else
        {
            Destroy(this.gameObject);
        }



        //====================所持金データの同期・または作成=======================
        if (PlayerPrefs.HasKey("Money"))
        {
            //すでにキーがある
            money = PlayerPrefs.GetInt("Money", money); //データの同期。万が一ロード失敗したときは初期値をロードする
            Debug.Log("KEY:Money キーがある。　所持金：" + money);
        }
        else
        {
            //キーが無い
            Debug.Log("KEY:Money キーが無い。　所持金：" + money);
            PlayerPrefs.SetInt("Money", money); 
        }



        //====================所持Costumeデータの同期・または作成=======================
        if (PlayerPrefs.HasKey("UnlockedCostumeList"))
        {
            //すでにキーがある
            Debug.Log("KEY:UnlockedCostumeList キーがある。");
            unLockedCostumeListString = PlayerPrefs.GetString("UnlockedCostumeList");　//データの同期。万が一ロード失敗したときは初期値(0)をロードする

            //文字列を「,」で分割して文字列を数値化する
            foreach(var value in unLockedCostumeListString.Split(','))
            {
                unLockedCostumeList.Add(int.Parse(value));
                Debug.Log("所持コスチューム番号：" + int.Parse(value));
                unLockedCostumeList.Sort(); //昇順にソート
            }
        }
        else
        {
            //キーが無い
            unLockedCostumeList.Add(0); //0（初期値：リンゴ）を代入する
            unLockedCostumeListString = unLockedCostumeList[0].ToString(); //文字列に変換して代入
            PlayerPrefs.SetString("UnlockedCostumeList", unLockedCostumeListString);
            Debug.Log("KEY:UnlockedCostumeList キーが無い。　所持カラー番号：0");
        }


        //====================所持BodyColorデータの同期・または作成=======================
        if (PlayerPrefs.HasKey("UnlockedBodyColorList"))
        {
            //すでにキーがある
            Debug.Log("KEY:UnlockedBodyColorList キーがある。");
            unLockedBodyColorListString = PlayerPrefs.GetString("UnlockedBodyColorList"); //データの同期。万が一ロード失敗したときは初期値(0)をロードする

            //文字列を「,」で分割して文字列を数値化する
            foreach (var value in unLockedBodyColorListString.Split(','))
            {
                unLockedBodyColorList.Add(int.Parse(value));
                Debug.Log("所持BodyColor番号：" + int.Parse(value));
                unLockedBodyColorList.Sort(); //昇順にソート
            }
        }
        else
        {
            //キーが無い
            unLockedBodyColorList.Add(0); //0（初期値：黄色を代入する
            unLockedBodyColorList.Add(1); //1（初期値：白色）を代入する
            unLockedBodyColorListString = unLockedBodyColorList[0].ToString() + "," + unLockedBodyColorList[1].ToString(); //文字列に変換して代入
            Debug.Log("KEY:UnlockedBodyColorList キーが無い。　所持BodyColor番号：" + unLockedBodyColorListString);
        }



        //====================選択Costumeデータの同期・または作成=======================
        if (PlayerPrefs.HasKey("SelectedCostumeNum"))
        {
            //すでにキーがある
            selectedCostumeNum = PlayerPrefs.GetInt("SelectedCostumeNum"); //データの同期。
            Debug.Log("KEY:SelectedCostumeNum キーがある 　選択Costume番号：" + selectedCostumeNum);
        }
        else
        {
            //キーがない
            PlayerPrefs.SetInt("SelectedCostumeNum",selectedCostumeNum);
            Debug.Log("KEY:SelectedCostumeNum キーが無い。　選択Costume番号：0");
        }



        //====================選択BodyColorデータの同期・または作成=======================
        if (PlayerPrefs.HasKey("SelectedBodyColorNum"))
        {
            //すでにキーがある
            selectedBodyColorNum = PlayerPrefs.GetInt("SelectedBodyColorNum"); //データの同期。
            Debug.Log("KEY:SelectedBodyColorNum キーがある 　選択BodyColor番号：" + selectedBodyColorNum);
        }
        else
        {
            //キーがない
            PlayerPrefs.SetInt("SelectedBodyColorNum", selectedBodyColorNum);
            Debug.Log("KEY:SelectedBodyColorNum キーが無い。　選択BodyColor番号：0");
        }



        //====================連勝数データの同期・または作成=======================

        if (PlayerPrefs.HasKey("BestConsecutiveWins"))
        {
            //すでにキーがある
            bestConsecutiveWins = PlayerPrefs.GetInt("BestConsecutiveWins"); //データの同期。万が一ロード失敗したときは初期値をロードする
            Debug.Log("KEY:BestConsecutiveWins キーがある。　連勝数：" + bestConsecutiveWins);
        }
        else
        {
            //キーが無い
            PlayerPrefs.SetInt("BestConsecutiveWins", bestConsecutiveWins);
            Debug.Log("KEY:BestConsecutiveWins キーが無い。　連勝数：0");
        }


        //====================広告関連=======================
        admob = Admob.Instance;

    }


    //所持金の更新
    public void ChangeMoneyAmount(int amount)
    {
        if(amount > 99999)
        {
            money = 99999;
        }
        else
        {
            money = amount;
        }

        PlayerPrefs.SetInt("Money", money);
        DataSave();
    }


    /// <summary>
    /// Costumeの開放
    /// </summary>
    /// <param name="num"></param>
    public void UnlockCostume(int num)
    {
        unLockedCostumeList.Add(num); //リストに番号を追加
        unLockedCostumeList.Sort(); //昇順にソート

        bool isFirst = true; //フラグ
        foreach (var value in unLockedCostumeList) //文字列に変換
        {
            if (isFirst)
            {
                unLockedCostumeListString = value.ToString();
                isFirst = false;
                continue;
            }
            unLockedCostumeListString += ',' + value.ToString();
        }

        PlayerPrefs.SetString("UnLockedCostumeList", unLockedCostumeListString); //文字列をセット

        Debug.Log("所持Costume開放。データをセット：" + unLockedCostumeListString);
        Debug.Log(PlayerPrefs.GetString("UnLockedCostumeList"));
    }

    /// <summary>
    /// BodyColorの開放
    /// </summary>
    /// <param name="num"></param>
    public void UnlockBodyColor(int num)
    {
        unLockedBodyColorList.Add(num); //リストに番号を追加
        unLockedBodyColorList.Sort(); //昇順にソート

        bool isFirst = true; //フラグ
        foreach(var value in unLockedBodyColorList) //文字列に変換
        {
            if (isFirst)
            {
                unLockedBodyColorListString = value.ToString();
                isFirst = false;
                continue;
            }
            unLockedBodyColorListString += ',' + value.ToString();
        }

        PlayerPrefs.SetString("UnlockedBodyColorList", unLockedBodyColorListString); //文字列をセット

        Debug.Log("所持BodyColor開放。データをセット：" + unLockedBodyColorListString);
    }


    //選択Costumeを更新
    public void ChangeSelectedCostume(int itemNum)
    {
        selectedCostumeNum = itemNum;
    }

    //選択BodyColorを更新
    public void ChangeSelectedBodyColor(int itemNum)
    {
        selectedBodyColorNum = itemNum;
    }


    //指定したBodyColorを所持しているか
    public bool CheckBodyColorUnlocked(int num)
    {
        foreach(var value in unLockedBodyColorList)
        {
            if (num == value)
            {
                //同じ数字がある場合
                return true;
            }
        }

        //同じ数字が見当たらない場合
        return false;
    }

    //指定したCostumeを所持しているか
    public bool CheckCostumeUnlocked(int num)
    {
        foreach (var value in unLockedCostumeList)
        {
            if (num == value)
            {
                //同じ数字がある場合
                return true;
            }
        }

        //同じ数字が見当たらない場合
        return false;
    }



    /// <summary>
    /// データのセーブ
    /// </summary>
    public void DataSave()
    {
        //SaveImageのAnimatorがアタッチされていなければ、タグで検索してアタッチ
        if(saveIconAnim == null)
        {
            saveIconAnim = GameObject.FindGameObjectWithTag("SaveImage").GetComponent<Animator>();
            Debug.Log("SaveImageのAnimatorアタッチ");
        }
        

        //所持金
        PlayerPrefs.SetInt("Money", money);
        Debug.Log("所持金セット：" + money);

               
        //所持Costume番号
        PlayerPrefs.SetString("UnlockedCostumeList", unLockedCostumeListString);
        Debug.Log("所持Costume番号セット：" + unLockedCostumeListString);
               

        //所持BodyColor番号
        PlayerPrefs.SetString("UnlockedBodyColorList", unLockedBodyColorListString);
        Debug.Log("所持BodyColor番号セット：" + unLockedBodyColorListString);


        //選択Costume番号
        PlayerPrefs.SetInt("SelectedCostumeNum", selectedCostumeNum);
        Debug.Log("選択Costume番号セット" + selectedCostumeNum);


        //選択BodyColor番号
        PlayerPrefs.SetInt("SelectedBodyColorNum", selectedBodyColorNum);
        Debug.Log("選択BodyColor番号セット" + selectedBodyColorNum);


        //連勝数
        PlayerPrefs.SetInt("BestConsecutiveWins", bestConsecutiveWins);
        Debug.Log("連勝数セット：" + bestConsecutiveWins);
        

        //セーブ
        PlayerPrefs.Save();
        saveIconAnim.SetTrigger("doAnimation");
    }


    public void DeleteAllPlayerprefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Playerprefs削除");
    }


    /// <summary>
    /// バトル状況のリセット
    /// </summary>
    public void ResetBattleStatus()
    {
        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Battle_Single:
                character2WinCount = 0;
                character1WinCount = 0;
                roundCount = 0;
                ClearAllDicInfo(); //ランダムで選択した見た目とアイコンデータを削除
                break;

            case GameStateManager.GameState.Battle_Multi:
                selectingBColNum_Left = 0;
                selectingCosNum_Left = 0;
                selectingBColNum_Right = 0;
                selectingCosNum_Right = 0;

                character2WinCount = 0;
                character1WinCount = 0;
                roundCount = 0;
                ClearAllDicInfo(); //ランダムで選択した見た目とアイコンデータを削除
                break;

            default:
                Debug.Log("そんなステートでは使わないメソッドです");
                break;
        }


    }

    public void CheckShowAd()
    {
        battleCount++;
        if(battleCount % 3 == 0)
        {
            if(admob == null)
            {
                admob = GameObject.FindGameObjectWithTag("Admob").GetComponent<Admob>();
            }

            admob.ShowInterstitial();
            Debug.Log("3試合目なのでインタースティシャル広告を表示");
        }
    }


    /// <summary>
    /// マテリアル情報取得用
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public Material GetMatInfo(int num)
    {
        return bodyColors[num];
    }

    /// <summary>
    /// アイコンイメージ（BodyColor）取得用
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public Sprite GetBColorIcon(int num)
    {
        return icons_BodyColor[num];
    }

    /// <summary>
    /// アイコンイメージ（Costume）取得用
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public Sprite GetCostumeIcon(int num)
    {
        return icons_Costume[num];
    }


    //====================見た目ランダム系処理関連=======================

    public void ClearAllDicInfo()
    {
        randomBodyColorNumDic.Clear();
        randomCostumeNumDic.Clear();
    }

    //====================BodyColor関連=======================

    /// <summary>
    /// ランダムで出たBodyColor番号を設定する
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public void SetRandomBodyColorDic(string name,int num)
    {
        randomBodyColorNumDic.Add(name, num);
    }

    /// <summary>
    /// 設定したマテリアルを返す
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public Material GetRandomBodyColorDicMaterial(string name)
    {
        return GetMatInfo(randomBodyColorNumDic[name]);
    }


    /// <summary>
    /// アイコンを設定する用
    /// </summary>
    /// <returns></returns>
    public int GetEnemyBColorIconNum()
    {
        return randomBodyColorNumDic["Enemy"];
    }



    //====================Costume関連=======================

    /// <summary>
    /// ランダムで出たCostume番号を設定する
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public void SetRandomCostumeDic(string name,int num)
    {
        randomCostumeNumDic.Add(name, num);
    }


    /// <summary>
    /// 設定したコスチューム番号を返す
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public int GetRandomCostumeDicNum(string name)
    {
        return randomCostumeNumDic[name];
    }

    /// <summary>
    /// アイコンを設定する用
    /// </summary>
    /// <returns></returns>
    public int GetEnemyCosIconNum()
    {
        return randomCostumeNumDic["Enemy"];
    }
}
