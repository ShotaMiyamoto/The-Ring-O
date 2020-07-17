//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class CustomizeController : MonoBehaviour
//{
//    public SkinnedMeshRenderer playerBodyMesh;
//    public SkinnedMeshRenderer enemyBodyMesh;

//    public Transform headCosPos;
//    public Transform bodyCodPos;
//    private int currentCostumeNum = 0;

//    public Image playerBodyColorIcon;
//    public Image playerCostumeIcon;

//    public Image enemyBodyColorIcon;
//    public Image enemyCostumeIcon;

//    private Material[] bodyMaterials;

//    private void Start()
//    {
//        ChangeBodyColor(GameDataStorageManager.Instance.GetSelectedBodyColorNum);
//        ChangeCostume(GameDataStorageManager.Instance.GetSelectedCostumeNum);
//    }

//    public void ChangeBodyColor(int num)
//    {

//        if(num >= bodyMaterials.Length)
//        {
//            num = 0; 
//        }

//        playerBodyMesh.material = bodyMaterials[num];

//    }

//    public void ChangeCostume(int num)
//    {

//        if (num >= playerCostumes.Length)
//        {
//            num = 0;
//        }

//        playerCostumes[currentCostumeNum].SetActive(false);
//        playerCostumes[num].SetActive(true);
//        currentCostumeNum = num;
//    }

//    public void RandomizeEnemyAppear(bool isFirst)
//    {
//        if (isFirst)
//        {
//            //一回目
//            int bodyColorNum = Random.Range(0, bodyMaterials.Length);
//            int costumeNum = Random.Range(0, enemyCostumes.Length);

//            enemyBodyMesh.material = bodyMaterials[bodyColorNum]; //マテリアルをランダムで設定
//            enemyCostumes[costumeNum].SetActive(true); //コスチュームをランダムで設定

//            GameDataStorageManager.Instance.EnemyBodyColorNum = bodyColorNum; //シーンロードで消えないようにデータ保存
//            GameDataStorageManager.Instance.EnemyCostumeNum = costumeNum;　//シーンロードで消えないようにデータ保存

//            Debug.Log("敵外見データを代入");
//        }
//        else
//        {
//            //二回目以降
//            enemyBodyMesh.material = bodyMaterials[GameDataStorageManager.Instance.EnemyBodyColorNum]; //マテリアルをランダムで設定
//            enemyCostumes[GameDataStorageManager.Instance.EnemyCostumeNum].SetActive(true); //コスチュームをランダムで設定
//            Debug.Log("敵の見た目を同期");
//        }
//    }

//    public void SyncBattleIcons()
//    {
//        playerBodyColorIcon.sprite = bodyColorIcon[GameDataStorageManager.Instance.GetSelectedBodyColorNum];
//        playerCostumeIcon.sprite = CostumeIcon[GameDataStorageManager.Instance.GetSelectedCostumeNum];
//        playerCostumeIcon.SetNativeSize();

//        enemyBodyColorIcon.sprite = bodyColorIcon[GameDataStorageManager.Instance.EnemyBodyColorNum];
//        enemyCostumeIcon.sprite = CostumeIcon[GameDataStorageManager.Instance.EnemyCostumeNum];
//        enemyCostumeIcon.SetNativeSize();
//    }
//}
