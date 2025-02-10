using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 裝備數據
/// </summary>
/*
public class InventoryData
{
    public double ID;               //  ID
    public InventoryNameEnum Name;  //  名稱
    public int Rare;                //  稀有度
    public int Location;            //  位置
    public int Level;               //  等級
}
*/
[Serializable]
public class InventoryData
{
    public InventoryNameEnum Name;                      //  名稱
    public int Rare;                                    //  稀有度
    public InventoryLocationEnum Location;              //  位置
    public int Level;                                   //  等級
    public InventoryInfo inventoryInfo;
}


/// <summary>
/// 裝備組合類別
/// </summary>
public enum InventoryNameEnum
{
    Scholar,                        //  學者
    Priest,                         //  牧師
}

public enum InventoryLocationEnum
{
    ONE,
    TWO,
    THREE,
    FOUR,
}


/// <summary>
/// 裝備屬性
/// </summary>
public class InventoryInfo
{
    public double HP;                             //  生命
    public double HP_Percent;                     //  生命加成
    public double Atk;                            //  攻擊
    public double Atk_Percent;                    //  攻擊加成
    public double Def;                            //  防禦
    public double Def_Percent;                    //  防禦加成
    public double Critical;                       //  暴擊
    public double CritDmg;                        //  暴擊傷害
    public double Speed;                          //  速度
    public double EffectHit;                      //  效果命中
    public double EffectResist;                   //  效果抵抗
}

public class IinventoryGroupEffect
{
    public string GroupName;                      //  套裝名稱
    public string Two_GroupEffect;                //  兩件套效果
    public int[] Two_EffectInfo;                  //  效果屬性
    public string Four_GroupEffect;               //  四件套效果
    public int[] Four_EffectInfo;                 //  效果屬性
}