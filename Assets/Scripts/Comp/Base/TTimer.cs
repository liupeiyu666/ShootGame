using System; 
using System.Collections.Generic;
using System.Text;

    using UnityEngine;
    using System.Collections;
    using System; 
    using System.IO; 
    /**
 * 计时器
 */
    public class TTimer
    {
        public float curtime;
        public float fTime;
        public void SetTime(float v)
        {
            fTime = v;
            curtime = 0;
        }
        public TTimer(float time)
        {
            fTime = time;
            curtime = 0;
        }
        public void Update(float v)
        {
            curtime += v;
        }
        public TTimer()
        {
            curtime = 1.0f;
            fTime = 1.0f;
        }
        public void Clear()
        {
            curtime = fTime;
        }
        public bool End()
        {
            return curtime >= fTime;
        }
        public void Reset()
        {
            curtime = 0;
        }
        public bool isReady()
        {
            return End();
        }
        public float GetPercent()
        {
            return Mathf.Max(0, Mathf.Min(curtime / fTime, 1.0f));
        }
    }
