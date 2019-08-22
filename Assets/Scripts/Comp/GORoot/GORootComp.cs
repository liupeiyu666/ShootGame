using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
 

///<summary>
/// 对层级嵌套的各个go的快捷引用,方便找到对应的go,避免再调用GetComponent<>
/// 用法参见UGEDemo的PlayerFactory.cs
/// 
/// 某个角色的游戏对象的嵌套层级结构
/// FS_root
/// 	  bodyContainer
/// 		bodyctry 包含各个动画
/// 			BIP001 骨骼动画，包含组右手挂接点
/// 			face 皮肤
/// 			hair
/// 			armor
/// 			leg
/// 		hud
///    weaponContainer
/// 		weaponA_Bip001_Prop1
/// 		   weaponGO
///	  mountContainer
///		bodyctrl
///			BIP001
/// </summary>
public class GORootComp : SpriteComp
{
	#region 基本
	public override void OnAdded(){
		//需要mainLoadercomp
	    if (GetSpriteComp<MainLoaderComp>() == null)
	    {
           Debug.LogError("GoRootComp需要MainLoaderComp");
	    }
	}

	public override void OnRemoved(){
		
	}
    #endregion

    #region GoRoot
    /// <summary>
    /// 精灵的各个部位构成,
    /// zlf 20160926 修改过 GO 改 trm
    /// </summary>
    public Transform 
        bodyContainerTrm,
        bodyctrlTrm,
        hudTrm;
    /// <summary>
    /// 存储所有挂点的信息
    /// </summary>
    private  Dictionary<int,Transform> m_bindPosDic=new Dictionary<int, Transform>();
    /// <summary>
    /// 添加body,如果是player,加的是bone_anim,如果是怪，加的是全套
    /// </summary>
    /// <param name="p_go">加载到的精灵主体gameObject</param>
    /// <returns></returns>
    public void Start()
    {

        bodyContainerTrm = GetSpriteComp<MainLoaderComp>().m_body.transform;

        bodyContainerTrm.gameObject.SetActive(true);

        //把新加载到的挂在原来的go下面
        UGEUtils.SetParent(bodyContainerTrm, m_containerTrans);

        //动画组件
        bodyctrlTrm = bodyContainerTrm.GetChild(0);
        //设置hud
        hudTrm = bodyctrlTrm.Find("bone_hud");
        //记录挂点
        BindPositon[] t_bindArry = bodyContainerTrm.GetComponentsInChildren<BindPositon>();
        for (int i = 0; i < t_bindArry.Length; i++)
        {
            m_bindPosDic.Add(t_bindArry[i].m_bindIndex,t_bindArry[i].transform);
        }
    }
    /// <summary>
    /// 移除body
    /// </summary>
    public void RemoveBody()
    {
        bodyContainerTrm = null;

        bodyctrlTrm = null;

        hudTrm = null;
    }
    #endregion

    #region 对外方法

    public Transform GetBindPos(int p_index)
    {
        if (m_bindPosDic.ContainsKey(p_index))
        {
            return m_bindPosDic[p_index];
        }

        return null;
    }


    #endregion
    #region 快捷引用

    private Transform _containerTrans;

    private Transform m_containerTrans {
        get
        {
            if (_containerTrans==null)
            {
                _containerTrans = GetCacheContainer();
            }
            return _containerTrans;
        }
    }
    #endregion
}