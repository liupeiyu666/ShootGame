namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("The sequence task is similar to an \"and\" operation. It will return failure as soon as one of its child tasks return failure. " +
                     "If a child task returns success then it will sequentially run the next task. If all child tasks return success then it will return success.")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=25")]
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class Sequence : Composite
    {
        // The index of the child that is currently running or is about to run.
        private int currentChildIndex = 0;
        // The task status of the last child ran.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }
        /// <summary>
        /// 是否可以继续
        /// </summary>
        /// <returns></returns>
        public override bool CanExecute()
        {
            // We can continue to execuate as long as we have children that haven't been executed and no child has returned failure.
            return currentChildIndex < children.Count && executionStatus != TaskStatus.Failure;
        }
        /// <summary>
        /// 当一个子节点执行完成后
        /// </summary>
        /// <param name="childStatus"></param>
        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Increase the child index and update the execution status after a child has finished running.
            currentChildIndex++;
            executionStatus = childStatus;
        }
        /// <summary>
        /// 当条件被打断
        /// </summary>
        /// <param name="childIndex"></param>
        public override void OnConditionalAbort(int childIndex)
        {
            // Set the current child index to the index that caused the abort
            currentChildIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }
        /// <summary>
        /// 结束
        /// </summary>
        public override void OnEnd()
        {
            // All of the children have run. Reset the variables back to their starting values.
            executionStatus = TaskStatus.Inactive;
            currentChildIndex = 0;
        }
    }
}