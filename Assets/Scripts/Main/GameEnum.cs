using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnum  {


}
public class EngineLayerEnum
{
    /// <summary>
    /// 忽略摄像层
    /// </summary>
    public static int IgnoreRaycast = 2;
  
    /// <summary>
    /// HERO
    /// </summary>
    public static int Hero = 10;//这是为了Hero处理
    /// <summary>
    /// NPC
    /// </summary>
    public static int Monster = 11;
    /// <summary>
    /// 不可移动
    /// </summary>
    public static int Block = 12;
    /// <summary>
    /// 地面
    /// </summary>
    public static int Ground = 13;
}
