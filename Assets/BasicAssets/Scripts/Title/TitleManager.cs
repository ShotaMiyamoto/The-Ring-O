using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TitleManager : MonoBehaviour
{

    public Animator titleAnimator;
    public Animator appleCharaAnimator;

    public Animator leftButtonAnimator;
    public Animator rightButtonAnimator;
    public Animator middleButtonAnimator;
    public GameObject transitionImage;

    private bool canPush = true;

    //==================左側関係======================
    [SerializeField] private ShopManager shopManager = default; 

    /// <summary>
    /// カメラ方向定義
    /// </summary>
    public enum CameraDir
    {
        Forward,
        Left,
        Back,
        Right
    }

    /// <summary>
    /// 現在向いているカメラ方向
    /// </summary>
    private CameraDir currentCamDir = CameraDir.Forward;

    public CameraDir GetCurrentCamDir { get { return currentCamDir; } }

    //==================システムメニュー関係======================
    private bool isSystemMenuOpened = false;
    [SerializeField] private GameObject systemPanel = default;
    


    // Start is called before the first frame update
    void Start()
    {
        transitionImage.SetActive(false);
    }


    public void PushedRightButton()
    {
        if (canPush)
        {
            switch (currentCamDir)
            {
                //正面　→　右
                case CameraDir.Forward:　
                    TurnToRight();

                    titleAnimator.SetBool("canFrameOut", true); //タイトルをフレームアウト

                    currentCamDir = CameraDir.Right;
                    break;

                //右　→　後ろ
                case CameraDir.Right:
                    TurnToBack();
                    currentCamDir = CameraDir.Back;
                    break;

                //後ろ　→　左
                case CameraDir.Back:
                    TurnToLeft();
                    currentCamDir = CameraDir.Left;
                    break;

                //左　→　正面
                case CameraDir.Left:
                    TurnToForward();

                    titleAnimator.SetBool("canFrameOut", false); //タイトルを表示
                    currentCamDir = CameraDir.Forward;
                    break;

                default:
                    Debug.Log("そんな方向はない");
                    break;
            }

            rightButtonAnimator.SetTrigger("isPushed");
            leftButtonAnimator.SetTrigger("isPushed");
            middleButtonAnimator.SetTrigger("isPushed");
            canPush = false;

            StartCoroutine(DelayMethod(1f, () =>
            {
                canPush = true;
            }));
        }
    }


    public void PushedLeftButton(){
        if (canPush)
        {
            switch (currentCamDir)
            {
                //正面　→　左
                case CameraDir.Forward:
                    TurnToLeft();

                    titleAnimator.SetBool("canFrameOut", true); //タイトルをフレームアウト

                    currentCamDir = CameraDir.Left;
                    break;

                //左　→　後ろ
                case CameraDir.Left:
                    TurnToBack();
                    currentCamDir = CameraDir.Back;

                    break;

                //後ろ　→　右
                case CameraDir.Back:
                    TurnToRight();
                    currentCamDir = CameraDir.Right;
                    break;

                //右　→　正面
                case CameraDir.Right:
                    TurnToForward();

                    titleAnimator.SetBool("canFrameOut", false); //タイトルを再表示

                    currentCamDir = CameraDir.Forward;
                    break;

                default:
                    Debug.Log("そんな方向はない");
                    break;
            }

            rightButtonAnimator.SetTrigger("isPushed");
            leftButtonAnimator.SetTrigger("isPushed");
            middleButtonAnimator.SetTrigger("isPushed");
            canPush = false;

            StartCoroutine(DelayMethod(1f, () =>
            {
                canPush = true;
            }));
        }
    }


    public void PushedMiddleButton()
    {
        switch (currentCamDir)
        {
            case CameraDir.Forward:
                Admob.Instance.DestroyBanner(); //バナー削除
                GoToSingleBattleScene();
                break;


            case CameraDir.Left:
                HideMainButtons(true);
                shopManager.ShowShopMenu(true);

                break;


            case CameraDir.Back:
                break;


            case CameraDir.Right:
                Admob.Instance.DestroyBanner(); //バナー削除

                GoToMultiBattleScene();
                break;


            default:
                Debug.Log("そんなMiddleButtonは無い");
                break;

        }
    }


    private void TurnToForward()
    {
        CameraManager.Instance.ChangeCamera(1);

        StartCoroutine(DelayMethod(0.5f, () =>
        {
            
        }));
        Debug.Log("中央に移動");

    }

    private void TurnToLeft()
    {
        CameraManager.Instance.ChangeCamera(2);

        Debug.Log("左側に移動");
    }

    private void TurnToBack()
    {
        CameraManager.Instance.ChangeCamera(3);

        Debug.Log("後ろ側に移動");
    }

    private void TurnToRight()
    {
        CameraManager.Instance.ChangeCamera(4);

        Debug.Log("右側に移動");
    }


    public void GoToSingleBattleScene()
    {
        if (canPush)
        {
            Debug.Log("シングルバトルシーンへ");
            CameraManager.Instance.ChangeCamera(0);
            titleAnimator.SetBool("canFrameOut",true); //タイトルをフレームアウト
            appleCharaAnimator.SetTrigger("StartGame"); //立ち上がるモーション

            HideMainButtons(true); //すべてのメインボタンを隠す

            StartCoroutine( DelayMethod( 3f,() => 
            {
                transitionImage.SetActive(true);

                StartCoroutine( DelayMethod( 1f,() => 
                {
                    // Sceneロード
                    SceneManager.sceneLoaded += SceneLoaded;
                    SceneManager.LoadScene("BattleScene_Single");
                }));

            }));

            canPush = false;
        }
    }

    public void GoToMultiBattleScene()
    {
        if (canPush)
        {
            Debug.Log("マルチバトルシーンへ");
            CameraManager.Instance.ChangeCamera(7);
            titleAnimator.SetBool("canFrameOut", true); //タイトルをフレームアウト

            HideMainButtons(true); //すべてのメインボタンを隠す

            StartCoroutine(DelayMethod(3f, () =>
            {
                transitionImage.SetActive(true);

                StartCoroutine(DelayMethod(1f, () =>
                {
                    // Sceneロード
                    SceneManager.sceneLoaded += SceneLoaded;
                    SceneManager.LoadScene("BattleScene_Multi");
                }));

            }));

            canPush = false;
        }
    }

    // シーンを読み込み完了時に呼び出される
    void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        switch (currentCamDir)
        {
            case CameraDir.Forward:
                GameStateManager.Instance.ChangeState(GameStateManager.GameState.Battle_Single);
                break;


            case CameraDir.Left:
                //Nothing
                break;


            case CameraDir.Back:
                //ToDo
                break;


            case CameraDir.Right:
                GameStateManager.Instance.ChangeState(GameStateManager.GameState.Battle_Multi);
                break;


            default:
                Debug.Log("そんなMiddleButtonは無い");
                break;
        }
    }

    public void HideMainButtons(bool value)
    {
        middleButtonAnimator.SetBool("canHide", value);
        rightButtonAnimator.SetBool("canHide", value);
        leftButtonAnimator.SetBool("canHide", value);

        if (value)　//trueだった場合表示する
        {
            middleButtonAnimator.SetTrigger("isPushed");
            rightButtonAnimator.SetTrigger("isPushed");
            leftButtonAnimator.SetTrigger("isPushed");
        }
    }

    public void BackToLeftMenu()
    {
        CameraManager.Instance.ChangeCamera(2); //Leftメニュー用カメラに移動
        shopManager.ShowShopMenu(false);

        StartCoroutine(DelayMethod(1f, () =>
        {
            HideMainButtons(false);
        }));
    }

    public void ActivateSystemPanel()
    {
        if (!isSystemMenuOpened)
        {
            systemPanel.SetActive(true);
            isSystemMenuOpened = true;
        }
        else
        {
            systemPanel.SetActive(false);
            isSystemMenuOpened = false;
        }
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://gist.github.com/ShotaMiyamoto/402a1cf39e0b47a094aa9e23bdea47e9");
    }


    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
