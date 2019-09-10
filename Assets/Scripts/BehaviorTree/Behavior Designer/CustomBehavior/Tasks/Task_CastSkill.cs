using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
namespace BehaviorDesigner.Custom
{
    [TaskCategory("自定义/任务")]
    [TaskDescription("Finds a GameObject by tag. Returns Success.")]
    public class Task_CastSkill : Action
    {
        [UnityEngine.Tooltip("The tag of the GameObject to find")]
        public SharedString tag;
        [UnityEngine.Tooltip("The objects found by name")]
        [RequiredField]
        public SharedGameObjectList storeValue;

        public override TaskStatus OnUpdate()
        {
            var gameObjects = GameObject.FindGameObjectsWithTag(tag.Value);
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                storeValue.Value.Add(gameObjects[i]);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            tag.Value = null;
            storeValue.Value = null;
        }
    }
}
