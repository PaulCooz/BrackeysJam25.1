using UnityEngine;
using UnityEngine.Events;

namespace JamSpace
{
    public class TriggerStateProvider2D : MonoBehaviour
    {
        [SerializeField] private string triggerTagName = "Player";
        [SerializeField] private bool oneTimeTrigger;
        [SerializeField] private UnityEvent onTriggerEnterAction;
        
        private bool _isTriggered = false;
        private GameObject _triggeredGameObject;
        
        public bool IsTriggered => _isTriggered;
        public GameObject TriggeredGameObject => _triggeredGameObject;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(triggerTagName))
            {
                _triggeredGameObject = other.gameObject;
                _isTriggered = true;
                onTriggerEnterAction?.Invoke();

                if (oneTimeTrigger)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(triggerTagName))
            {
                _triggeredGameObject = null;
                _isTriggered = false;
            }
        }

        private void OnDisable()
        {
            _triggeredGameObject = null;
            _isTriggered = false;
        }
    }
}