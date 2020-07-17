using UnityEngine;

public class DeadLineController : MonoBehaviour
{
    [SerializeField] private bool endGameWhenLose = true; //負けたときにラウンドを止めるかどうか
    [SerializeField] private GameObject[] characters = default; //キャラクターたち

    private void OnTriggerEnter(Collider other)
    {

        if (!GameManager.Instance.IsBattleEnded)
        {
            switch (other.tag)
            {
                case "Player": //プレイヤーの負け　敵の勝ち
                    characters[0].GetComponent<PlayerController>().Lose();
                    characters[1].GetComponent<EnemyController>().Win();
                    break;

                case "Enemy": //プレイヤーの勝ち　敵の負け
                    characters[0].GetComponent<PlayerController>().Win();
                    characters[1].GetComponent<EnemyController>().Lose();
                    break;

                case "LeftPlayer": //左プレイヤーの負け　右プレイヤーの勝ち
                    characters[0].GetComponent<PlayerController>().Lose();
                    characters[1].GetComponent<PlayerController>().Win();
                    break;

                case "RightPlayer": //左プレイヤーの勝ち　右プレイヤーの負け
                    characters[0].GetComponent<PlayerController>().Win();
                    characters[1].GetComponent<PlayerController>().Lose();
                    break;

                default:
                    Debug.Log("そんなキャラはいない");
                    break;
            }


            if (endGameWhenLose)
            {
                GameManager.Instance.IsBattleEnded = true;
            }
        }
    }


}
