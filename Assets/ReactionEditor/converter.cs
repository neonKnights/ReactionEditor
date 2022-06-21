using Game.Reactions;
using TMPro;
using UnityEngine;

namespace Game.Town.ReactionEditor{
    public class converter : MonoBehaviour
    {
        public Substance substance;
        void Awake()
        {
            TextMeshPro text = this.gameObject.GetComponent<TextMeshPro>();
            text.text = this.substance.ToString();
        }
    }
}
