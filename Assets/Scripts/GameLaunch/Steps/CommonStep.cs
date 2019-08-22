using System.Collections.Generic;
/// <summary>
/// 通用的步骤，所有的CheckStepCanStart都是直接为true
/// </summary>
public class CommonStep : BaseStep
{
    public CommonStep(List<string> p_BeforeStepEndCommandList, List<string> p_NextStepsStartCommandList) : base(p_BeforeStepEndCommandList, p_NextStepsStartCommandList)
    {
    }
    public override bool CheckStepCanStart()
    {
        return true;
    }


}