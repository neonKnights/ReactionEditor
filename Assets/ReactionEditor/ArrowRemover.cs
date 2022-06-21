using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Town.ReactionEditor
{
    public class ArrowRemover : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject _reactionEditor;

        private ArrowControl _control;

        private void Start()
        {
            this._control = this.gameObject.GetComponent<ArrowControl>();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }
            
            this.Remove();
        }

        public void Remove()
        {
            var trace = this._control.target.GetComponent<ArrowTrace>();
            trace?.arrowList.Remove(this.gameObject);

            var reactionDisplayer = this._control.target.GetComponent<ReactionDisplayer>();
            reactionDisplayer?.updateReactions();
            
            Destroy(this.gameObject);
        }

        private void OnDisable()
        {
            Destroy(this.gameObject);
        }
    }
}
