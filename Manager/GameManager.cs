using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using Cinemachine;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.VisualScripting;

public class GameManager : UnitySingleton<GameManager>
{
    [SerializeField]
    FightView fightView;

    int[] Enemy = {0, 1, 1, 0 ,0};                      //  配置關卡角色
    int[] Player = {0};                                 //  組隊人物

    [Header("=== 當前行動角色 ===")]
    [SerializeField]
    GameObject CurrentPlayerPos;

    [SerializeField]
    List<GameObject> PlayerListPos = new List<GameObject>();
    [SerializeField]
    List<GameObject> EnemyListPos = new List<GameObject>();

    List<ICharacterInfo> AllCharacterActionList = new List<ICharacterInfo>();       //  行動列

    List<PlayerInfoData> playerDatas;                                               //  BattlePlayer數據
    List<EnemyInfoData> enemyDatas;                                                 //  BattleEnemy數據
    
    ICharacterInfo CurrentRoundChar;                                                //  當前行動角色
    ICharacterInfo[] Targets;                                                       //  被選定目標

    [SerializeField]int TurnAmount {  get; set; }

    bool FirstEntryTurn = true;

    [SerializeField] GameObject MouseController;
    [SerializeField] GameObject UIAmount;
    [SerializeField] GameObject SelectTarget_VFX;
    [SerializeField] Transform TargetVFX_Parent;

    [SerializeField] CinemachineVirtualCamera FightViewCamera;
    [SerializeField] CinemachineVirtualCamera PrepareViewCamera;

    //  選中目標暫存
    ICharacterInfo[] TempSelect;                                                    //  Select Target Temp
    private int CurrentSkilID;

    int PlayerListIndex;                    //  暫存站位index

    /// <summary>
    /// 設置當前SkilID
    /// </summary>
    /// <param name="id"></param>
    public void SetCurrentSkilID(int id)
    {
        CurrentSkilID = id;
    }

    /// <summary>
    /// 獲取當前SkilID
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSkilID()
    {
        return CurrentSkilID;
    }

    /// <summary>
    /// 重置SkilID
    /// </summary>
    public void InitSkilID()
    {
        CurrentSkilID = 0;
    }

    public ICharacterInfo GetCurrentChar()
    {
        return CurrentRoundChar;
    }

    public ICharacterInfo[] GetTarget()
    {
        return Targets;
    }

    private void UpdateTargets(ICharacterInfo[] targets)
    {
        Targets = targets;
    }

    public void ClearSelectTarget()
    {
        TempSelect = null;
    }

    public void ActiveMouseController(bool active)
    {
        if (MouseController.activeSelf)
        {
            MouseController.SetActive(active);
        }
    }

    public int[] GetPlayerGroup()
    {
        return Player;
    }

    /// <summary>
    /// 獲取戰鬥中角色數據
    /// </summary>
    /// <returns></returns>
    public List<PlayerInfoData> GetPlayerInfoDatas()
    {
        return playerDatas;
    }

    public override void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        SaveLoadManager.Instance.Load(SaveLoadEnum.EnemyInfoData);
        Init();
    }

    private void Start()
    {

        StartCoroutine(GameLoop(GameFlow.NewTurn));                 //  開始回合
    }


    private TurnData turnData;

    /// <summary>
    /// 回合關卡資料
    /// </summary>
    public class TurnData
    {
        public string LevelName;            //  關卡名
        public double CurrentRound;         //  當前輪數
        public int TurnAmount;              //  回合數
    }

    /// <summary>
    /// 初始化配置
    /// </summary>
    private void Init()
    {
        turnData = new TurnData();
        turnData.LevelName = "";
        turnData.CurrentRound = 0;
        LoadCharacter();

    }

    private IEnumerator GameLoop(GameFlow flow)
    {
        switch (flow)
        {
            case GameFlow.RoundStart:
                StartCoroutine(RoundStart());
                break;
            case GameFlow.RoundOver:

                //  回合結束時，我方角色退回原定站位
                if (CurrentRoundChar.GetType() == typeof(PlayerInfoData))
                {
                    CurrCameraController(PrepareViewCamera,false);
                    Transform CurrentObj = CurrentPlayerPos.transform.GetChild(0);
                    CurrentObj.SetParent(PlayerListPos[PlayerListIndex].transform);
                    CurrentObj.localPosition = Vector3.zero;
                    CurrentObj.localRotation = Quaternion.identity;
                    CurrentObj.localScale = Vector3.one;
                }

                //  Linq轉型，可優化
                var playerList = playerDatas.Cast<ICharacterInfo>().ToList();
                var enemyList = playerDatas.Cast<ICharacterInfo>().ToList();

                if (JudgeGameLoop(playerList))
                {
                    yield return GameLoop(GameFlow.GameOver);
                }
                if (JudgeGameLoop(enemyList))
                {
                    yield return GameLoop(GameFlow.GameOver);
                }

                else if (CurrentRoundChar == AllCharacterActionList[AllCharacterActionList.Count-1])
                {
                    Debug.Log($"========== 進到下回合 ==========");
                    yield return GameLoop(GameFlow.NewTurn);
                }
                else
                {

                    for(int i = 0; i < AllCharacterActionList.Count; i++)
                    {
                        if (CurrentRoundChar.GetType() == typeof(EnemyInfoData) && 
                            CurrentRoundChar.GetType() == AllCharacterActionList[i].GetType())
                        {
                            var curr = CurrentRoundChar as EnemyInfoData;
                            var allChar = AllCharacterActionList[i] as EnemyInfoData;
                            if (curr.TempID == allChar.TempID)
                            {
                                CurrentRoundChar = AllCharacterActionList[i + 1];
                                break;
                            }
                        
                        }
                        else if (CurrentRoundChar.GetType() == typeof(PlayerInfoData) &&
                                 CurrentRoundChar == AllCharacterActionList[i])
                        {
                            CurrentRoundChar = AllCharacterActionList[i + 1];
                            break;
                        }
                        
                    }
                    yield return GameLoop(GameFlow.RoundStart);
                }

                break;
            case GameFlow.NewTurn:
                StartCoroutine(StartTurn());
                break;
            case GameFlow.GameOver:
                //  Player全滅 or 完成Quest
                Debug.Log("結束....");
                yield break;
        }
    }

    private IEnumerator RoundStart()
    {
        Debug.Log($"開始 ({CurrentRoundChar.Name}) 的回合...");
        if (CurrentRoundChar.GetType() == typeof(PlayerInfoData))
        {
            fightView.CreateSkils();
            SetCurrentPosToTarget();
            CurrCameraController(PrepareViewCamera,true);
        }
        else
        {
            EnemyAI();
        }

        yield return null;
    }

    /// <summary>
    /// 回合開始
    /// </summary>
    private IEnumerator StartTurn()
    {
        SetAllCharacterAction();

        if(FirstEntryTurn)
            yield return new WaitForSeconds(3.0f);

        CurrentRoundChar = AllCharacterActionList[0];
        TurnAmount++;
        //Debug.Log($"第 ({TurnAmount}) 回合");
        yield return GameLoop(GameFlow.RoundStart);
    }

    /// <summary>
    /// 動畫播放協程
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayAnimationCoroutine()
    {
        int Time = 3;
        //Debug.Log($"等待動畫 {Time}秒");
        yield return new WaitForSeconds(Time);
        //ClearCurrAnimation();
        CurrCameraController(FightViewCamera,false);

        Debug.Log($"結束 ({CurrentRoundChar.Name}) 回合");

        yield return GameLoop(GameFlow.RoundOver);
    }

    /// <summary>
    /// 播放攻擊動畫
    /// </summary>
    public void PlayCurrAnimation(string Anim,bool isPlay)
    {
        if (CurrentRoundChar.GetType() == typeof(PlayerInfoData))
        {
            Transform CurrentObj = CurrentPlayerPos.transform.GetChild(0);
            Animator anim = CurrentObj.GetComponentInChildren<Animator>();
            anim.SetBool(Animator.StringToHash(Anim),isPlay);
        }
        else
        {
            EnemyInfoData enemy = CurrentRoundChar as EnemyInfoData;
            Transform CurrentObj = EnemyListPos[enemy.TempID].transform;
            if (GetTarget().Length <= 1)
            {
                for (int i = 0; i < PlayerListPos.Count;i++)
                {
                    if (PlayerListPos[i].transform.childCount >= 1 && 
                        Targets[0].Name == PlayerListPos[i].transform.GetChild(0).name)
                    {
                        GameObject obj = PlayerListPos[i].transform.GetChild(0).gameObject;
                        CurrentObj.GetChild(0).LookAt(obj.transform);
                    }
                }
            }
            Animator anim = CurrentObj.GetComponentInChildren<Animator>();
            anim.SetBool(Animator.StringToHash(Anim),isPlay);
        }
    }

    public void ClearCurrAnimation()
    {
        if (CurrentRoundChar.GetType() == typeof(EnemyInfoData))
        {
            PlayCurrAnimation("Attack01", false);
        }
        else
        {
            PlayCurrAnimation("AttackIDLE", false);
            PlayCurrAnimation("Attack01", false);
        }
        
    }


    /// <summary>
    /// 載入角色到指定位置
    /// </summary>
    private void LoadCharacter()
    {
        List<PlayerInfoData> PlayerdataList = new List<PlayerInfoData>();
        List<EnemyInfoData> EnemydataList = new List<EnemyInfoData>();

        for (int i=0;i<Player.Length;i++)
        {
            GameObject obj = Instantiate(AssetManager.Instance.GetModelAssets(ModelAlbumEnum.PlayerAlbum).modelAlbum[i]);
            obj.name = $"{AssetManager.Instance.GetModelAssets(ModelAlbumEnum.PlayerAlbum).modelAlbum[i].name}";
            obj.transform.SetParent(PlayerListPos[i].transform);
            obj.transform.localPosition = new Vector3(0,0,0);
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            PlayerInfo info = obj.GetComponent<PlayerInfo>();
            PlayerInfoData data = new PlayerInfoData();
            data = info.SetPlayerAttri(DataManager.playerInfos[Player[i]]);
            data.CurrentPos = obj.transform.position;
            PlayerdataList.Add(data);
        }
        for (int i = 0; i < Enemy.Length; i++)
        {
            GameObject obj = Instantiate(AssetManager.Instance.GetModelAssets(ModelAlbumEnum.EnemyAlbum).modelAlbum[Enemy[i]]);
            obj.name = $"{AssetManager.Instance.GetModelAssets(ModelAlbumEnum.EnemyAlbum).modelAlbum[Enemy[i]].name}";
            obj.transform.SetParent(EnemyListPos[i].transform);
            obj.transform.localPosition = new Vector3(0,0,0);

            EnemyInfoData data = new EnemyInfoData();

            data.ID = DataManager.InitEnemyData[Enemy[i]].ID;
            data.Name = DataManager.InitEnemyData[Enemy[i]].Name;
            data.Level = DataManager.InitEnemyData[Enemy[i]].Level;
            data.HP = DataManager.InitEnemyData[Enemy[i]].HP;
            data.Def = DataManager.InitEnemyData[Enemy[i]].Def;
            data.Atk = DataManager.InitEnemyData[Enemy[i]].Atk;
            data.Critical = DataManager.InitEnemyData[Enemy[i]].Critical;
            data.CriticalDamage = DataManager.InitEnemyData[Enemy[i]].CriticalDamage;
            data.Speed = DataManager.InitEnemyData[Enemy[i]].Speed;
            data.EffectHit = DataManager.InitEnemyData[Enemy[i]].EffectHit;
            data.EffectResist = DataManager.InitEnemyData[Enemy[i]].EffectResist;

            data.TempID = i;
            data.CurrentPos = obj.transform.position;
            EnemydataList.Add(data);
        }
        playerDatas = PlayerdataList;
        enemyDatas = EnemydataList;
    }

    /// <summary>
    /// 設置行動
    /// </summary>
    private void SetAllCharacterAction()
    {
        var list = playerDatas.Cast<ICharacterInfo>().ToList();
        list = list.Concat(enemyDatas).ToList();                    //  合併列表
        list = list.OrderByDescending(x => x.Speed).ToList();       //  降冪列表

        AllCharacterActionList = list;
    }

    private void EnemyAI()
    {
        var Skil = DataManager.EnemySkils[CurrentRoundChar.ID];
        int Rand = UnityEngine.Random.Range(0, Skil.Count);
                
        List<PlayerInfoData> targetList = new List<PlayerInfoData>();
        targetList = playerDatas.Where(x => !x.isDead).ToList();

        if (Skil[Rand].MoreTarget)
        {
            var list = targetList.ToArray();
            UpdateTargets(list);
            PlayCurrAnimation("Attack01", true);
        }
        else
        {
            //TODO 隨機選中目標 or 寫入AI邏輯
            int RandTarget = UnityEngine.Random.Range(0, targetList.Count);

            ICharacterInfo[] infos = { targetList[RandTarget] };
            UpdateTargets(infos);
            PlayCurrAnimation("Attack01", true);
        }
         SendEffect(Rand);
         StartCoroutine(PlayAnimationCoroutine());
    }


    public void MouseSelect(GameObject obj)
    {
        var Skil = DataManager.PlayerSkils[CurrentRoundChar.ID];

        //  Skil為MoreTarget也需觸發
        
        if (Skil[CurrentSkilID].MoreTarget)
        {
            SendEffect(CurrentSkilID);
            ClearSelectTarget();
            InitSkilID();
            StartCoroutine(fightView.DestroySkils());
        }
        else
        {
        
            for (int i = 0; i < EnemyListPos.Count; i++)
            {
                if (obj.gameObject == EnemyListPos[i].transform.GetChild(0).gameObject)
                {
                    List<EnemyInfoData> list = new List<EnemyInfoData>();
                    list = enemyDatas.Where(x => !x.isDead).ToList();
                    ICharacterInfo[] info = { list[i] };

                    var Select = info[0] as EnemyInfoData;
                    var tempSelect = TempSelect[0] as EnemyInfoData;
                    if (Select.TempID == tempSelect.TempID)
                    {
                        SendEffect(CurrentSkilID);
                        ClearSelectTarget();
                        StartCoroutine(fightView.DestroySkils());
                    }

                    /*
                    if (TempSelect[0] == info[0])
                    {
                        SendEffect(CurrentSkilID);
                        ClearSelectTarget();
                        StartCoroutine(fightView.DestroySkils());
                    }*/
                    
                    else
                    {
                        var select = info[0] as EnemyInfoData;
                        DestroyEffectVFX();
                        EffectsVFX(select.CurrentPos);

                        UpdateTargets(info);
                        TempSelect = info;
                    }
                    
                    
                    break;
                }

            }
        }
    }

    public void PlayerSelectTargets(int ID)
    {
        //  Defult is Skil01 and Target Wail be zero

        var Skil = DataManager.PlayerSkils[CurrentRoundChar.ID];
        List<EnemyInfoData> targetList = new List<EnemyInfoData>();
        targetList = enemyDatas.Where(x => !x.isDead).ToList();

        if (TempSelect == null)
        {
            ICharacterInfo[] temp = { targetList[0] };
            TempSelect = temp;
            UpdateTargets(TempSelect);
        }

        if (Skil[ID].MoreTarget)
        {
            UpdateTargets(targetList.ToArray());
            for (int i = 0; i < targetList.Count;i++)
            {
                EffectsVFX(targetList[i].CurrentPos);
            }
        }
        else
        {
            if (TempSelect == null)
            {
                var select = Targets[0] as EnemyInfoData;
                DestroyEffectVFX();
                EffectsVFX(select.CurrentPos);
                UpdateTargets(TempSelect);
            }
            else
            {
                var Temp = TempSelect[0] as EnemyInfoData;
                DestroyEffectVFX();
                EffectsVFX(Temp.CurrentPos);
                UpdateTargets(TempSelect);
            }
        }
    }

#region GameUI特效

    /// <summary>
    /// 設置選中目標特效
    /// </summary>
    /// <param name="target"></param>
    public void EffectsVFX(Vector3 targetPos)
    {
        GameObject targetVfx = Instantiate(SelectTarget_VFX);
        targetVfx.transform.position = new Vector3(targetPos.x,targetPos.y + 0.5f,targetPos.z);
        targetVfx.transform.SetParent(TargetVFX_Parent);
    }

    public void DestroyEffectVFX()
    {
        for (int i=0;i<TargetVFX_Parent.childCount;i++)
        {
            Destroy(TargetVFX_Parent.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 創建傷害數值
    /// </summary>
    /// <param name="damage">傷害值</param>
    /// <param name="target">目標</param>
    public void CreateUIAmount(float damage,ICharacterInfo target)
    {
        GameObject obj = PoolManager.Release(UIAmount);
        var DamageText = obj.GetComponent<SpawnDamageText>();
        DamageText.SetShowText(damage);

        if(target.GetType() == typeof(PlayerInfoData))
        {
            var Target = target as PlayerInfoData;
            obj.transform.position = Target.CurrentPos;
            DamageText.SetPosition(Target.CurrentPos);
            obj.transform.forward = Camera.main.transform.forward;
        }
        else if(target.GetType() == typeof(EnemyInfoData))
        {
            var Target = target as EnemyInfoData;
            obj.transform.position = Target.CurrentPos;
            DamageText.SetPosition(Target.CurrentPos);
            obj.transform.forward = Camera.main.transform.forward;
        }
    }

#endregion

    /// <summary>
    /// 對象受到效果
    /// </summary>
    public void ReceiveEffect(float damage)
    {
        foreach (var target in Targets)
        {
            target.GetDamageAmount(damage);
            
            CreateUIAmount(damage, target);
        }
    }
    /// <summary>
    /// 給予影響效果
    /// </summary>
    public void SendEffect(int SkilID)
    {
        //CurrentRoundChar.DoSkils(CurrentRoundChar, SkilID);
        SetCurrentSkilID(SkilID);
    }

    public void SendAttackEvent(int SkilID)
    {
        CurrentRoundChar.DoSkils(CurrentRoundChar, SkilID);
    }

    /// <summary>
    /// 自身回合開始設置該角色到指定位置
    /// </summary>
    private void SetCurrentPosToTarget()
    {

        for (int i=0;i<PlayerListPos.Count;i++)
        {
            if (PlayerListPos[i].transform.childCount >= 1 && CurrentRoundChar.Name == PlayerListPos[i].transform.GetChild(0).name)
            {
                PlayerListIndex = i;

                GameObject CurrentObj = PlayerListPos[i].transform.GetChild(0).gameObject;
                CurrentObj.transform.SetParent(CurrentPlayerPos.transform);
                CurrentObj.transform.localPosition = Vector3.zero;
                CurrentObj.transform.localRotation = Quaternion.identity;
                CurrentObj.transform.localScale = Vector3.one;

                break;
            }
        }
    }
    /// <summary>
    /// CineMachine控制
    /// </summary>
    /// <param name="目標相機開關"></param>
    public void CurrCameraController(CinemachineVirtualCamera _camera, bool EnableCamera)
    {
        _camera.enabled = EnableCamera;
    }
    public void FightCameraEnable(bool enableCamera)
    {
        FightViewCamera.enabled = enableCamera;
    }

    /// <summary>
    /// <para>我方or敵方陣亡時傳 false 布林值，反之則Loop繼續</para>
    /// </summary>
    /// <returns> 我方陣亡or敵方陣亡 </returns>
    public bool JudgeGameLoop(List<ICharacterInfo> Chars)
    {
        int DeadAmount = 0;

        for (int i=0;i<Chars.Count;i++)
        {
            if (Chars[i].isDead)
            {
                DeadAmount++;
            }
        }
        
        if (DeadAmount >= Chars.Count)
            return true;
        else
            return false;
    }


}

public enum GameFlow
{
    RoundStart,
    RoundOver,
    NewTurn,
    GameOver,
}
