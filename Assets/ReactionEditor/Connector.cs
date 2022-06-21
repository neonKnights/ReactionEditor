using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Town.ReactionEditor
{
    public class Connector :
        MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private RectTransform line;

        private Vector3 startPos;
        [SerializeField]
        private GameObject ReactionEditor;
        [SerializeField]
        private GameObject linePrefab;
        [SerializeField]
        private LayerMask Mask;

        [SerializeField]
        private bool allowConnectingMe;

        public bool allowConnectingToMe;
    
        private PointerEventData mouseDat;
        private List<RaycastResult> rayResults;
        [SerializeField]
        private GraphicRaycaster graphicsRaycast;

        private void Awake()
        {
            this.line.gameObject.SetActive(false);
            this.mouseDat = new PointerEventData(EventSystem.current);
            this.rayResults = new List<RaycastResult>();
            this.graphicsRaycast.blockingMask=this.Mask;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!this.allowConnectingMe)
            {
                return;
            }
        
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }

            if (FindObjectsOfType<ArrowControl>().Count(x => x.origin.Equals(this.gameObject)) > 0)
            {
                return;
            }

            this.line.gameObject.SetActive(true);
            this.startPos = this.transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!this.allowConnectingMe)
            {
                return;
            }
        
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }
        
            // [calc rotation]
            var deltaX = this.startPos.x - eventData.position.x;
            var deltaY = this.startPos.y - eventData.position.y;
            var rotationZ = Mathf.Atan(deltaY / deltaX) * Mathf.Rad2Deg;
            // prevent pointer from reversing position
            rotationZ = (deltaX > 0) ? rotationZ - 180 : rotationZ;
            var newRotation = Quaternion.Euler(0, 0, rotationZ);
            this.line.rotation = newRotation;

            // [calc len]
            var len = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
            this.line.sizeDelta = new Vector2(len, this.line.sizeDelta.y);

            // [calc pos]
            var pos = this.startPos - new Vector3(
                deltaX / 2, deltaY / 2, 0
            );

            this.line.position = pos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!this.allowConnectingMe)
            {
                return;
            }
        
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }

            this.line.gameObject.SetActive(false);
            this.mouseDat.position = Input.mousePosition;
            EventSystem.current.RaycastAll(this.mouseDat, this.rayResults);

            var hitObject = this.rayResults.Where(hit => hit.gameObject.name.Contains("Alchemist") || hit.gameObject.name.Contains("SubstanceGiver") || hit.gameObject.name.Contains("FireField")).Select(hit => hit.gameObject).FirstOrDefault();

            if (!hitObject)
            {
                return;
            }

            // disallow recursive arrows
            var controller = this.GetComponent<ArrowTrace>();
            if (controller && controller.arrowList.Select(x => x.GetComponent<ArrowControl>()).Where(x => x != null)
                    .Where(x => x.target.Equals(this.gameObject)).Where(x => x.origin.Equals(hitObject)).ToArray()
                    .Length > 0)
            {
                return;
            }
            
            var connector = hitObject.GetComponent<Connector>();
            if (connector && !connector.allowConnectingToMe)
            {
                return;
            }
        
            var breadcrumb = hitObject.GetComponent<ArrowTrace>();
            if (breadcrumb == null)
            {
                breadcrumb = hitObject.AddComponent<ArrowTrace>();
                breadcrumb.arrowList = new List<GameObject>();
            }

            GameObject arrow = Instantiate(this.linePrefab, Vector3.zero, Quaternion.identity, this.ReactionEditor.transform);
            ArrowControl arrowController = arrow.GetComponent<ArrowControl>();
            arrow.transform.SetSiblingIndex(1);
            arrowController.origin = this.gameObject;
            arrowController.target = hitObject;
            arrowController.updateArrow();
            if (!breadcrumb.isInList(arrow))
            {
                breadcrumb.arrowList.Add(arrow);
                hitObject.BroadcastMessage("updateReactions", null, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Destroy(arrow);
            }

            // foreach (GameObject iarrow in breadcrumb.arrowList)
            // {
            //     Debug.Log(iarrow);
            //     ArrowControl iarrowcontroller = iarrow.GetComponent<ArrowControl>();
            //     Debug.Log(iarrowcontroller.origin.GetInstanceID()==arrowController.target.GetInstanceID());
            //     Debug.Log(iarrowcontroller.origin);
            //     Debug.Log(arrowController.origin);
            //     Debug.Log(iarrowcontroller.target.GetInstanceID()==arrowController.origin.GetInstanceID());
            //     if (iarrowcontroller.origin==arrowController.target&&iarrowcontroller.target==arrowController.origin)
            //     {
            //         Destroy(arrow);
            //     }
            // }

            this.rayResults.Clear();
        }
    }
}
