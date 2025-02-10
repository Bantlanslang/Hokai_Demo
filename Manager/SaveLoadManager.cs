using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System;
using System.IO;
using Newtonsoft.Json.Converters;
using System.Data.Common;
using Newtonsoft.Json.Serialization;


public class SaveLoadManager : UnitySingleton<SaveLoadManager>
{

    public void Save(SaveLoadEnum saveLoad)
    {
        switch (saveLoad)
        {
            case SaveLoadEnum.InitPlayerData:
                ISaveData(DataManager.InitPlayerData, "InitPlayerData.json");
                break;
            case SaveLoadEnum.PlayerInfoData:
                ISaveData(DataManager.playerInfos, "PlayerInfoData.json");
                break;
            case SaveLoadEnum.InventoryStoreData:
                ISaveData(DataManager.InventoryStore,"InventoryStoreData.json");
                break;
        }
    }

    public void Load(SaveLoadEnum saveLoad)
    {
        switch (saveLoad)
        {
            case SaveLoadEnum.InitPlayerData:
                DataManager.InitPlayerData = ILoadData(DataManager.InitPlayerData,"InitPlayerData.json");
                break;
            case SaveLoadEnum.PlayerInfoData:
                DataManager.playerInfos = ILoadData(DataManager.playerInfos,"PlayerInfoData.json");
                break;
            case SaveLoadEnum.InventoryStoreData:
                DataManager.InventoryStore = ILoadData(DataManager.InventoryStore, "InventoryStoreData.json");
                break;
            case SaveLoadEnum.EnemyInfoData:
                DataManager.InitEnemyData = ILoadData(DataManager.InitEnemyData, "EnemyInfoData.json");
                break;
        }
    }


    private void ISaveData<T>(T saveData ,string dataName)
    {
        string dataPath = Application.persistentDataPath;
        string FullPath = Path.Combine(dataPath,dataName);

        try
        {
            Directory.CreateDirectory(Path.GetFileName(FullPath));

            //  JsonConvert 處理Vector3時出現異常
            //  使用"JsonSerializerSettings"處理自我循環錯誤
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(saveData,Formatting.Indented,settings);
            
            using (FileStream stream = new FileStream(FullPath,FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error!! When trying to save data to file"+ FullPath +"\n"+ e);
        }
    }
    private T ILoadData<T>(T LoadData,string dataName)
    {
        string dataPath = Application.persistentDataPath;
        string FullPath = Path.Combine(dataPath, dataName);

        if (File.Exists(FullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(FullPath,FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                LoadData = JsonConvert.DeserializeObject<T>(dataToLoad);
                return LoadData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error!! When trying to save data to file" + FullPath + "\n" + e);
            }
        }
        return default(T);
    }
}

public enum SaveLoadEnum
{
    InitPlayerData,
    PlayerInfoData,
    InventoryStoreData,
    EnemyInfoData,
}
