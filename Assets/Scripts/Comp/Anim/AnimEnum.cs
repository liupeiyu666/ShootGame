using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AnimEnum
{
    //普通待机
    public const string STAND = "stand";
    public const string FIGHT_STAND = "ready";                  //战斗内待机


    public const string RUN_FORWARD = "run_forward";    //行走
    public const string RUN_F_R = "run_forward_right";
    public const string RUN_RIGHT = "run_right";
    public const string RUN_B_R = "run_back_right";
    public const string RUN_BACK = "run_back";
    public const string RUN_F_L = "run_forward_left";
    public const string RUN_LEFT = "run_left";
    public const string RUN_B_L = "run_back_left";


    public const string DIE = "die";               //死亡倒地动画
    public const string DEATH = "death";                //静态死尸

    public const string COLLECT = "collect";    //采集-常规

    //这几个是主角用的
    public const string ATTACK_1 = "attack1";
    public const string ATTACK_2 = "attack2";
    public const string ATTACK_3 = "attack3";

}

