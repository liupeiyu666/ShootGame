using UnityEngine;
using System.Collections;
using ZFrame;
using System.Collections.Generic;

public class GameLaunchModel : Module
{
    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new GameLaunchProcessor(this),
            new LoadTableMetaProcessor(this),
        };
    }

}