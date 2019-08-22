using System.Collections;
using ZFrame;
using System.Collections.Generic;

public class SpriteModule : Module
{
    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new SpritesProcessor(this)
        };
    }
}