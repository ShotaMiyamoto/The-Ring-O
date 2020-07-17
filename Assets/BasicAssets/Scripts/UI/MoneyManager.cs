using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    private int currentMoney = 0;
    public int GetCurrentMoney { get { return currentMoney;} }

    public Text moneyText;

    public GameObject effectText;


    private void Start()
    {
        currentMoney = GameDataStorageManager.Instance.GetMoney;
        moneyText.text = currentMoney.ToString();
    }


    public void ChangeCurrentMoney(int amount)
    {
        if(amount > 99999)
        {
            amount = 99999;
        }

        StartCoroutine( MoneyAnimation(amount) ); //カウントアニメーション再生


        if(amount != currentMoney) //同額でなければ
        {
            if (amount > currentMoney) //増減分のアニメーション再生
            {
                effectText.GetComponent<Text>().text = "+" + (amount - currentMoney).ToString();
                effectText.GetComponent<Animator>().SetTrigger("canPlay");
            }
            else
            {
                effectText.GetComponent<Text>().text = "-" + (currentMoney - amount).ToString();
                effectText.GetComponent<Animator>().SetTrigger("canPlay");
            }
        }
        currentMoney = amount;　//最終的な値に設定
        GameDataStorageManager.Instance.ChangeMoneyAmount(amount); //データ保存
    }

    private IEnumerator MoneyAnimation(int value)
    {
        Debug.Log("カウント開始");
        int updateMoney = currentMoney;
        
        //変更先の数値が異なれば処理開始
        if(updateMoney != value)
        {
            //現在の数値より多い場合、少ない場合
            if (updateMoney < value)
            {
                //現在の数値より多い場合
                while (updateMoney < value)
                {
                    updateMoney ++;
                    moneyText.text = updateMoney.ToString();
                    // 0.01秒待つ
                    yield return new WaitForSeconds(0.01f);
                }
               
            }
            else
            {
                //現在の数値より少ない場合
                while (updateMoney > value)
                {
                    updateMoney --;
                    moneyText.text = updateMoney.ToString();
                    // 0.01秒待つ
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
        else
        {
            Debug.Log("値段一致完了");
        }
    }

    public bool CheckMoneyAmount(int amount)
    {
        Debug.Log("CurrentMoney：" + currentMoney + "  料金：" + amount);
        if(currentMoney >= amount)
        {
            Debug.Log("十分なお金がある");
            return true;
        }
        else
        {
            Debug.Log("お金が足りないよ");
            return false;
        }
    }

    public void NotEnoughtMoneyAnimation()
    {
        this.gameObject.GetComponent<Animator>().SetTrigger("doAnimation");
    }

}
