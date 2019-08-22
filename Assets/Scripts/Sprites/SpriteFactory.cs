using UnityEngine;
using System.Collections;

using System;
using Tgame.Game.Table;

public class SpriteFactory
{
    public static T Create<T>(CreatData p_data) where T:TSprite,new ()
    {
        Table_role t_role=Table_role.GetPrimary(p_data.role_id);
        T t_sprite= SpritesManager.instance.CreateSprite<T>(t_role.avatar_id, 5,0);
        t_sprite.GetComp<DataComp>().AddData(p_data);
        t_sprite.Startup();
        return t_sprite;
    }

}