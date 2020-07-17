using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameStateManager : SingletonMonoBehaviour<GameStateManager>
{

    private bool created = false;
    public enum GameState
    {
        Title,
        Battle_Single,
        Battle_Multi
    }
    
    private GameState currentState;

    public GameState StateProperty
    {
        get { return currentState; }
    }


    protected override void Awake()
    {
        Application.targetFrameRate = 60; // ターゲットフレームレートを60に設定

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


        //ステート切り替え
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "TitleScene":
                currentState = GameState.Title;
                break;

            case "BattleScene_Single":
                currentState = GameState.Battle_Single;
                break;

            case "BattleScene_Multi":
                currentState = GameState.Battle_Multi;
                break;
        }
    }



    public void ChangeState(GameState state)
    {
        currentState = state;

        switch (state)
        {
            case GameState.Title:

                Admob.Instance.DestroyBanner(); //バナーも表示されてれば削除

                Screen.orientation = ScreenOrientation.Portrait;
                Debug.Log("Titleステートへ変更完了");

                StartCoroutine(DelayMethod(1.5f, () => //Resultパネルのアニメーションが終わるまで待機
                {
                    Admob.Instance.ShowInterstitial(); //広告表示
                    Debug.Log("Titleへ戻ったので広告表示");
                }));

                break;

            case GameState.Battle_Single:

                Admob.Instance.DestroyBanner(); //バナー削除

                Screen.orientation = ScreenOrientation.Portrait;
                Debug.Log("Battle_Singleステートへ変更完了");
                break;

            case GameState.Battle_Multi:

                Admob.Instance.DestroyBanner(); //バナーも表示されてれば削除

                Screen.orientation = ScreenOrientation.LandscapeLeft;
                Debug.Log("Battle_Multiステートへ変更完了");
                break;

        }

    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
