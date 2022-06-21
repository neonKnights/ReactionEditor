using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Town.ReactionEditor
{
    public class MouseDragBehaviourSource : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector2 lastMousePosition;
    
        private RectTransform rect;
    
        [SerializeField]
        private GameObject prefab;
    
        [SerializeField]
        private GameObject ReactionEditor;
        private Transform parent;
    
        public bool stopCloning;
    
        private void Awake()
        {
            this.rect = this.GetComponent<RectTransform>();
            this.parent = this.ReactionEditor.transform;
        }

        /// <summary>
        /// This method will be called on the start of the mouse drag
        /// </summary>
        /// <param name="eventData">mouse pointer event data</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(eventData.button==PointerEventData.InputButton.Left){
                if(!this.stopCloning){
                    Instantiate(this.prefab, this.rect.position,Quaternion.identity, this.parent);
                    this.stopCloning=true;
                }
            
                this.lastMousePosition = eventData.position;
            }

        }
    
        /// <summary>
        /// This method will be called during the mouse drag
        /// </summary>
        /// <param name="eventData">mouse pointer event data</param>
        public void OnDrag(PointerEventData eventData)
        {
            if(eventData.button==PointerEventData.InputButton.Left){
                var currentMousePosition = eventData.position;
                var diff = currentMousePosition - this.lastMousePosition;
                var newPosition = this.rect.position +  new Vector3(diff.x, diff.y, this.transform.position.z);
            
                this.rect.position = newPosition;
                if(!IsRectTransformInsideScreen(this.rect))
                {
                    this.rect.position = new Vector3(
                        Mathf.Clamp(this.rect.position.x, 0, Screen.width),
                        Mathf.Clamp(this.rect.position.y, 0, Screen.height),
                        0
                    );
                }

                this.lastMousePosition = currentMousePosition;
                this.ReactionEditor.BroadcastMessage("updateArrow", null, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// This method will be called at the end of mouse drag
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
        
        }

        /// <summary>
        /// This methods will check is the rect transform is inside the screen or not
        /// </summary>
        /// <param name="rectTransform">Rect Trasform</param>
        /// <returns></returns>
        private static bool IsRectTransformInsideScreen(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            var rect = new Rect(0,0,Screen.width, Screen.height);

            var visibleCorners = corners.Count(corner => rect.Contains(corner));
        
            return visibleCorners == 4;
        }
    }
}
