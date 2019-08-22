using System.Collections;
using ZFrame;
using System.Collections.Generic;

public class CameraModule : Module
{
    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new CameraProcessor(this)
        };
    }
}