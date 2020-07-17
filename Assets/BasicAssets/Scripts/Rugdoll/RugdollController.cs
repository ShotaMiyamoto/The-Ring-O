using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RugdollController : MonoBehaviour
{
    private Rigidbody[] rbs;
    private float timeElapsed = 0f;
    [SerializeField] private float destroyLimit = 3f;

    public Material[] bodyMaterials;
    public GameObject[] costumes;

    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody i in rbs)
        {
            i.AddForce(new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)), ForceMode.VelocityChange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > destroyLimit)
        {
            Destroy(this.gameObject);
        }
    }

    public void SyncAppear(int charaNum)
    {
        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Battle_Single:

                switch (charaNum)
                {
                    case 0: //プレイヤー側(Multiだと左側)
                        this.GetComponent<SkinnedMeshRenderer>().material = GameDataStorageManager.Instance.GetMatInfo(GameDataStorageManager.Instance.GetSelectedBodyColorNum);
                        costumes[GameDataStorageManager.Instance.GetSelectedCostumeNum].SetActive(true);
                        Debug.Log("左側の見た目と同期");
                        break;

                    case 1: //敵側(Multiだと右側)
                        this.GetComponent<SkinnedMeshRenderer>().material = GameDataStorageManager.Instance.GetRandomBodyColorDicMaterial(this.name);
                        costumes[GameDataStorageManager.Instance.GetRandomCostumeDicNum(this.name)].SetActive(true);
                        Debug.Log("右側の見た目と同期");
                        break;

                    default:
                        Debug.Log("そんなキャラ番号はない");
                        break;
                }
                break;

            case GameStateManager.GameState.Battle_Multi:

                switch (charaNum)
                {
                    case 0: //プレイヤー側(Multiだと左側)
                        this.GetComponent<SkinnedMeshRenderer>().material = GameDataStorageManager.Instance.GetMatInfo(GameDataStorageManager.Instance.SelecBColNumLeft);
                        costumes[GameDataStorageManager.Instance.SelecCosNumLeft].SetActive(true);
                        Debug.Log("左側の見た目と同期");
                        break;

                    case 1: //敵側(Multiだと右側)
                        this.GetComponent<SkinnedMeshRenderer>().material = GameDataStorageManager.Instance.GetMatInfo(GameDataStorageManager.Instance.SelecBColNumRight);
                        costumes[GameDataStorageManager.Instance.SelecCosNumRight].SetActive(true);
                        Debug.Log("右側の見た目と同期");
                        break;

                    default:
                        Debug.Log("そんなキャラ番号はない");
                        break;
                }

                break;

            default:
                Debug.Log("そんなステートでは使わないメソッド");
                break;

        }

    }
}
