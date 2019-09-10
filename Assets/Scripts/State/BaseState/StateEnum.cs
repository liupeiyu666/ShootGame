using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StateEnum
{
    public static int IDLE = 1;

    public static int DIR_MOVE = 2;

    public static int CAST_SKILL = 3;
}

public class FSMEvent
{
    public const string EVENT_FSM_STATE_COMPLETE = "EVENT_FSM_STATE_COMPLETE";
}