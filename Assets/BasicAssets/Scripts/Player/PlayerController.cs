//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
    public Joystick joystick;
    public bool SetCanMove { set { base.canMove = value; } }

     
    public override void Update()
    {
        //ジョイスティックからの情報をもとに移動方向を設定
        if (!GameManager.Instance.IsBattleEnded)
        {
            moveDir.x = joystick.Horizontal;
            moveDir.z = joystick.Vertical;
            base.Update();
        }
    }

    public void SetAppearManually(int bColorNum, int cosNum)
    {
        base.bodyMesh.material = gdsm.GetMatInfo(bColorNum); //選択している値を設定
        costumes[cosNum].SetActive(true); //選択している値を設定
    }

    public override void Win()
    {                           
        switch(GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Battle_Single:
                UIController.Instance.UISetActivater(UIController.UIGroup.Character1, 0, false); //Joystick無効化
                break;

            case GameStateManager.GameState.Battle_Multi:
                UIController.Instance.SetActivatePlayerJoysticks(false); //2つのJoystickを無効化
                break;
        }
        base.Win(); //0がプレイヤー番号 (左側)　1が敵（右側）
    }

    public override void Lose()
    {
        if(GameStateManager.Instance.StateProperty == GameStateManager.GameState.Battle_Single)
        {
            UIController.Instance.UISetActivater(UIController.UIGroup.Character1, 0, false); //Joystick無効化
        }
        base.Lose();
    }
}
