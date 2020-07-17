using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System; //Action用
using UnityEngine.Playables;

public class GameManager : SingletonMonoBehaviour<GameManager>
{

    //＝＝＝＝＝＝＝＝＝＝＝＝所持金処理用＝＝＝＝＝＝＝＝＝＝＝
    public MoneyManager moneyManager;

    //＝＝＝＝＝＝＝＝＝＝＝＝時間処理系＝＝＝＝＝＝＝＝＝＝＝

    private float timeElapsed = 0;
    private bool canStartToCount = false;
    private float timeLimit = 0;


    //＝＝＝＝＝＝＝＝＝＝＝＝勝利数処理系＝＝＝＝＝＝＝＝＝＝＝

    /// <summary>
    /// バトル開始時や終了に情報を出し入れする用
    /// </summary>
    private GameDataStorageManager gameDataStorageManager;

    private int roundCount = 0;
    public int GetRoundCount { get { return roundCount; } }

    private int character1WinCount = 0; //シングル：プレイヤー　マルチ：左側
    private int character2WinCount = 0; //シングル：NPC　マルチ：右側

    //＝＝＝＝＝＝＝＝＝＝＝＝累計連勝数処理系＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private ConsecutiveWinsManager consecWinManager = default;

    //＝＝＝＝＝＝＝＝＝＝＝＝勝敗管理系＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private bool isBattleEnded = false;
    public bool IsBattleEnded {
        set { isBattleEnded = value; }
        get { return isBattleEnded; }
    }

    //＝＝＝＝＝＝＝＝＝＝＝＝バトル開始処理系＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private bool canStartBattle = true; //バトルを開始できるかどうか
    [SerializeField] private PlayableDirector readyTimeLine = default; //タイムライン用

    //＝＝＝＝＝＝＝＝＝＝＝＝ロードシーン処理系＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private bool canLoadScene = false;

    //＝＝＝＝＝＝＝＝＝＝＝＝ルーレットパネル処理系＝＝＝＝＝＝＝＝＝＝＝
    private bool isOpened = false;

    //＝＝＝＝＝＝＝＝＝＝＝＝マルチプレイ処理系＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private int multiPrizePrice = 15;


    // Start is called before the first frame update
    void Start()
    {
        canLoadScene = false; //次のシーンロードを不可能にする        

        gameDataStorageManager = GameDataStorageManager.Instance;
        SyncRoundStatus(); //ラウンド情報の取得とUIの更新

        Debug.Log(gameDataStorageManager.RoundCount + "ラウンド目");

        if (gameDataStorageManager.RoundCount > 2 || gameDataStorageManager.Character1WinCount >= 2 || gameDataStorageManager.Character2WinCount >= 2)
        {
            //バトル終了
            Debug.Log("試合停止");
        }
        else 
        {
            //すぐにバトル初めていいなら
            if (canStartBattle || roundCount != 0)
            {
                StartBattle();
                Debug.Log("試合開始");
            }
            else
            {
                Debug.Log("カスタマイズ画面表示");
            }
        }

        //Debug.Log("TimeScaleリセット");
        Time.timeScale = 1f; //Timescaleリセット
        timeElapsed = 0f;
        canStartToCount = false;

    }

    // Update is called once per frame
    void Update()
    {
        //===================Timescale関係=====================
        if (canStartToCount)
        {
            timeElapsed += Time.unscaledDeltaTime;
            if (timeElapsed > this.timeLimit)
            {
                canStartToCount = false;
                timeElapsed = 0f;
                Time.timeScale = 1f;　//もとに戻す
                Debug.Log("タイムスロー終わり");
            }
        }

        //===================SceneLoad関係=====================
        if(canLoadScene && Input.GetMouseButtonDown(0))
        {
            Admob.Instance.DestroyBanner(); //バナー削除
            ReloadScene();
        }
    }


    public void StartBattle()
    {
        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 1, true); //カウントダウン開始
        readyTimeLine.Play(); //タイムライン再生

        if(GameStateManager.Instance.StateProperty == GameStateManager.GameState.Battle_Multi)
        {
            Invoke("SyncMultiBattleIcons", 0.1f); //マルチバトル用のアイコンを反映
        }
        canStartBattle = true;
    }

    public void TimeSlower(float slowSpeed, float timeLimit)
    {
        Time.timeScale = slowSpeed;
        canStartToCount = true;
        this.timeLimit = timeLimit;
        Debug.Log("タイムスロー中");
    }

    public void ReloadScene()
    {
        //トランジションアニメーション再生
        UIController.Instance.SetAnimParam(UIController.UIGroup.Menu, 2, "DoFrameIn");

        StartCoroutine(DelayMethod(2f, () =>
        {
            // 現在のScene名を取得する
            Scene loadScene = SceneManager.GetActiveScene();
            // Sceneの読み直し
            SceneManager.LoadScene(loadScene.name);
        }));

        canLoadScene = false;
    }

    public void RestartGame()
    {
        Admob.Instance.DestroyBanner(); //バナー削除

        //トランジションアニメーション再生
        UIController.Instance.SetAnimParam(UIController.UIGroup.Menu, 2, "DoFrameIn");
        gameDataStorageManager.ResetBattleStatus();

        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Battle_Single:

                gameDataStorageManager.CheckShowAd(); //3回に一度インタースティシャル広告を表示する

                break;

            case GameStateManager.GameState.Battle_Multi:

                break;

            default:
                Debug.Log("そんなステートでは使わないメソッドです。");
                break;
        }

        StartCoroutine(DelayMethod(2f, () =>
        {
            // 現在のScene名を取得する
            Scene loadScene = SceneManager.GetActiveScene();
            // Sceneの読み直し
            SceneManager.LoadScene(loadScene.name);
        }));
    }

    public void BackToTitle()
    {

        Admob.Instance.DestroyBanner(); //バナー削除

        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 4, true); //FadeOutを表示
        gameDataStorageManager.ResetBattleStatus(); //ステータスリセット

        StartCoroutine(DelayMethod(1.5f, () =>
        {
            //TitleSceneをロード
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadScene("TitleScene");
        }));

    }

    //シーンを読み込み完了時に呼び出される
    void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        GameStateManager.Instance.ChangeState(GameStateManager.GameState.Title); //ステートをタイトルに切り替え
    }


    public void ShowRoulettePanel()
    {
        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Battle_Single:
                if (isOpened)
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 5, false); //RoulettePanelを非表示
                }
                else
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 6, false); //RewardButtonを非表示
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 7, false); //RewardButtonを非表示
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 5, true); //RoulettePanelを表示
                    isOpened = true;
                }
                break;

            case GameStateManager.GameState.Battle_Multi:
                if (isOpened)
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 5, false); //RoulettePanelを非表示
                }
                else
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 6, false); //RewardButtonを非表示
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 5, true); //RoulettePanelを表示
                    isOpened = true;
                }
                break;

            default:
                Debug.Log("そのステートでは使わないメソッドです");
                break;
        }



    }

    //＝＝＝＝＝＝＝＝＝＝＝シングルプレイ用＝＝＝＝＝＝＝＝＝＝＝＝

    public void IncreaseSingleBattleCount(int charaNum)
    {
        //ラウンド数加算
        roundCount++;

        switch (charaNum)
        {

            case 0: //プレイヤー側

                character1WinCount++; //勝数加算

                //プレイヤーの勝ち
                if (character1WinCount > 1)
                {

                    UIController.Instance.UISetActivater(UIController.UIGroup.Character1, 4, true); //二個目の勝ち星アニメーション再生
                    UIController.Instance.SetResultText("You Win!",true); //リザルトテキスト設定
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 3, true); //リザルト画面を表示
                    UIController.Instance.SetParticleSystem(true); //紙吹雪再生

                    consecWinManager.SetWinText(true); //連勝数テキストをセット

                    //2回に一度リワードアドボタンを出して、4回に一度ルーレットボタンが出る
                    if (gameDataStorageManager.GetBattleCount % 4 == 0 && gameDataStorageManager.GetBattleCount % 2 == 0)
                    {
                        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 6, true); //RewardAdボタン表示
                    }
                    else
                    {
                        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 7, true); //Rouletteボタン表示
                    }


                    StartCoroutine(DelayMethod(1.1f, () => //Resultパネルのアニメーションが終わるまで待機
                    {
                        gameDataStorageManager.BestConsecutiveWinsProperty += 1; //連勝数プラス1
                        consecWinManager.StartCoroutine("TextAnimation", true);　//連勝数アニメーション再生
                        DesideRewardPrice(); //お金追加
                    }));
                }
                else
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Character1, 3, true); //一個目の勝ち星アニメーション再生
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 0, true); //TapToContinueを表示
                    SaveRoundStatus(); //バトル情報保存
                    canLoadScene = true; //次のシーンロードを可能にする
                }

                break;

            case 1:　//敵側

                character2WinCount++; //勝数加算
                
                //敵の勝ち
                if (character2WinCount > 1)
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Character2, 3, true); //二個目の勝ち星アニメーション再生
                    UIController.Instance.SetResultText("You Lose",false);　//リザルトテキスト設定
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 3, true); //リザルト画面を表示

                    consecWinManager.SetWinText(false); //連勝数テキストをセット

                    //2回に一度リワードアドボタンを出して、4回に一度ルーレットボタンが出る
                    if (gameDataStorageManager.GetBattleCount % 4 == 0 && gameDataStorageManager.GetBattleCount % 2 == 0)
                    {
                        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 6, true); //RewardAdボタン表示
                    }
                    else
                    {
                        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 7, true); //Rouletteボタン表示
                    }

                    StartCoroutine(DelayMethod(1.1f, () => //Resultパネルのアニメーションが終わるまで待機
                    {
                        gameDataStorageManager.BestConsecutiveWinsProperty = 0; //連勝数リセット
                        consecWinManager.StartCoroutine("TextAnimation", false);　//連勝数アニメーション再生
                        DesideRewardPrice(); //お金追加
                    }));
                }
                else
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Character2, 2, true); //一個目の勝ち星アニメーション再生
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 0, true); //TapToContinueを表示
                    SaveRoundStatus(); //バトル情報保存
                    canLoadScene = true; //次のシーンロードを可能にする
                }

                break;

            default:
                Debug.Log("そんなキャラは勝ってない");
                break;
        }
    }


    //＝＝＝＝＝＝＝＝＝＝＝マルチプレイ用＝＝＝＝＝＝＝＝＝＝＝＝

    public void IncreaseMultiBattleCount(int charaNum)
    {
        //ラウンド数加算
        roundCount++;

        switch (charaNum)
        {

            case 0: //左プレイヤー側

                character1WinCount++; //勝数加算

                //プレイヤーの勝ち
                if (character1WinCount > 1)
                {

                    UIController.Instance.UISetActivater(UIController.UIGroup.Character1, 4, true); //二個目の勝ち星アニメーション再生
                    UIController.Instance.SetResultText("Left Player Win!", true); //リザルトテキスト設定
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 3, true); //リザルト画面を表示
                    UIController.Instance.SetParticleSystem(true); //紙吹雪再生

                    //3回に一度ルーレットボタンが出る
                    if (gameDataStorageManager.GetBattleCount % 3 == 0)
                    {
                        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 6, true); //Rouletteボタン表示
                    }

                    StartCoroutine(DelayMethod(1.1f, () => //Resultパネルのアニメーションが終わるまで待機
                    {
                        moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + multiPrizePrice); //お金追加
                    }));
                }
                else
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Character1, 3, true); //一個目の勝ち星アニメーション再生
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 0, true); //TapToContinueを表示
                    SaveRoundStatus(); //バトル情報保存
                    canLoadScene = true; //次のシーンロードを可能にする
                }

                break;

            case 1:　//右プレイヤー側

                character2WinCount++; //勝数加算

                //敵の勝ち
                if (character2WinCount > 1)
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Character2, 4, true); //二個目の勝ち星アニメーション再生
                    UIController.Instance.SetResultText("Right Player Win!", false);　//リザルトテキスト設定
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 3, true); //リザルト画面を表示
                    UIController.Instance.SetParticleSystem(true); //紙吹雪再生

                    //3回に一度ルーレットボタンが出る
                    if (gameDataStorageManager.GetBattleCount % 3 == 0)
                    {
                        UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 6, true); //Rouletteボタン表示
                    }

                    StartCoroutine(DelayMethod(1.1f, () => //Resultパネルのアニメーションが終わるまで待機
                    {
                        moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + multiPrizePrice); //お金追加
                    }));
                }
                else
                {
                    UIController.Instance.UISetActivater(UIController.UIGroup.Character2, 3, true); //一個目の勝ち星アニメーション再生
                    UIController.Instance.UISetActivater(UIController.UIGroup.Menu, 0, true); //TapToContinueを表示
                    SaveRoundStatus(); //バトル情報保存
                    canLoadScene = true; //次のシーンロードを可能にする
                }

                break;

            default:
                Debug.Log("そんなキャラは勝ってない");
                break;
        }
    }



    private void DesideRewardPrice()
    {
        switch (character1WinCount)
        {
            //連勝数によって金額変化

            case 0: //プレイヤーストレート負け (基本4円追加) 
                Debug.Log((int)(4 * consecWinManager.MoneyMagnification()));
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + Mathf.FloorToInt(4 * consecWinManager.MoneyMagnification()));
                break;

            case 1:　//プレイヤー1ラウンド取ったけど負け　（基本8円追加）
                Debug.Log((int)(8 * consecWinManager.MoneyMagnification()));
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + Mathf.FloorToInt(8 * consecWinManager.MoneyMagnification()));
                break;

            case 2:　//プレイヤーのストレート勝ち　（基本10円追加）
                Debug.Log(Mathf.FloorToInt(10 * consecWinManager.MoneyMagnification()));
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + Mathf.FloorToInt(10 * consecWinManager.MoneyMagnification()));
                break;

            default:
                Debug.Log("そんな勝利数はない");
                break;
        }
    }

    private void SyncRoundStatus()
    {
        Debug.Log("ラウンド情報同期");
        roundCount = gameDataStorageManager.RoundCount;
        character2WinCount = gameDataStorageManager.Character2WinCount;
        character1WinCount = gameDataStorageManager.Character1WinCount;

        UIController.Instance.SetRoundUI(roundCount, character1WinCount, character2WinCount); //UIの情報を更新
    }

    private void SaveRoundStatus()
    {
        Debug.Log("ラウンド情報保存");
        gameDataStorageManager.Character2WinCount = character2WinCount;
        gameDataStorageManager.Character1WinCount = character1WinCount;
        gameDataStorageManager.RoundCount = roundCount;
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
