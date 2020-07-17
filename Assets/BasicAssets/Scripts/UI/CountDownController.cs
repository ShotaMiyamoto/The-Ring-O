using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CountDownController : MonoBehaviour
{
    [SerializeField] private Text text = default;
    private int count = 4;

    //＝＝＝＝＝＝＝＝＝＝＝シングルバトル用＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private EnemyController enemyController = default;
    [SerializeField] private PlayerController playerController = default;

    //＝＝＝＝＝＝＝＝＝＝＝マルチバトル用＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private PlayerController leftSidePlayerController = default;
    [SerializeField] private PlayerController rightSidePlayerController = default;

    //private Animator animator;


    private void Start()
    {

        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Title:
                //なにもしない
                //Debug.Log("そんなステートはない");
                break;

            case GameStateManager.GameState.Battle_Single:
                playerController.SetCanMove = false;
                enemyController.SetCanMove = false;
                //Debug.Log("Battle_Singleステート　canMove False");
                break;

            case GameStateManager.GameState.Battle_Multi:
                leftSidePlayerController.SetCanMove = false;
                rightSidePlayerController.SetCanMove = false;
                //Debug.Log("Battle_Multiステート　canMove False");
                break;

            default:
                Debug.Log("そんなステートはない");
                break;
        }
    }

    public void SetText()
    {
        count--;
        switch (count)
        {
            case 3:
                text.text = count.ToString();
                break;

            case 2:
                text.text = count.ToString();
                break;

            case 1:
                text.text = count.ToString();
                break;

            case 0:
                text.text = "GO!";

                switch (GameStateManager.Instance.StateProperty)
                {
                    case GameStateManager.GameState.Title:
                        //なにもしない
                        break;

                    case GameStateManager.GameState.Battle_Single:
                        playerController.SetCanMove = true;
                        enemyController.SetCanMove = true;
                        //Debug.Log("Battle_Singleステート　canMove True");
                        break;

                    case GameStateManager.GameState.Battle_Multi:
                        leftSidePlayerController.SetCanMove = true;
                        rightSidePlayerController.SetCanMove = true;
                        //Debug.Log("Battle_Multiステート　canMove True");
                        break;

                    default:
                        Debug.Log("そんなステートはない");
                        break;
                }
                break;

            case -1:
                this.gameObject.SetActive(false);
                break;

            default:
                Debug.Log("そんな数値ない");
                break;
        }
    }
}
