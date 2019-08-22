using UnityEngine;
using System.Collections;

/// <summary>
/// 由于实测数据显示7个createPlayer里的Monobehavior.AddComponent()导致40%的cpu消耗
/// 
/// !!严重注意，游戏项目内，OnAdd()里注意把DataComp _data要置为null,因为有组件缓存的泄漏问题
/// 
/// </summary>
public abstract class SpriteComp:TEventDispatcher  {

    //引用，供各个state使用
    protected TSprite sprite;

    public void Init(TSprite p_sprite)
    {
        sprite = p_sprite;
    }

    public abstract void OnAdded();

	public abstract void OnRemoved();

	public T GetSpriteComp<T>() where T:SpriteComp,new (){
		if(sprite != null){
			return sprite.GetComp<T>();
		}
		return null;
	}

    public Transform GetCacheContainer()
    {
        if (sprite != null)
        {
            return sprite.transform;
        }
        return null;
    }

}
