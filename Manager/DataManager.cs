using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;



public static class DataManager
{

    //public readonly static string dataPath = Application.persistentDataPath;

    /// <summary>
    /// 可寫成讀取JSON格式來增加角色池
    /// </summary>
    /*
    public static void PlayerAddInfo()
    {
        playerInfos.Add(InitPlayerData[0]);
        //double[] store = { 0, 1, 2, 4 };

        //playerInfos[0].SetInventoryToPlayer(store);
    }
    */

    /// <summary>
    /// 已獲得角色庫
    /// </summary>
    public static List<PlayerInfoData> playerInfos = new List<PlayerInfoData>();

    /// <summary>
    /// 儲存裝備Store
    /// </summary>
    public static Dictionary<double, InventoryData> InventoryStore = new Dictionary<double, InventoryData>();

    /// <summary>
    /// 角色初始參數
    /// </summary>
    public static List<PlayerInfoData> InitPlayerData = new List<PlayerInfoData>();

    public static List<EnemyInfoData> InitEnemyData = new List<EnemyInfoData>();

    #region 角色技能參數
    /// <summary>
    /// 角色技能參數
    /// </summary>
    public static Dictionary<int, List<SkilsInfo>> PlayerSkils = new Dictionary<int, List<SkilsInfo>>()
    {
        { 0, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100},
                new SkilsInfo() {isAttack = true,MoreTarget = true,Damage = 20},
                new SkilsInfo() {isAttack = true,Damage = 10} 
        }},
        { 1, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100},
                new SkilsInfo() {isAttack = true,MoreTarget = true,Damage = 30},
                new SkilsInfo() {isAttack = true,Damage = 90}
        }},　
        { 2, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100},
                new SkilsInfo() {isAttack = true,MoreTarget = true,Damage = 40},
                new SkilsInfo() {isAttack = true,Damage = 50}
        }},
        { 3, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100},
                new SkilsInfo() {isAttack = true,MoreTarget = true,Damage = 50},
                new SkilsInfo() {isAttack = true,Damage = 100}
        }},
        { 4, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100},
                new SkilsInfo() {isAttack = true,MoreTarget = true,Damage = 60},
                new SkilsInfo() {isAttack = true,Damage = 20}
        }},
    };

    #endregion

    #region 敵人技能參數
    /// <summary>
    /// 敵人技能參數
    /// </summary>
    public static Dictionary<int, List<SkilsInfo>> EnemySkils = new Dictionary<int, List<SkilsInfo>>()
    {
        { 0, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100,Hit = 1},
                new SkilsInfo(){isAttack = true, Damage = 80 ,MoreTarget = true,Hit = 3},
                new SkilsInfo(){isAttack = true, Damage = 200,Hit = 2},
        }},
        { 1, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100 ,Hit = 1},
                new SkilsInfo(){isAttack = true, Damage = 20 ,MoreTarget = true,Hit = 2},
        }},
        { 2, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100 },
                new SkilsInfo(){isAttack = true, Damage = 80 ,MoreTarget = true},
        }},
        { 3, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100 },
                new SkilsInfo(){isAttack = true, Damage = 80 ,MoreTarget = true},
        }},
        { 4, new List<SkilsInfo>(){
                new SkilsInfo() {isAttack = true,Damage = 100 },
                new SkilsInfo(){isAttack = true, Damage = 80 ,MoreTarget = true},
        }},
    };

    #endregion

    #region 儲存裝備Data
    /// <summary>
    /// 儲存裝備欄位
    /// </summary>
    /// 
    /*
    public static Dictionary<double,InventoryData> InventoryStore = new Dictionary<double, InventoryData>()
    {
        {0,new InventoryData(){Name =  InventoryNameEnum.Scholar,Rare = 0,Location = InventoryLocationEnum.ONE,Level = 1,
        inventoryInfo = new InventoryInfo(){EffectHit = 5,EffectResist = 100, Def_Percent = 3,Critical = 30,CritDmg = 100 }}},

        {1,new InventoryData(){Name = InventoryNameEnum.Priest,Rare =4,Location = InventoryLocationEnum.FOUR,Level = 2,
        inventoryInfo = new InventoryInfo(){HP = 80,EffectResist = 11, Def_Percent = 5,Critical = 3,CritDmg = 8 }}},

        {2,new InventoryData(){Name = InventoryNameEnum.Scholar,Rare =2,Location = InventoryLocationEnum.TWO,Level = 12,
        inventoryInfo = new InventoryInfo(){Atk = 10,Atk_Percent = 50, Def_Percent = 5,Critical = 3,Speed = 4 }}},

        {3,new InventoryData(){Name = InventoryNameEnum.Priest,Rare =3,Location = InventoryLocationEnum.THREE,Level = 2,
        inventoryInfo = new InventoryInfo() { EffectHit = 9, EffectResist = 11, Def_Percent = 1, Def = 33, CritDmg = 8 }}},

        {4,new InventoryData(){Name = InventoryNameEnum.Priest,Rare =0,Location = InventoryLocationEnum.THREE,Level = 2,
        inventoryInfo = new InventoryInfo() { HP_Percent = 10, EffectResist = 11, Def_Percent = 9, Def = 33, CritDmg = 8 }}},

        {5,new InventoryData(){Name = InventoryNameEnum.Priest,Rare =2,Location = InventoryLocationEnum.THREE,Level = 2,
        inventoryInfo = new InventoryInfo() { EffectHit = 9, EffectResist = 11, Def_Percent = 1, Def = 33, CritDmg = 8 }}},

        {6,new InventoryData(){Name = InventoryNameEnum.Priest,Rare =1,Location = InventoryLocationEnum.THREE,Level = 2,
        inventoryInfo = new InventoryInfo() { EffectHit = 9, EffectResist = 11, Def_Percent = 1, Def = 33, CritDmg = 8 }}},
    };

    */

    #endregion

    #region 套裝屬性Data

    /// <summary>
    /// 套裝屬性配置
    /// </summary>
    public static List<IinventoryGroupEffect> GroupEffectList = new List<IinventoryGroupEffect>()
    {
        {new IinventoryGroupEffect(){ GroupName = $"學者之才智",
                                      Two_GroupEffect = $"二件套: 曓擊加成",       //  10%
                                      Two_EffectInfo = new int[]{10},
                                      Four_GroupEffect = $"四件套: 造成暴擊時，絕技和絕招傷害增加",       //  25%
                                      Four_EffectInfo = new int[]{25} }},

        {new IinventoryGroupEffect(){ GroupName = $"神父之祈願",
                                      Two_GroupEffect = $"二件套: 生命加成",       //  10%
                                      Two_EffectInfo = new int[]{10},
                                      Four_GroupEffect = $"四件套: 治療時，回復量提升",     //  20%
                                      Four_EffectInfo = new int[]{20} }},
    };

    #endregion
    
    #region 角色初始參數
    /// <summary>
    /// 角色初始參數(首次獲得時配置參數)
    /// </summary>
    /*
    public static List<PlayerInfoData> InitPlayerData = new List<PlayerInfoData>()
    {
        {new PlayerInfoData(){ID = 0,Name = "aa",Level = 1,
        HP = 100,Def = 30,Atk = 120,Critical = 10,CriticalDamage = 50,Speed = 15,EffectHit = 0,EffectResist = 0}},

        {new PlayerInfoData(){ID = 1,Name = "bb",Level = 1,
        HP = 120,Def = 25,Atk = 100,Critical = 20,CriticalDamage = 60,Speed = 12,EffectHit = 0,EffectResist = 0}},

        {new PlayerInfoData(){ID = 2,Name = "cc",Level = 1,
        HP = 130,Def = 30,Atk = 90,Critical = 30,CriticalDamage = 30, Speed = 10,EffectHit = 0,EffectResist = 0}},

        {new PlayerInfoData(){ID = 3,Name = "dd",Level = 1,
        HP = 95,Def = 40,Atk = 140,Critical = 10,CriticalDamage = 80, Speed = 13,EffectHit = 0,EffectResist = 0}},
    };

    */
    #endregion
}
