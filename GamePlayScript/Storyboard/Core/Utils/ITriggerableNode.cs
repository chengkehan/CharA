using System;

namespace StoryboardCore
{
    public interface ITriggerableNode
    {
        // When a trigger is complete immediatelly, invoke completeCallback at the end of TriggerOn function.
        // For those asynchronous operation, invoke complateCallback after asynchronous operation is complete.
        // If you don't invoke completeCallback, thread will be waiting forever.
        void TriggerOn(Action completeCallback);
    }
}
