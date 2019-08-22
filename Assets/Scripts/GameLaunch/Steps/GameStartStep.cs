using System.Collections.Generic;
/// <summary>
/// 启动游戏
/// </summary>
public class GameStartStep:BaseStep
{
    public GameStartStep(List<string> p_BeforeStepEndCommandList, List<string> p_NextStepsStartCommandList) : base(p_BeforeStepEndCommandList, p_NextStepsStartCommandList)
    {
    }
    public override bool CheckStepCanStart()
    {
        return true;
    }


}