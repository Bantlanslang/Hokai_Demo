using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : UnitySingleton<AssetManager>
{
    Dictionary<SpriteAlbumEnum,SpriteAlbum> SpriteAlbumDic = new Dictionary<SpriteAlbumEnum, SpriteAlbum>();

    Dictionary<ModelAlbumEnum,ModelAlbum> ModelAlbumDic = new Dictionary<ModelAlbumEnum, ModelAlbum>();

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 載入圖集資源
    /// </summary>
    /// <returns></returns>
    public IEnumerator ILoadSpriteAsset()
    {
        
        foreach (var album in Enum.GetValues(typeof(SpriteAlbumEnum)))
        {
            ResourceRequest request = Resources.LoadAsync<SpriteAlbum>($"SpriteAlbum/{album}");
            yield return request;
            SpriteAlbum obj = request.asset as SpriteAlbum;
            SpriteAlbumDic.Add((SpriteAlbumEnum)album, obj);
        }
    }
    /// <summary>
    /// 獲取圖集資源
    /// </summary>
    public SpriteAlbum GetSpriteAssets(SpriteAlbumEnum spriteAlbumEnum)
    {
        if (SpriteAlbumDic.ContainsKey(spriteAlbumEnum))
        {
            return SpriteAlbumDic[spriteAlbumEnum];
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("無法找到 ScriptableObject_SpriteData 物件");
#endif
            return null;
        }
    }
    
    public SpriteAlbum GetInventoryAssets(InventoryData data)
    {
        SpriteAlbumEnum SpriteAlbumEnum = 0;

        switch (data.Name)
        {
            case InventoryNameEnum.Scholar:
                SpriteAlbumEnum = SpriteAlbumEnum.Scholar_InventoryAlbum;
                break;
            case InventoryNameEnum.Priest:
                SpriteAlbumEnum = SpriteAlbumEnum.Priest_InventoryAlbum;
                break;
        }
        return SpriteAlbumDic[SpriteAlbumEnum];
        
    }

    /// <summary>
    /// 載入人物模型資源
    /// </summary>
    /// <returns></returns>
    public IEnumerator ILoadModelAsset()
    {
        foreach (var album in Enum.GetValues(typeof(ModelAlbumEnum)))
        {
            ResourceRequest request = Resources.LoadAsync<ModelAlbum>($"ObjectAlbum/{album}");
            yield return request;
            ModelAlbum obj = request.asset as ModelAlbum;
            ModelAlbumDic.Add((ModelAlbumEnum)album,obj);
        }
    }

    public ModelAlbum GetModelAssets(ModelAlbumEnum modelAlbumEnum)
    {
        if (ModelAlbumDic.ContainsKey(modelAlbumEnum))
        {
            return ModelAlbumDic[modelAlbumEnum];
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("無法找到 ScriptableObject_SpriteData 物件");
#endif
            return null;
        }
    }


    public void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
}
