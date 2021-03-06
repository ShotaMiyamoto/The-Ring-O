﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private MoneyManager moneyManager = default;

    public GameObject shopUI;　//ShopUI
    public GameObject bodyColorButtons;　//BodyColorのボタンパネル
    public GameObject costumeButtons;　//Costumeのボタンパネル

    public Image bodyColorButtonImage; //BodyColor切り替えボタンのImage
    public Image costumeButtonImage;　//Costume切り替えボタンのImage

    public GameObject dirLight; //ディレクショナルライト
    public GameObject spotLight;　//ショップ内のスポットライト
    public Animator customCharaAnim; //カスタムキャラのAnimation

    //============================購入処理関連=============================
    public int[] bodyColorPrices; //BodyColorアイテムの値段
    public int[] costumePrices; //Costumeアイテムの値段

    public GameObject purchasePanel; //購入パネル

    public Text beforeMoneyAmountText; //購入前の所持金
    public Text afterMoneyAmoutText; //購入後の所持金
    
    public Image purchaseItemImage; //購入するアイテム
    public Sprite[] costumeSprites; //Costumeイメージ
    public Sprite[] bodyColorSprites; //BodyColorイメージ

    public Text[] bodyColorPriceTexts; //BodyColorButtonのText
    public Text[] CostumePriceTexts; //CostumeButtonのText

    //============================カスタマイズ処理関連=============================
    [SerializeField] private SkinnedMeshRenderer bodyMesh = default;

    private int currentCostumeNum = 0;
    private GameDataStorageManager GDSM_Instance;
    public GameObject[] cosHead;
    public GameObject[] cosBody;

    public enum Item
    {
        BodyColor,
        Costume
    }

    private Item choosingItemPanel = Item.BodyColor;
    private int selectingItemNum = 0;

    //============================SecretItem用処理関連=============================
    public bool isOpenSecretItemPanel = false;
    public GameObject secretBodyColorButtons;
    public GameObject secretCostumeButtons;
    public GameObject nextPageButton; //右矢印
    public GameObject backPageButton; //左矢印
    public GameObject[] lockedCostumeIcons; //コスチュームの未開放アイコン
    public GameObject[] lockedBodyColorIcons; //BodyColorの未開放アイコン

    private void Start()
    {
        GDSM_Instance = GameDataStorageManager.Instance;
        ShowShopMenu(false);
        SyncAllShopInfo();
    }

    //ショップ情報を最新に更新する
    public void SyncAllShopInfo()
    {
        SyncAppear();
        SyncPriceTexts();
        SyncLockedIcon();
    }

    public void ShowShopMenu(bool value)
    {
        if (value == true) 
        {
            SyncAllShopInfo(); //ショップ情報更新

            ShowBodyColorButtons(); //BodyColor用のカメラとメニュー表示にする

            StartCoroutine(DelayMethod(1f, () =>
            {
                shopUI.SetActive(true); //メニュー画面を表示

                dirLight.SetActive(false); //directional Light を消してSpotLightをつける
                spotLight.SetActive(true);

                customCharaAnim.SetBool("canMove", true);
                int moveNum = UnityEngine.Random.Range(0, 6);
                customCharaAnim.SetInteger("Move_int", moveNum);
            }));
        }
        else
        {
            if (purchasePanel.activeInHierarchy == true)
            {
                purchasePanel.GetComponent<Animator>().SetBool("canAppear", false); //購入画面を非表示にする
            }

            isOpenSecretItemPanel = false; //デフォルトアイテム画面状態にする
            shopUI.SetActive(false); //メニュー画面を非表示

            cosBody[currentCostumeNum].SetActive(false); //メニューを閉じるときは頭コスチュームの姿にする
            cosHead[currentCostumeNum].SetActive(true);

            dirLight.SetActive(true); //directional Light を付けてSpotLightを消す
            spotLight.SetActive(false);

            customCharaAnim.SetBool("canMove", false); //アイドルに戻す
            GDSM_Instance.DataSave(); //データのセーブ
        }

    }

    //BodyColorタブButton
    public void ShowBodyColorButtons()
    {
        CameraManager.Instance.ChangeCamera(5);

        cosHead[currentCostumeNum].SetActive(true); //頭コスチュームを表示
        if(cosBody[currentCostumeNum].activeInHierarchy == true)
        {
            cosBody[currentCostumeNum].SetActive(false); //体コスチュームが表示されていたら非表示に変える
        }

        bodyColorButtonImage.color = new Color(0, 0, 0, 190f/255f);
        costumeButtonImage.color = new Color(0, 0, 0, 80f/255f);
        
        isOpenSecretItemPanel = false;
        secretCostumeButtons.SetActive(false);
        costumeButtons.SetActive(false);

        //右矢印ボタンを表示
        nextPageButton.SetActive(true);
        backPageButton.SetActive(false);

        bodyColorButtons.SetActive(true);

        if(purchasePanel.activeInHierarchy == true)
        {
            purchasePanel.GetComponent<Animator>().SetBool("canAppear", false);
        }
        choosingItemPanel = Item.BodyColor;　//購入するアイテムタグを設定
    }


    //CostumeタブButton
    public void ShowCostumeButtons()
    {
        CameraManager.Instance.ChangeCamera(6);
        cosBody[currentCostumeNum].SetActive(true); //体コスチュームを表示
        if(cosHead[currentCostumeNum].activeInHierarchy == true)
        {
            cosHead[currentCostumeNum].SetActive(false); //頭コスチュームが表示されていたら非表示に変える
        }

        costumeButtonImage.color = new Color(0, 0, 0, 190f/255f);
        bodyColorButtonImage.color = new Color(0, 0, 0, 80f/255f);

        isOpenSecretItemPanel = false;
        bodyColorButtons.SetActive(false);
        secretBodyColorButtons.SetActive(false);


        //右矢印ボタンを表示
        nextPageButton.SetActive(true);
        backPageButton.SetActive(false);

        costumeButtons.SetActive(true);

        if (purchasePanel.activeInHierarchy == true)
        {
            purchasePanel.GetComponent<Animator>().SetBool("canAppear", false);
        }
        choosingItemPanel = Item.Costume;　//購入するアイテムタグを設定
    }


    //ItemButtonsに設定
    public void CheckCanPurchase(int itemNum)
    {
        switch (choosingItemPanel)
        {
            case Item.BodyColor:

                if (itemNum <= 11) //11番以前（コモンアイテム）の場合
                {
                    //所持してるか確認。
                    if (!GDSM_Instance.CheckBodyColorUnlocked(itemNum))
                    {
                        //所持金が足りているか確認。
                        if (moneyManager.CheckMoneyAmount(bodyColorPrices[itemNum]))
                        {
                            ActivePurchasePanel(itemNum, moneyManager.GetCurrentMoney, (moneyManager.GetCurrentMoney - bodyColorPrices[itemNum]));
                            Debug.Log("購入可能。パネル表示");
                        }
                        else
                        {
                            moneyManager.NotEnoughtMoneyAnimation();
                            Debug.Log("購入不可。アニメーション再生");
                        }
                    }
                    else
                    {
                        Debug.Log("すでに持ってるアイテムだよ");
                        ChangeBodyColor(itemNum);
                        GDSM_Instance.ChangeSelectedBodyColor(itemNum); //選択アイテムを変更
                    }

                }
                else //8番以上（レアアイテム）の場合
                {
                    //所持してるか確認。
                    if (!GDSM_Instance.CheckBodyColorUnlocked(itemNum))
                    {
                        //未所持アニメーション再生
                        Debug.Log("持っていないレアアイテムだよ");
                    }
                    else
                    {
                        Debug.Log("すでに持ってるアイテムだよ");
                        ChangeBodyColor(itemNum);
                        GDSM_Instance.ChangeSelectedBodyColor(itemNum); //選択アイテムを変更
                    }
                }

                break;


            case Item.Costume:

                if (itemNum <= 7) //7番以前（コモンアイテム）の場合
                {
                    //所持してるか確認。
                    if (!GDSM_Instance.CheckCostumeUnlocked(itemNum))
                    {
                        //所持金が足りているか確認
                        if (moneyManager.CheckMoneyAmount(costumePrices[itemNum]))
                        {
                            Debug.Log("購入可能。パネル表示");
                            ActivePurchasePanel(itemNum, moneyManager.GetCurrentMoney, (moneyManager.GetCurrentMoney - costumePrices[itemNum]));
                        }
                        else
                        {
                            moneyManager.NotEnoughtMoneyAnimation();
                            Debug.Log("購入不可。アニメーション再生");
                        }
                    }
                    else
                    {
                        Debug.Log("すでに持ってるアイテムだよ");
                        ChangeCostume(itemNum);
                        GDSM_Instance.ChangeSelectedCostume(itemNum); //選択アイテムを変更
                    }
                }
                else //8番以上（レアアイテム）の場合
                {
                    //所持してるか確認。
                    if (!GDSM_Instance.CheckCostumeUnlocked(itemNum))
                    {
                        //未所持アニメーション再生
                        Debug.Log("持っていないレアアイテムだよ");
                    }
                    else
                    {
                        Debug.Log("すでに持ってるアイテムだよ");
                        ChangeCostume(itemNum);
                        GDSM_Instance.ChangeSelectedCostume(itemNum); //選択アイテムを変更
                    }
                }
                break;

            default:
                Debug.Log("そんなアイテムジャンルはない");
                break;
        }
    }

    public void SwitchToSecretItemPanel()
    {
        switch (choosingItemPanel)
        {
            case Item.BodyColor:

                if (isOpenSecretItemPanel)
                {
                    //通常BodyColorボタンを表示
                    secretBodyColorButtons.SetActive(false);
                    bodyColorButtons.SetActive(true);

                    //右矢印ボタンを表示
                    nextPageButton.SetActive(true);
                    backPageButton.SetActive(false);

                    isOpenSecretItemPanel = false;
                }
                else
                {
                    //シークレットBodyColorボタンを表示
                    bodyColorButtons.SetActive(false);
                    secretBodyColorButtons.SetActive(true);

                    //左矢印ボタンを表示
                    backPageButton.SetActive(true);
                    nextPageButton.SetActive(false);

                    isOpenSecretItemPanel = true;
                }

                break;

            case Item.Costume:

                if (isOpenSecretItemPanel)
                {
                    //通常Costumeボタンを表示
                    secretCostumeButtons.SetActive(false);
                    costumeButtons.SetActive(true);

                    //右矢印ボタンを表示
                    nextPageButton.SetActive(true);
                    backPageButton.SetActive(false);

                    isOpenSecretItemPanel = false;
                }
                else
                {
                    //シークレットCostumeボタンを表示
                    costumeButtons.SetActive(false);
                    secretCostumeButtons.SetActive(true);

                    //左矢印ボタンを表示
                    backPageButton.SetActive(true);
                    nextPageButton.SetActive(false);

                    isOpenSecretItemPanel = true;
                }

                break;

            default:
                Debug.Log("そんなアイテムジャンルはない");
                break;
        }
    }


    public void ActivePurchasePanel(int itemNum, int beforeAmount, int afterAmount)
    {
        switch (choosingItemPanel)
        {
            case Item.BodyColor:
                costumeButtons.SetActive(false); //Costumeボタンを非表示
                bodyColorButtons.SetActive(false); //BodyColorボタンを非表示
                beforeMoneyAmountText.text = beforeAmount.ToString(); //所持金を代入
                afterMoneyAmoutText.text = afterAmount.ToString(); //購入後の所持金を代入
                purchaseItemImage.sprite = bodyColorSprites[itemNum]; //購入するアイテム画像を代入
                selectingItemNum = itemNum; //選択したアイテム番号を登録

                choosingItemPanel = Item.BodyColor; //購入するアイテムタグを設定

                purchasePanel.GetComponent<Animator>().SetBool("canAppear", true);//表示アニメーション再生
                break;

            case Item.Costume:
                costumeButtons.SetActive(false); //Costumeボタンを非表示
                bodyColorButtons.SetActive(false); //BodyColorボタンを非表示
                beforeMoneyAmountText.text = beforeAmount.ToString(); //所持金を代入
                afterMoneyAmoutText.text = afterAmount.ToString(); //購入後の所持金を代入
                purchaseItemImage.sprite = costumeSprites[itemNum]; //購入するアイテム画像を代入
                selectingItemNum = itemNum; //選択したアイテム番号を登録

                choosingItemPanel = Item.Costume; //購入するアイテムタグを設定

                purchasePanel.GetComponent<Animator>().SetBool("canAppear", true);　//表示アニメーション再生
                break;

            default:
                Debug.Log("そんなアイテムパネルは無い");
                break;
        }
    }


    public void DeactivetePurchasePanel()
    {
        switch (choosingItemPanel)
        {
            case Item.BodyColor:
                ShowBodyColorButtons();
                purchasePanel.GetComponent<Animator>().SetBool("canAppear", false);
                break;

            case Item.Costume:
                ShowCostumeButtons();
                purchasePanel.GetComponent<Animator>().SetBool("canAppear", false);
                break;

            default:
                Debug.Log("そんなアイテムタブはない");
                purchasePanel.GetComponent<Animator>().SetBool("canAppear", false);
                break;
        }
    }

    public void PurchaseItem()
    {
        switch (choosingItemPanel)
        {
            case Item.BodyColor:
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney - bodyColorPrices[selectingItemNum]); //所持金の変更とセーブ
                GDSM_Instance.UnlockBodyColor(selectingItemNum); //アイテムの開放とセーブ
                GDSM_Instance.ChangeSelectedBodyColor(selectingItemNum); //選択アイテムを変更とセーブ
                ChangeBodyColor(selectingItemNum); //アイテムの適用
                ChangePriceText(selectingItemNum); //値段テキストの更新
                purchasePanel.GetComponent<Animator>().SetBool("canAppear", false); //パネル非表示
                ShowBodyColorButtons();　//ボタンの表示
                break;

            case Item.Costume:
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney - costumePrices[selectingItemNum]); //所持金の変更とセーブ
                GDSM_Instance.UnlockCostume(selectingItemNum); //アイテムの開放とセーブ
                GDSM_Instance.ChangeSelectedCostume(selectingItemNum); //選択アイテムを変更とセーブ
                ChangeCostume(selectingItemNum);　//アイテムの適用
                ChangePriceText(selectingItemNum); //値段テキストの更新
                purchasePanel.GetComponent<Animator>().SetBool("canAppear", false); //パネル非表示
                ShowCostumeButtons(); //ボタンの表示
                break;

            default:
                Debug.Log("そんなアイテムは買えない");
                break;
        }
    }

    /// <summary>
    /// すべてのPriceを更新確認する
    /// </summary>
    private void SyncPriceTexts()
    {
        //BodyColorの値段更新
        List<int> list = GDSM_Instance.GetUnlockedBodyColorList;
        choosingItemPanel = Item.BodyColor;

        foreach (var value in list)
        {
            if (value < 12) 
            {
                ChangePriceText(value);
                Debug.Log("BodyColor：" + value + "番更新");
            }
        }

        //Costumeの値段更新
        list = GDSM_Instance.GetUnlockedCostumeList;
        choosingItemPanel = Item.Costume;

        foreach (var value in list)
        {
            if(value < 8)
            {
                ChangePriceText(value);
                Debug.Log("Costume：" + value + "番更新");
            }
        }
    }


    //アンロックしているアイテムの値段テキストを更新する
    private void ChangePriceText(int itemNum)
    {
        switch (choosingItemPanel)
        {
            case Item.BodyColor:
                bodyColorPriceTexts[itemNum].text = "----";
                break;

            case Item.Costume:
                CostumePriceTexts[itemNum].text = "----";
                break;

            default:
                Debug.Log("そんな値段テキストは無い");
                break;
        }
    }


    /// <summary>
    /// ショップ内のレアアイテムのロックアイコン情報を最新状態に更新する
    /// </summary>
    public void SyncLockedIcon()
    {
        List<int> cosList = GDSM_Instance.GetUnlockedCostumeList.FindAll(n => n >= 8); //8番以上の番号を検索して代入
        if(cosList != null)
        {
            foreach(var i in cosList)
            {
                lockedCostumeIcons[i - 8].SetActive(false);
                Debug.Log((i - 8) + "番のロックアイコン非表示");
            }
        }

        List<int> bdcolList = GDSM_Instance.GetUnlockedBodyColorList.FindAll(n => n >= 12); //12番以上の番号を検索して代入
        if(bdcolList != null)
        {
            foreach(var i in bdcolList)
            {
                lockedBodyColorIcons[i - 12].SetActive(false);
                Debug.Log((i - 12) + "番のロックアイコンを非表示");
            }
        }
    }

    public void ChangeBodyColor(int num)
    {

        if (num >= GDSM_Instance.GetBColorMatLength)
        {
            num = 0;
        }

        bodyMesh.material = GDSM_Instance.GetMatInfo(num);
    }

    public void ChangeCostume(int num)
    {

        if (num >= cosBody.Length)
        {
           num = 0;
        }

        cosBody[currentCostumeNum].SetActive(false);
        cosBody[num].SetActive(true);

        currentCostumeNum = num;
    }

    public void SyncAppear()
    {
        bodyMesh.material = GDSM_Instance.GetMatInfo(GDSM_Instance.GetSelectedBodyColorNum);
        cosHead[GDSM_Instance.GetSelectedCostumeNum].SetActive(true);

        Debug.Log(cosHead[GDSM_Instance.GetSelectedCostumeNum].name + "有効化");

        if(cosBody[GDSM_Instance.GetSelectedCostumeNum].activeInHierarchy == true)
        {
            cosBody[GDSM_Instance.GetSelectedCostumeNum].SetActive(false);
        }
        currentCostumeNum = GDSM_Instance.GetSelectedCostumeNum;
    }



    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
