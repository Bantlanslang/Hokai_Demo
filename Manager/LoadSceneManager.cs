using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : UnitySingleton<LoadSceneManager>
{
    public LobbyUIView lobby;


    public void LoadScene(SceneEnum sceneEnum)
    {
        if (SceneManager.GetActiveScene().name == "Entry")
        {
            StartCoroutine(IEntryToLogin(sceneEnum));
        }
        else
        {
            StartCoroutine(IEntryLoadScene(sceneEnum));
        }
        
    }
    public void LoadLobbyScene(SceneEnum sceneEnum, LobbyUIEnum lobbyUIEnum)
    {
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            StartCoroutine(IEntryToLobbyUI(sceneEnum, lobbyUIEnum));
        }
    }


    /// <summary>
    /// 載入登入場景
    /// </summary>
    /// <param name="sceneEnum"></param>
    /// <returns></returns>
    public IEnumerator IEntryToLogin(SceneEnum sceneEnum)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneEnum.ToString());
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                Debug.Log($"載入{sceneEnum} 場景完成");
                yield return new WaitForSeconds(0.1f);
                /*
                if (Input.GetMouseButtonDown(0))        //  點擊任意位置進入遊戲
                {
                    async.allowSceneActivation = true;
                }
                */
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 載入瀏海功能欄UI
    /// </summary>
    /// <param name="sceneEnum"></param>
    /// <returns></returns>
    public IEnumerator IEntryToLobbyUI(SceneEnum sceneEnum,LobbyUIEnum lobbyUIEnum)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneEnum.ToString());
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.1f);
                async.allowSceneActivation = true;
            }
            yield return null;
        }

        lobby = FindObjectOfType<LobbyUIView>();
        lobby.CreateView(lobbyUIEnum);
    }

    public IEnumerator IEntryLoadScene(SceneEnum sceneEnum)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneEnum.ToString());
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.1f);
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }


    /// <summary>
    /// 判斷進入場景
    /// </summary>
    /// <param name="sceneEnum"></param>
    private void JudgeScene(SceneEnum sceneEnum)
    {
        switch (sceneEnum)
        {
            case SceneEnum.LobbyScene:

                break;
            case SceneEnum.LoginScene:

                break;
        }

        
    }
}
