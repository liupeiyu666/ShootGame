using System.Collections;
using ZFrame;
using System.Collections.Generic;

public class MapModule : Module
{
    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new MapProcessor(this),
        };
    }
}
