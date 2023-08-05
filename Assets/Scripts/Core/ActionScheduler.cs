using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction currentAction;

        public void StartAction(IAction action)
        {
            if (currentAction == action)
                return;
            if (currentAction != null)
            {
                currentAction.Cancel();
                Debug.Log($"canceled action {currentAction}");
            }
            currentAction = action;
            Debug.Log($"started action {action}");
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
