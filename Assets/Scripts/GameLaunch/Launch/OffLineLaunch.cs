using System.Collections.Generic;
using ZFrame;

public class OffLineLaunch : BaseLaunch
{
    public override void AddLaunchStep()
    {
        //启动游戏-启动uge
        m_launchStepList.Add(new GameStartStep(new List<string>() { LaunchStepCommands.Start_LaunchUnityPlay },
            new List<string>() { LaunchStepCommands.Start_LaunchLoadTable }));//
        
        m_launchStepList.Add(new CommonStep(new List<string>() { LaunchStepCommands.End_LaunchLoadTable },
            new List<string>() { LaunchStepCommands.Start_GamePlay }));
    }

    public override void Start()
    {
        Frame.DispatchEvent(LaunchStepCommands.Start_LaunchUnityPlay,null);
    }
}