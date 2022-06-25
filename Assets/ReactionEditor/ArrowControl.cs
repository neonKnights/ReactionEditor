using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Town.ReactionEditor
{
    public class ArrowControl : MonoBehaviour
    {
        [NonSerialized]
        public GameObject origin;
        [NonSerialized]
        public GameObject target;

        private RectTransform rect;
        private int multip, add;


        private Vector2 scale;

        private void Awake()
        {
            this.rect = this.gameObject.GetComponent<RectTransform>();
            var currentResolution = GetComponentInParent<Canvas>().renderingDisplaySize;
            var designedSize = this.GetComponentInParent<CanvasScaler>().referenceResolution;
            this.scale = designedSize / currentResolution;
        }

        public void updateArrow()
        {
            var delta = new Vector2(
                this.origin.transform.position.x*this.scale.x - this.target.transform.position.x*this.scale.x,
                this.origin.transform.position.y*this.scale.y - this.target.transform.position.y*this.scale.y);
        
            var originModifier = GetLengthModifier(delta, this.origin.GetComponent<RectTransform>());
            var targetModifier = GetLengthModifier(delta, this.target.GetComponent<RectTransform>());
            //[calculate position]
            this.gameObject.transform.position = new Vector3(
                (this.origin.transform.position.x + originModifier.x + this.target.transform.position.x - targetModifier.x) / 2,
                (this.origin.transform.position.y + originModifier.y + this.target.transform.position.y - targetModifier.y) / 2,
                0);
            //[calculcate rotation]
            //[set rotation]
            var rotationZ = Mathf.Atan(delta.y / delta.x) * Mathf.Rad2Deg;
            // prevent pointer from reversing position
            rotationZ = (delta.x > 0) ? rotationZ - 180 : rotationZ;
            var newRotation = Quaternion.Euler(0, 0, rotationZ);
            this.gameObject.transform.rotation = newRotation;
            //[calculate length]
            var arrowSize = this.rect.sizeDelta;
            arrowSize.x = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);

            if (originModifier.magnitude< delta.magnitude)
            {
                arrowSize.x -= originModifier.magnitude;
            }
        
            if (targetModifier.magnitude< delta.magnitude)
            {
                arrowSize.x -= targetModifier.magnitude;
            }

            //[set length]
            this.rect.sizeDelta = arrowSize;
        }

        // GetLengthModifier returns distance from center off rectangle w, h to its edge.
        // basing on x,y - delta of length between two points (used to calculate angle)
        public static Vector2 GetLengthModifier(float x, float y, float w, float h)
        {
            var result = Vector2.zero;
            var alpha0 = Mathf.Atan(h / w) * Mathf.Rad2Deg;
            var alpha = Mathf.Atan(y / x) * Mathf.Rad2Deg;
            var d = Mathf.Sqrt(x * x + y * y);
            if (
                (alpha > -alpha0 && alpha < alpha0) ||
                (alpha > 180 - alpha0 && alpha < 180 + alpha0)
            )
            {
                result = new Vector2((x < 0? 1 : -1) * w/2, (x < 0? 1:-1) *(y * w) / (2 * x));
                return result;
            }

            result = new Vector2((y < 0 ? 1 : -1) * (x * h) / (2 * y), (y < 0? 1:-1)*h/2);
            return result;
        }

        public static Vector2 GetLengthModifier(Vector2 delta, Vector2 rectSize)
        {
            return GetLengthModifier(delta.x, delta.y, rectSize.x, rectSize.y);
        }

        public static Vector2 GetLengthModifier(Vector2 delta, RectTransform rect)
        {
            return GetLengthModifier(delta, rect.rect.size);
        }
    }
}
