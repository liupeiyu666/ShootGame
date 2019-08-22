using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpritesManager
{
    #region 单例

    private SpritesManager()
    {
        m_spriteContainer=new GameObject("Sprites").transform;
    }

    private static SpritesManager _instance;

    public static SpritesManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SpritesManager();
            }

            return _instance;
        }
    }

    #endregion

    public Transform m_spriteContainer;
    /// <summary>
    /// Sprite字典,供特殊情况扫描
    /// Dictionary<TSprite.id,TSprite>
    /// </summary>
    Dictionary<long, TSprite> _spriteDict = new Dictionary<long, TSprite>();

    //全体精灵
    public Dictionary<long, TSprite> spriteDict
    {
        get { return _spriteDict; }
    }

    //精灵类型的容器Dictionary[TSprte.type]=Dictionary<TBaseSprite.id,TBaseSprite>
    Dictionary<int, Dictionary<long, TSprite>> _spriteTypeDict = new Dictionary<int, Dictionary<long, TSprite>>();

    #region hero相关

    //hero引用单独保留,因为Clear的时候不会删除
    TSprite _hero = null;

    /// <summary>
    /// 设置hero,注意有可能是null
    /// </summary>
    /// <param name="p_hero">hero</param>
    public TSprite hero
    {
        get
        {
            return _hero;
        }
        set
        {
            _hero = value;
            if (_hero != null)
            {
                _hero.isHero = true;
            }
        }
    }

    /// <summary>
    /// 卸载Hero,这个是在每次进入选角色界面的时候用
    /// </summary>
    public void UnloadHero()
    {
        if (_hero != null)
        {
            _hero.isHero = false;
            DestroySprite(_hero.tID);
            _hero = null;
        }
    }

    #endregion

    //TODO prefabName要去掉
    /// <summary>
    /// 通过prebab name创建Sprite
    /// </summary>
    /// <param name="id">sprite id</param>
    /// <param name="type">sprite type,比如NPC, Monster, DropItem, CollectItem</param>
    /// <param name="prefabName">精灵prefab Name,位于Resources/BaseObject文件夹下面</param>
    /// <returns>创建成功的sprite</returns>
    /// 
    /// TODO LF t_layer写好赋值给go.layer
    public T CreateSprite<T>(long id, int type, int layer) where T : TSprite, new()
    {
        //if (bShutdown)return null;

        //这样会爆发严重问题，因为我们是延迟销毁,如果在一个Update里销毁一个对象，并且再创建一个同id对象，则会报告出错.因为此时并没真正销毁
        //为了简化引擎结构，不做额外特殊处理
        if (GetSprite(id) != null)
        {
            //TTrace.LogError("sprsMgr.CreaeteSprite,之前没销毁:" + id + " " + type);
            tobeRemoveSpriteIDList.Remove(id);
            return GetSprite(id) as T;
        }
        if (_spriteTypeDict.ContainsKey(type) == false)
        {
            _spriteTypeDict[type] = new Dictionary<long, TSprite>();
        }

        //创建go
        T sprite =new T();
        UGEUtils.SetParent(sprite.transform, m_spriteContainer);
        sprite.InitBase(type, id, layer);
        //设置sprite

        //将创建的sprite放入各个数据容器
        _spriteDict[id] = sprite;
        _spriteTypeDict[type][id] = sprite;

        return sprite;
    }

    //这些会在lateUpdate里进行销毁
    List<long> tobeRemoveSpriteIDList = new List<long>();

    ///<summary>
    /// 命令销毁一个sprite,并从容器中注销
    ///immediately是说立即销毁还是带淡出动画的
    /// </summary>
    public void DestroySprite(long id)
    {

        var sprite = GetSprite(id);
        if (sprite != null)
        {
            tobeRemoveSpriteIDList.Add(id);
        }
    }

    //2016 0926 kk 改，改成了TryGetValue 有更好的效率
    void RealDestroySprite(long id)
    {
        int type = -1;
        TSprite sprite;
        if (_spriteDict.TryGetValue(id, out sprite))
        {

            type = sprite.tType;

            _spriteDict.Remove(id);

            //真正销毁
            sprite.tDispose();
        }

        Dictionary<long, TSprite> tempDict;
        if (_spriteTypeDict.TryGetValue(type, out tempDict))
        {
            if (tempDict.ContainsKey(id))
            {
                tempDict.Remove(id);
            }
        }
    }

    /// <summary>
    /// 在LateUpdate里执行精灵清理，因为精灵大部分逻辑都在Update里，有少量外显相关在LateUpdate里
    /// 注意这里做了字典删除的操作
    /// </summary>
    public void Purge()
    {
        //
        int len = tobeRemoveSpriteIDList.Count;
        if (len > 0)
        {
            for (int i = 0; i < len; i++)
            {
                var spr = GetSprite(tobeRemoveSpriteIDList[i]);
                if (spr != null)
                {
                    RealDestroySprite(spr.tID);
                }
            }
            //
            tobeRemoveSpriteIDList.Clear();
        }
    }

    /// <summary>
    /// 销毁全部Sprite,不包括hero
    /// 注意这些不是立即销毁，这些是在tLateUpdate的时候被真正销毁的，DestoryObject()和删除字典
    /// </summary>
    public void Clear(bool destroyHero = false)
    {
        //TTrace.p ("SpriteManager.Clear", destroyHero);
        var list = new List<TSprite>();
        foreach (var sprite in _spriteDict.Values)
        {
            list.Add(sprite);
        }

        foreach (var sprite in list)
        {
            if (destroyHero == false && sprite.isHero)
            {

            }
            else
            {
                //请注意 true参数,在clear的时候，是要立刻销毁的，注意，此事有可能有的sprite还在DelayDestory().【这是可以的，因为最终会被销毁】
                DestroySprite(sprite.tID);
            }
        }

        //2016 0629 kk- 无效
        //tobeRemoveSpriteIDList.Clear ();
    }

    /// <summary>
    /// 通过id获取sprite
    /// </summary>
    /// <param name="id">sprite id</param>
    /// <returns>获取sprite</returns>
    public TSprite GetSprite(long id)
    {
        TSprite spr;
        _spriteDict.TryGetValue(id, out spr);

        return spr;
    }

    //取得某一类型的对象
    public Dictionary<long, TSprite> GetSprites(int type)
    {


        if (_spriteTypeDict.ContainsKey(type))
        {
            return _spriteTypeDict[type];

        }

        return null;
    }

    /// <summary>
    /// 	根据提供的回调函数，获得符合条件的精灵
    /// </summary>
    /// <param name="func">用于确定是否是目标精灵的回调函数</param>
    /// <param name="type">type=-1表示无类型查找,扫描全体精灵</param>
    /// <returns>目标sprite</returns>
    public TSprite Find(Func<TSprite, bool> func, int type = -1)
    {

        if (type == -1)
        {
            foreach (var sprite in _spriteDict.Values)
            {
                if (func(sprite))
                {
                    return sprite;
                }
            }
        }
        else
        {
            if (_spriteTypeDict.ContainsKey(type))
            {
                var dict = _spriteTypeDict[type];
                foreach (var sprite in dict.Values)
                {
                    if (func(sprite))
                    {
                        return sprite;
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 	根据提供的回调函数，获得所有符合条件的目标精灵
    /// </summary>
    /// <param name="func">用于确定是否是目标精灵的回调函数</param>
    /// <param name="type">type=-1表示无类型查找,扫描全体精灵</param>
    /// <returns>目标sprites</returns>
    public List<TSprite> FindSprites(Func<TSprite, bool> func, int type = -1)
    {

        var result = new List<TSprite>();
        if (type == -1)
        {
            foreach (var sprite in _spriteDict.Values)
            {
                if (func(sprite))
                {
                    result.Add(sprite);
                }
            }
        }
        else
        {
            if (_spriteTypeDict.ContainsKey(type))
            {
                var dict = _spriteTypeDict[type];
                foreach (var sprite in dict.Values)
                {
                    if (func(sprite))
                    {
                        result.Add(sprite);
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 将精灵作为参数，执行一遍提供的回调函数
    /// </summary>
    /// <param name="action">对精灵执行某种操作的回调函数</param>
    /// <param name="type">type=-1表示无类型查找,扫描全体精灵</param>
    public void ForEach(Action<TSprite> action, int type = -1)
    {
        if (type == -1)
        {
            foreach (var sprite in _spriteDict.Values)
            {
                action(sprite);
            }
        }
        else
        {
            if (_spriteTypeDict.ContainsKey(type))
            {
                var dict = _spriteTypeDict[type];
                foreach (var sprite in dict.Values)
                {
                    action(sprite);
                }
            }
        }
    }
}
