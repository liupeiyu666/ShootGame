using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Basic
{
    [TaskCategory("自定义/任务")]
    [TaskDescription("Finds a GameObject by tag. Returns Success.")]
    public class Task_CastSkill : Action
    {
        [Tooltip("The tag of the GameObject to find")]
        public SharedString tag;
        [Tooltip("The objects found by name")]
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
