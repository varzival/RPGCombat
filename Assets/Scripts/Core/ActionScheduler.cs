using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction currentAction;

        private float ignoreActionChangeTime = 0.2f;
        private float timeSinceLastAction = 0f;

        public void StartAction(IAction action)
        {
            if (currentAction == action || timeSinceLastAction < ignoreActionChangeTime)
                return;
            if (currentAction != null)
            {
                currentAction.Cancel();
                timeSinceLastAction = 0f;
                Debug.Log($"canceled action {currentAction}");
            }
            currentAction = action;
            Debug.Log($"started action {action}");
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }

        private void Update()
        {
            timeSinceLastAction += Time.deltaTime;
        }
    }
}
