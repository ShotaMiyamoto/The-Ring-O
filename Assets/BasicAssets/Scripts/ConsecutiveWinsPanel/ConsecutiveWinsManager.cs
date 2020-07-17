using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsecutiveWinsManager : MonoBehaviour
{
    [SerializeField] private int currentWins = 0;
    [SerializeField] private Text winText = default;
    [SerializeField] private GameObject bonusPanel = default;
    [SerializeField] private Text bonusText = default;

    // Start is called before the first frame update
    void Start()
    {
       //バトル前の連勝数を同期
       currentWins = GameDataStorageManager.Instance.BestConsecutiveWinsProperty;
       bonusPanel.SetActive(false);
    }

    public float MoneyMagnification()
    {
        currentWins = GameDataStorageManager.Instance.BestConsecutiveWinsProperty;
        
        
        if(currentWins < 3)
        {
            //3連勝以下かつ1勝目でなければ
            Debug.Log("お金ボーナス 1倍");
            return 1f;
        }
        else if(currentWins >= 3 && currentWins < 6)
        {
            //6連勝以下
            Debug.Log("お金ボーナス 1.2倍");
            bonusText.text = "1.2";
            bonusPanel.SetActive(true);
            return 1.2f;
        }
        else if(currentWins >= 6 && currentWins < 10)
        {
            //10連勝以下
            Debug.Log("お金ボーナス 1.4倍");
            bonusText.text = "1.4";
            bonusPanel.SetActive(true);
            return 1.4f;
        }
        else if(currentWins >= 10 && currentWins < 15)
        {
            //15連勝以下
            Debug.Log("お金ボーナス 1.6倍");
            bonusText.text = "1.6";
            bonusPanel.SetActive(true);
            return 1.6f;
        }
        else if(currentWins >= 15 && currentWins < 20)
        {
            //20連勝以下
            Debug.Log("お金ボーナス 1.8倍");
            bonusText.text = "1.8";
            bonusPanel.SetActive(true);
            return 1.8f;
        }
        else if(currentWins >= 20 && currentWins < 30)
        {
            //30連勝以下
            Debug.Log("お金ボーナス 2倍");
            bonusText.text = "2";
            bonusPanel.SetActive(true);
            return 2f;
        }
        else
        {
            //30連勝以上は3倍ボーナスで固定
            Debug.Log("お金ボーナス 3倍");
            bonusText.text = "3";
            bonusPanel.SetActive(true);
            return 3f;
        }
    }

    /// <summary>
    /// アニメーション開始前にテキストに初期値を代入しておく
    /// </summary>
    /// <param name="win"></param>
    public void SetWinText(bool win)
    {
        if (win)
        {
            winText.text = "0";
        }
        else
        {
            winText.text = currentWins.ToString();
        }
    }

    public IEnumerator TextAnimation(bool win)
    {

        int updateValue;

        if (win)　//勝った場合
        {
            updateValue = 0;
            currentWins = GameDataStorageManager.Instance.BestConsecutiveWinsProperty;

            //現在の数値より多い場合
            while (updateValue < currentWins)
            {
                updateValue ++;
                winText.text = updateValue.ToString();
                //Debug.Log("テキスト更新中");
                // 0.05秒待つ
                yield return new WaitForSeconds(0.05f);
            }
        }
        else　//負けた場合
        {
            Debug.Log("負けたので連勝数リセット");
            updateValue = currentWins;

            //0より多い場合
            while (updateValue > 0)
            {
                updateValue --;
                winText.text = updateValue.ToString();
                //Debug.Log("テキスト更新中");
                // 0.05秒待つ
                yield return new WaitForSeconds(0.05f);
            }
        }

        Debug.Log("連勝数一致完了");
    }
}
