using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZframeModel.ZEvent.Sprites;
using ZFrame;

public class MinLoadScene : MonoBehaviour
{
    private AsyncOperation async = null;

  
    /// <summary>
    /// 最小的时间
    /// </summary>
    public float m_minTime = 2f;

    private float m_startTime=3;

    private  SafeTimer m_timer=new SafeTimer();

    private void Start()
    {

        StartCoroutine("LoadScene");
    }

    private float progressValue;
    IEnumerator LoadScene()
    {
        m_timer.Start(m_startTime);
        async = SceneManager.LoadSceneAsync(MapManager.instance.m_nextSceneId, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            progressValue = async.progress +(m_timer.Percentage())*0.1f;
            if (progressValue < 1f)
            {
                //progressValue = async.progress;
            }
            else
            {
                async.allowSceneActivation = true;
                SceneManager.UnloadSceneAsync("midScene");
                Frame.DispatchEvent(MEFactory.New<ME_SwitchMapOK>());
            }

           
            yield return null;
        }
       
    }

}
