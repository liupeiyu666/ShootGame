using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BehaviorDesigner.Custom
{
    [TaskDescription("检测角色属性")]
    [TaskCategory("自定义/检测属性")]
    [TaskIcon("{SkinColor}HasReceivedEventIcon.png")]
    public class CheckSpriteData : Conditional
    {
        [Runtime.Tasks.Tooltip("The name of the event to receive")]
        public SharedInt m_spriteID ;
        /// <summary>
        /// 获取到数据组件
        /// </summary>
        private DataComp m_dataComp;
        public override void OnStart()
        {
            TSprite t_sprite = SpritesManager.instance.GetSprite(m_spriteID.Value);
            if (t_sprite!=null)
            {
                m_dataComp = t_sprite.m_data;
            }
            Debug.LogError(gameObject.name);
           
        }

        public override TaskStatus OnUpdate()
        {
            return true ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnEnd()
        {
           
            
        }


        public override void OnBehaviorComplete()
        {
        }

        public override void OnReset()
        {
            
        }
    }
}