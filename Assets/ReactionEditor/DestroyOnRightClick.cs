using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Town.ReactionEditor
{
    public class DestroyOnRightClick : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject target;
        private MouseDragBehaviourSource sleepStatus;
        private bool _selfDestruct;

        private void Awake()
        {
            this.sleepStatus = this.target.GetComponent<MouseDragBehaviourSource>();
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right || !this.sleepStatus.stopCloning)
            {
                return;
            }

            var arrows = FindObjectsOfType<ArrowControl>()
                .Where(x => (x.target.Equals(this.target) || x.origin.Equals(this.target)))
                .Select(x => x.GetComponent<ArrowRemover>());

            foreach (var arrow in arrows)
            {
                arrow.Remove();
            }

            Destroy(this.target);
        }
        public void OnDisable()
        {
            if (this.sleepStatus.stopCloning)
            {
                Destroy(this.target);
            }
        }
    }
}
