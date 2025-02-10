using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 角色數值介面
/// </summary>
[Serializable]
public class ICharacterInfo
{
    public int ID;                   //  該角色ID
    public string Name;              //  名稱
    public int Level;                //  等級
    public double HP;                //  生命
    public double Def;               //  防禦
    public double Atk;               //  攻擊
    public double Critical;          //  暴擊
    public double CriticalDamage;    //  暴擊傷害
    public double Speed;             //  速度
    public double EffectHit;         //  效果命中
    public double EffectResist;      //  效果抵抗

    public bool isDead;              //  已陣亡
    
    /// <summary>
    /// 計算損傷
    /// </summary>
    public virtual void GetDamageAmount(double Amount)
    {
        Debug.Log($"承受傷害角色: ({Name}) HP: {HP} => 受到損傷: {Amount}");
        HP -= Amount;
        Debug.Log($"承受傷害角色: ({Name}) HP: {HP}");
        if (HP <= 0)
        {
            Debug.Log($"承受傷害角色: ({Name})  已陣亡");
            isDead = true;
        }
    }

    public virtual void GetBuffAndDeBuffAmount()
    {

    }

    public virtual void DoSkils(ICharacterInfo character,int SkilID)
    {
        
    }

}

/// <summary>
/// 角色資料
/// </summary>
public class PlayerInfoData : ICharacterInfo
{
    public int EquipMax = 4;
    public List<double> inventorysID;
    public Vector3 CurrentPos;

    public override void DoSkils(ICharacterInfo character, int SkilID)
    {
        var Skil =  DataManager.PlayerSkils[character.ID];
        GameManager.Instance.ReceiveEffect(Skil[SkilID].Damage);
    }

    /// <summary>
    /// 裝備
    /// </summary>
    /// <param name="id"></param>
    public void Equip(double id)
    {
        if(inventorysID.Count <= 0)
            inventorysID.Add(id);
        else
        {
            for (int i = 0; i < inventorysID.Count; i++)
            {
                if (DataManager.InventoryStore[inventorysID[i]].Location == DataManager.InventoryStore[id].Location)
                {
                    //Debug.Log($"當前裝備: {inventorysID[i]} 準備替換: {id}");
                    inventorysID.RemoveAt(i);
                }
            }
            inventorysID.Add(id);
        }
    }
}

[Serializable]
public class EnemyInfoData : ICharacterInfo
{
    public int TempID;                          //  戰鬥中暫存ID，重複敵人判斷
    public Vector3 CurrentPos;


    public override void DoSkils(ICharacterInfo character, int SkilID)
    {
        var Skil = DataManager.EnemySkils[character.ID];
        GameManager.Instance.ReceiveEffect(Skil[SkilID].Damage);
    }
}
