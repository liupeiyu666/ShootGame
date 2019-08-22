using System.Collections;
using ZFrame;
using System.Collections.Generic;

public class GameModule : Module
{
    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new GameStartProcessor(this),
        };
    }
}