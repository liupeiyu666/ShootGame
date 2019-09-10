﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZframeModel.ZEvent.Sprites;
using ZFrame;

public class MapManager  {
    #region 单例

    private MapManager()
    {
    }

    private static MapManager _instance;

    public static MapManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MapManager();
            }

            return _instance;
        }
    }

    #endregion

    #region 场景切换

    private string m_midSceneID = "midScene";
    private AsyncOperation m_asyncOperation;
    public int m_nextSceneId;
    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="p_sceneID"></param>
    /// <param name="p_haveMid">是否有中间过渡的场景</param>
    public void LoadScene(int p_sceneID,bool p_haveMid=true)
    {
        m_nextSceneId = p_sceneID;
        if (p_haveMid)
        {
            SceneManager.LoadScene(m_midSceneID, LoadSceneMode.Additive);  //U533以上
           
        }
        else
        {
            SceneManager.LoadScene(p_sceneID, LoadSceneMode.Additive);  //U533以上
            Frame.DispatchEvent(MEFactory.New<ME_SwitchMapOK>());
        }
       
    }

    public void UnloadScene(int p_sceneID)
    {
        SceneManager.UnloadSceneAsync(p_sceneID);
    }

    #endregion
    /// <summary>
    /// 如果返回为-1000说明没有高度，换句话说就是不能移动
    /// </summary>
    /// <param name="p_pos"></param>
    /// <returns></returns>
    public float GetHeight(Vector3 p_pos)
    {
        float t_height = -1000;
        Ray ray = new Ray();
        ray.origin = new Vector3(p_pos.x, 1000.0f, p_pos.z);
        ray.direction = new Vector3(0.0f, -1.0f, 0.0f);

        RaycastHit hit;
        LayerMask nl = 1 << EngineLayerEnum.Ground;
        if (Physics.Raycast(ray, out hit, 1500.0f, nl))
        {
            float ret = (1000.0f - hit.distance);
          //  Debug.LogError(hit.transform.name+"  "+Time.frameCount);
            return ret;
        }
        return t_height;
    }

    public float GetHeight(int x,int y)
    {
        return GetHeight(new Vector3(x,0,y));
    }
    public float GetHeight(float x, float y)
    {
        return GetHeight(new Vector3(x, 0, y));
    }
    public Vector3 GetHitNormal(Vector3 p_pos,Vector3 p_dir)
    {
       
        Ray ray = new Ray();
        ray.origin = p_pos;
        ray.direction = p_dir;

        RaycastHit hit;
        LayerMask nl = 1 << EngineLayerEnum.Block;
        if (Physics.Raycast(ray, out hit, 1f, nl))
        {
            return hit.normal;
        }
        return Vector3.zero;
    }

    public bool CanMove(Vector3 p_pos)
    {
        bool t_canMove = false;
        Ray ray = new Ray();
        ray.origin = new Vector3(p_pos.x, 1000.0f, p_pos.z);
        ray.direction = new Vector3(0.0f, -1.0f, 0.0f);

        RaycastHit hit;
        LayerMask nl = 1 << EngineLayerEnum.Ground;
        if (Physics.Raycast(ray, out hit, 1500.0f, nl))
        {
            t_canMove = true;
        }
        return t_canMove;
    }
}
