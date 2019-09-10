using System;
using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

///<summary>
/// Sprite父类
/// 作为一个门面，与外界交互,接受外部命令，分派给各个具体组建
/// 两类函数组成,
/// </summary>
/// <remarks>TODO KK  注意1028,目前有变身变大变小的buff</remarks>
public class TSprite
{
    #region 基础信息

    #region 创建信息
    public long tID;

    public int tType;

    public int m_layer;

    public GameObject gameObject;

    public Transform transform;
    #endregion

    #region 组件容器
    /// <summary>
    /// 所有的组件容器，这里我们限定同一类型只能加一个，因为目前来看，加多个的需求暂时没有
    /// 仿照unity的组件式开发的方法
    /// </summary>
    private Dictionary<Type,SpriteComp> m_allComp=new Dictionary<Type, SpriteComp>();
    /// <summary>
    /// 添加comp，当有这个组件的时候，直接返回这个组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddComp<T>() where T:SpriteComp,new ()
    {
        if (m_allComp.ContainsKey(typeof(T)))
        {
            return (T)m_allComp[typeof(T)];
        }
        T t_temp=new T();
        m_allComp.Add(typeof(T),t_temp);
        t_temp.OnAdded();
        t_temp.Init(this);
        return t_temp;
    }
    public void RemoveComp<T>() where T : SpriteComp, new()
    {
        if (m_allComp.ContainsKey(typeof(T)))
        {
            m_allComp[typeof(T)].OnRemoved();
            m_allComp.Remove(typeof(T));
        }
    }
    /// <summary>
    /// 获取组件，如果没有是否需创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="p_noneCreat"></param>
    /// <returns></returns>
    public T GetComp<T>(bool p_noneCreat=true) where T : SpriteComp, new()
    {
        if (m_allComp.ContainsKey(typeof(T)))
        {
            return (T)m_allComp[typeof(T)];
        }
        else
        {
            if (p_noneCreat)
            {
                return AddComp<T>();
            }
            return null;
        }
    }

    #endregion




    #region 基础模块--位置信息模块。其余 的全部自己去添加

    public SpacialComp m_spacialComp;

    public DataComp m_data;
    #endregion

    public bool isHero = false;




    #endregion

    #region 周期函数

    public TSprite(){
        gameObject = new GameObject();
        gameObject.name = "TSprite_WaitUse";
        transform = gameObject.transform;
        Debug.LogError("+++++++++++_______");
        InitComp();
    }
    /// <summary>
    /// 用于外部调用，初始化数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <param name="layer"></param>
    public void InitBase(int type, long id, int layer)
    {
        this.tID = id;
        tType = type;
        m_layer = layer;
    }
    /// <summary>
    /// 用于子类初始化组件使用
    /// </summary>
    protected virtual void InitComp()
    {
        m_spacialComp = AddComp<SpacialComp>();
        m_data = AddComp<DataComp>();
    }

    /// <summary>
    /// 构造完成(数据，组件，状态机设置完成)后启动
    /// </summary>
    public virtual void Startup()
    {

        //设置层级
        UGEUtils.SetLayers(gameObject, m_layer, true);
        Debug.LogError(m_data.GetData<CreatData>().m_posx+"    "+ m_data.GetData<CreatData>().m_posz);
        //设置位置
        m_spacialComp.ResetPosi(m_data.GetData<CreatData>().m_posx, m_data.GetData<CreatData>().m_posz);
    }


    public virtual void tDispose()
    {
        GameObject.Destroy(gameObject);
    }
    #endregion

    #region 对外开放
    /// <summary>
    /// 根据传入的节点信息，获取对应的transform，目前用于子弹发射，由于特效的挂点可能多种
    /// 比如，主角的子弹发射挂点在有武器（可以切换）的时候是武器的挂点，没有的时候可能使用自身的挂点，比如怪物
    /// </summary>
    /// <returns></returns>
    public virtual Transform GetBindPos(int p_index)
    {
        return null;
    }


    #endregion
}
