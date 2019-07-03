using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    [Serializable]
    public class CameraOffsetComponent:CameraComponent
    {
        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);
            OnOffset = SetOffset;
        }
        /// <summary>
        /// 摄像机移动时的偏移函数
        /// </summary>
        public Action<Vector3> OnOffset;
        //x2/a2+x2/b2=1
        public float m_circleA = 2;
        public float m_circleB = 1;

        public float m_Speed = 1;
        /// <summary>
        /// 当前的偏移值
        /// </summary>
        private Vector3 m_offset=Vector3.zero;

        private void SetOffset(Vector3 p_offset)
        {
            Vector3 t_temp = m_offset + p_offset.normalized*Time.deltaTime* m_Speed;

            //检测是否可以继续偏移
            //使用椭圆计算公式
            float t_rad = Mathf.Pow(t_temp.x,2) / Mathf.Pow(m_circleA,2) +
                          Mathf.Pow(t_temp.z,2) / Mathf.Pow(m_circleB,2);
            if (t_rad < 1)
            {
                m_offset = t_temp;
            }
            else
            {
                //说明已经出界了，那么就要寻找对应的轨迹了，简易的切边行走了

            }
        }
        /// <summary>
        /// 记录上一次获取的偏移值
        /// </summary>
        private Vector3 m_recordOffset;
        /// <summary>
        /// 获取偏移数据
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCameraOffset()
        {

            //获取差值
            Vector3 t_tempOffset =Vector3.Lerp(m_recordOffset,m_offset,1) ;
            m_recordOffset = t_tempOffset;
            return m_recordOffset;
        }
    }
}
