using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : SingletonMonoBehaviour<UIController>
{
    [SerializeField] private GameObject[] character1UIs = default;
    [SerializeField] private GameObject[] character2UIs = default;
    [SerializeField] private GameObject[] menuUIs = default;
    [SerializeField] private Text roundText = default;
    [SerializeField] private Text resultText = default;
    [SerializeField] private ParticleSystem confetti = default;

    [SerializeField] private Color character1TextColor = new Color(255f/255f, 243f/255f, 73f/255f, 255f/255f); //黄色に設定;
    [SerializeField] private Color character2TextColor = new Color(71f / 255f, 100f / 255f, 255f / 255f, 255f / 255f);　//青色に設定;

    private GameDataStorageManager GDSM_Instance;

    public Image character1BodyColorIcon;
    public Image character1CostumeIcon;

    public Image character2BodyColorIcon;
    public Image character2CostumeIcon;

    public enum UIGroup
    {
        Character1,
        Character2,
        Menu
    }

    private void Start()
    {
        GDSM_Instance = GameDataStorageManager.Instance;

        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Battle_Single:
                Invoke("SyncBattleIcons", 0.1f);
                break;

            case GameStateManager.GameState.Battle_Multi:
                SetActivatePlayerJoysticks(true); //Joystick有効化
                Debug.Log("パネルで指示があるまで何もしない");
                break;

            default:
                Debug.Log("そんなステートではUIController使わない");
                break;
        }

    }

    public void SyncBattleIcons()
    {
        character1BodyColorIcon.sprite = GDSM_Instance.GetBColorIcon(GDSM_Instance.GetSelectedBodyColorNum);
        character1CostumeIcon.sprite = GDSM_Instance.GetCostumeIcon(GDSM_Instance.GetSelectedCostumeNum);
        character1CostumeIcon.SetNativeSize();

        character2BodyColorIcon.sprite = GDSM_Instance.GetBColorIcon(GDSM_Instance.GetEnemyBColorIconNum());
        character2CostumeIcon.sprite = GDSM_Instance.GetCostumeIcon(GDSM_Instance.GetEnemyCosIconNum());
        character2CostumeIcon.SetNativeSize();
    }

    public void SyncMultiBattleIcons()
    {
        character1BodyColorIcon.sprite = GDSM_Instance.GetBColorIcon(GDSM_Instance.SelecBColNumLeft);
        character1CostumeIcon.sprite = GDSM_Instance.GetCostumeIcon(GDSM_Instance.SelecCosNumLeft);
        character1CostumeIcon.SetNativeSize();

        character2BodyColorIcon.sprite = GDSM_Instance.GetBColorIcon(GDSM_Instance.SelecBColNumRight);
        character2CostumeIcon.sprite = GDSM_Instance.GetCostumeIcon(GDSM_Instance.SelecCosNumRight);
        character2CostumeIcon.SetNativeSize();
    }

    public void SetActivatePlayerJoysticks(bool value)
    {
        character1UIs[0].SetActive(value);
        character2UIs[0].SetActive(value);
    }

    public void SetResultText(string text, bool isWin)
    {
        resultText.text = text;

        if (isWin)
        {　//プレイヤーの勝ち
            resultText.color = character1TextColor;
        }
        else
        {　//敵の勝ち
            resultText.color = character2TextColor;　
        }
    }

    public void SetParticleSystem(bool value)
    {
        if (value)
        {
            confetti.Play();
        }
        else
        {
            confetti.Stop();
        }
    }

    //==================ラウンド系UIの表示変更======================

    public void SetRoundUI(int roundCount,int playerWinCount,int enemyWinCount)
    {
        //==================ラウンド処理======================

        switch (roundCount)
        {
            case 0:
                roundText.text = ("Round 1");
                break;

            case 1:
                roundText.text = ("Round 2");
                break;

            case 2:
                roundText.text = ("Final Round");
                break;

            default:
                Debug.Log("そんなラウンド数は無い");
                break;
        }

        //==================プレイヤー勝利数処理======================

        switch (playerWinCount)
        {
            case 0:
                //ラウンド1は勝数が無いので勝数のFillをすべて非表示にする
                character1UIs[1].SetActive(false); //1勝目のFillを非表示
                character1UIs[2].SetActive(false); //2勝目のFillを非表示
                break;

            case 1:
                //一勝していれば一個目のFillを表示
                character1UIs[1].SetActive(true); //1勝目のFillを表示
                break;

            case 2:
                //二勝していれば両方のFillを表示
                character1UIs[1].SetActive(true); //1勝目のFillを表示
                character1UIs[2].SetActive(true); //1勝目のFillを表示
                break;

            default:
                Debug.Log("そんなプレイヤーの勝ち数は無い");
                break;
        }

        //==================敵勝利数処理======================

        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Battle_Single:
                switch (enemyWinCount)
                {
                    case 0:
                        //ラウンド1は勝数が無いので勝数のFillをすべて非表示にする
                        character2UIs[0].SetActive(false); //1勝目のFillを非表示
                        character2UIs[1].SetActive(false); //2勝目のFillを非表示
                        break;

                    case 1:
                        //一勝していれば一個目のFillを表示
                        character2UIs[0].SetActive(true); //1勝目のFillを表示
                        break;

                    case 2:
                        //二勝していれば両方のFillを表示
                        character2UIs[0].SetActive(true); //1勝目のFillを表示
                        character2UIs[1].SetActive(true); //1勝目のFillを表示
                        break;

                    default:
                        Debug.Log("そんなプレイヤーの勝ち数は無い");
                        break;
                }
                break;

            case GameStateManager.GameState.Battle_Multi:
                switch (enemyWinCount)
                {
                    case 0:
                        //ラウンド1は勝数が無いので勝数のFillをすべて非表示にする
                        character2UIs[1].SetActive(false); //1勝目のFillを非表示
                        character2UIs[2].SetActive(false); //2勝目のFillを非表示
                        break;

                    case 1:
                        //一勝していれば一個目のFillを表示
                        character2UIs[1].SetActive(true); //1勝目のFillを表示
                        break;

                    case 2:
                        //二勝していれば両方のFillを表示
                        character2UIs[1].SetActive(true); //1勝目のFillを表示
                        character2UIs[2].SetActive(true); //1勝目のFillを表示
                        break;

                    default:
                        Debug.Log("そんなプレイヤーの勝ち数は無い");
                        break;
                }
                break;

        }

    }


    //==================UIアクティベート処理======================
    public void UISetActivater(UIGroup UIGroupName,int num,bool value)
    {
        switch (UIGroupName)
        {
            case UIGroup.Character1: //プレイヤーUI
                if (num >= character1UIs.Length)
                {
                    Debug.Log("そんなUI無い");
                }
                else
                {
                    character1UIs[num].SetActive(value);
                }
                break;

            case UIGroup.Character2: //エネミーUI
                if (num >= character2UIs.Length)
                {
                    Debug.Log("そんなUI無い");
                }
                else
                {
                    character2UIs[num].SetActive(value);
                }
                break;

            case UIGroup.Menu: //メニューUI
                if (num >= menuUIs.Length)
                {
                    Debug.Log("そんなUI無い");
                }
                else
                {
                    menuUIs[num].SetActive(value);
                }
                break;

            default:
                Debug.Log("そんなUI配列はない");
                break;
        }
    }


    //==================UIのアニメーションのパラメータ処理======================

    //int型
    public void SetAnimParam(UIGroup UIGroupName,int num, string paramName, int value)
    {
        Animator anim;

        switch (UIGroupName)
        {
            case UIGroup.Character1: //プレイヤーUI
                anim = character1UIs[num].GetComponent<Animator>();
                anim.SetInteger(paramName, value);
                break;

            case UIGroup.Character2: //エネミーUI
                anim = character2UIs[num].GetComponent<Animator>();
                anim.SetInteger(paramName, value);
                break;

            case UIGroup.Menu: //メニューUI
                anim = menuUIs[num].GetComponent<Animator>();
                anim.SetInteger(paramName, value);
                break;

            default:
                Debug.Log("そんなUI配列はない");
                break;
        }
    }

    //float型
    public void SetAnimParam(UIGroup UIGroupName, int num, string paramName, float value)
    {
        Animator anim;

        switch (UIGroupName)
        {
            case UIGroup.Character1: //プレイヤーUI
                anim = character1UIs[num].GetComponent<Animator>();
                anim.SetFloat(paramName, value);
                break;

            case UIGroup.Character2: //エネミーUI
                anim = character2UIs[num].GetComponent<Animator>();
                anim.SetFloat(paramName, value);
                break;

            case UIGroup.Menu: //メニューUI
                anim = menuUIs[num].GetComponent<Animator>();
                anim.SetFloat(paramName, value);
                break;

            default:
                Debug.Log("そんなUI配列はない");
                break;
        }
    }

    //bool型
    public void SetAnimParam(UIGroup UIGroupName, int num, string paramName, bool value)
    {
        Animator anim;

        switch (UIGroupName)
        {
            case UIGroup.Character1: //プレイヤーUI
                anim = character1UIs[num].GetComponent<Animator>();
                anim.SetBool(paramName, value);
                break;

            case UIGroup.Character2: //エネミーUI
                anim = character2UIs[num].GetComponent<Animator>();
                anim.SetBool(paramName, value);
                break;

            case UIGroup.Menu: //メニューUI
                anim = menuUIs[num].GetComponent<Animator>();
                anim.SetBool(paramName, value);
                break;

            default:
                Debug.Log("そんなUI配列はない");
                break;
        }
    }

    //Trigger型
    public void SetAnimParam(UIGroup UIGroupName, int num, string paramName)
    {
        Animator anim;

        switch (UIGroupName)
        {
            case UIGroup.Character1: //プレイヤーUI
                anim = character1UIs[num].GetComponent<Animator>();
                anim.SetTrigger(paramName);
                break;

            case UIGroup.Character2: //エネミーUI
                anim = character2UIs[num].GetComponent<Animator>();
                anim.SetTrigger(paramName);
                break;

            case UIGroup.Menu: //メニューUI
                anim = menuUIs[num].GetComponent<Animator>();
                anim.SetTrigger(paramName);
                break;

            default:
                Debug.Log("そんなUI配列はない");
                break;
        }
    }
}
