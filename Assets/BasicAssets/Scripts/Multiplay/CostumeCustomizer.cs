using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CostumeCustomizer : MonoBehaviour
{
    [SerializeField] private GameObject customizePanel = default;

    [SerializeField] private PlayerController leftPlayerController = default;
    [SerializeField] private PlayerController rightPlayerController = default;

    //左プレイヤーのAppear系番号

    [SerializeField] private int bodyColor_Left = 0;

    public int SetBodyColorLeft { set { bodyColor_Left = value; }  get { return bodyColor_Left; } }

    [SerializeField] private int costume_Left = 0;

    public int SetCostumeLeft { set { costume_Left = value; } get { return costume_Left; } }
    

    //右プレイヤーのAppear系番号

    [SerializeField] private int bodyColor_Right = 0;

    public int SetBodyColorRight { set { bodyColor_Right = value; }  get { return bodyColor_Right; } }


    [SerializeField] private int costume_Right = 0;

    public int SetCostumeRight { set { costume_Right = value; } get { return costume_Right; } }


    //ボタン類　持っているアイテムのものを有効化させる
    [SerializeField] private GameObject[] bColBtn_Left = default;
    [SerializeField] private GameObject[] cosBtn_Left = default;

    [SerializeField] private GameObject[] bColBtn_Right = default;
    [SerializeField] private GameObject[] cosBtn_Right = default;

    [SerializeField] private GameObject bColButtonGroup_Left = default; //左側のBodyColorボタン群
    [SerializeField] private GameObject cosButtonGroup_Left = default; //左側のCostumeボタン群
    [SerializeField] private GameObject bColButtonGroup_Right = default; //右側のBodyColorボタン群
    [SerializeField] private GameObject cosButtonGroup_Right = default; //右側のCostumeボタン群

    [SerializeField] private Image bColTabLeft = default;
    [SerializeField] private Image cosTabLeft = default;

    [SerializeField] private Image bColTabRight = default;
    [SerializeField] private Image cosTabRight = default;

    //所持アイテムの確認用
    GameDataStorageManager gdsm = default;

    //選択アイコン処理用
    [SerializeField] private GameObject selecBColIconLeft = default;
    [SerializeField] private GameObject selecCosIconLeft = default;

    [SerializeField] private GameObject selecBColIconRight = default;
    [SerializeField] private GameObject selecCosIconRight = default;

    private void Start()
    {
        gdsm = GameDataStorageManager.Instance;

        if(gdsm.RoundCount == 0)
        {
            SyncItemButton(); //ボタンを同期
            ShowBodyColorButtonsLeft(); //BodyColorボタンを表示（左側）
            ShowBodyColorButtonsRight(); //BodyColorボタンを表示（右側）
            customizePanel.SetActive(true);　//パネル表示
            Debug.Log("1ラウンド目なのでカスタムパネル表示");
        }
        else
        {
            customizePanel.SetActive(false); //パネル非表示

            StartCoroutine(DelayMethod(0.1f, () => //(各クラスがGDSMをインスタンス化するまで待ってから読み込む)
            {
                ChangePlayerAppear();
                UIController.Instance.SyncMultiBattleIcons();
            }));

            Debug.Log("2ラウンド目なので保存している外見を反映");
        }

    }

    public void ChangePlayerAppear()
    {
        //保存している情報を反映
        leftPlayerController.SetAppearManually(gdsm.SelecBColNumLeft, gdsm.SelecCosNumLeft);
        rightPlayerController.SetAppearManually(gdsm.SelecBColNumRight, gdsm.SelecCosNumRight);
    }


    public void StartBattle()
    {        
        //データを保存
        gdsm.SelecBColNumLeft = bodyColor_Left;
        gdsm.SelecCosNumLeft = costume_Left;

        gdsm.SelecBColNumRight = bodyColor_Right;
        gdsm.SelecCosNumRight = costume_Right;

        //プレイヤーの見た目反映
        ChangePlayerAppear();

        //UIを反映
        UIController.Instance.SyncMultiBattleIcons();

        //パネル無効化
        customizePanel.SetActive(false);

        //ゲームプレイ開始
        GameManager.Instance.StartBattle();
    }

    /// <summary>
    /// 所持しているアイテムのボタンを表示する
    /// </summary>
    public void SyncItemButton()
    {
        //持っているBodyColorのボタンを有効化
        foreach (var value in gdsm.GetUnlockedBodyColorList)
        {
            bColBtn_Left[value].SetActive(true);
            bColBtn_Right[value].SetActive(true);
        }

        //持っているCostumeのボタンを有効化
        foreach (var value in gdsm.GetUnlockedCostumeList)
        {
            cosBtn_Left[value].SetActive(true);
            cosBtn_Right[value].SetActive(true);
        }
    }

    //左側

    public void ShowBodyColorButtonsLeft()
    {
        cosTabLeft.color = new Color(0, 0, 0, 80f / 255f); //色を薄くする
        bColTabLeft.color = new Color(0, 0, 0, 190f / 255f); //色を濃くする

        cosButtonGroup_Left.SetActive(false);
        bColButtonGroup_Left.SetActive(true);
    }

    public void ShowCostumeButtonsLeft()
    {
        bColTabLeft.color = new Color(0, 0, 0, 80f / 255f); //色を薄くする
        cosTabLeft.color = new Color(0, 0, 0, 190f / 255f); //色を濃くする

        bColButtonGroup_Left.SetActive(false);
        cosButtonGroup_Left.SetActive(true);
    }

    //右側

    public void ShowBodyColorButtonsRight()
    {
        cosTabRight.color = new Color(0, 0, 0, 80f / 255f); //色を薄くする
        bColTabRight.color = new Color(0, 0, 0, 190f / 255f); //色を濃くする

        cosButtonGroup_Right.SetActive(false);
        bColButtonGroup_Right.SetActive(true);
    }

    public void ShowCostumeButtonsRight()
    {
        bColTabRight.color = new Color(0, 0, 0, 80f / 255f); //色を薄くする
        cosTabRight.color = new Color(0, 0, 0, 190f / 255f); //色を濃くする

        bColButtonGroup_Right.SetActive(false);
        cosButtonGroup_Right.SetActive(true);
    }


    public void ChangeSelectBColIconLeft(GameObject obj)
    {
        if(selecBColIconLeft != null)
        {
            selecBColIconLeft.SetActive(false);
        }
        selecBColIconLeft = obj;
    }

    public void ChangeSelectCosIconLeft(GameObject obj)
    {
        if (selecCosIconLeft != null)
        {
            selecCosIconLeft.SetActive(false);
        }
        selecCosIconLeft = obj;
    }

    public void ChangeSelectBColIconRight(GameObject obj)
    {
        if (selecBColIconRight != null)
        {
            selecBColIconRight.SetActive(false);
        }
        selecBColIconRight = obj;
    }

    public void ChangeSelectCosIconRight(GameObject obj)
    {
        if(selecCosIconRight != null)
        {
            selecCosIconRight.SetActive(false);
        }
        selecCosIconRight = obj;
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
