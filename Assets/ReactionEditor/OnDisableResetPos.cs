using UnityEngine;

namespace Game.Town.ReactionEditor
{
    public class OnDisableResetPos : MonoBehaviour
    {
        private Vector3 _origPosition;
        private void Awake()
        {
            this._origPosition = this.gameObject.transform.position;
        }

        private void OnDisable()
        {
            this.gameObject.transform.position = this._origPosition;
        }
    }
}
