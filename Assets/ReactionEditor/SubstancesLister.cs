using TMPro;
using UnityEngine;

namespace Game.Town.ReactionEditor
{
    public class SubstancesLister : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown _dropdown;
    
        private void Awake()
        {
            this.UpdateList();
        }

        private void UpdateList()
        {
            this._dropdown.options.Clear();
        
            foreach (var substance in GameData.playersSubstances)
            {
                this._dropdown.options.Add(new TMP_Dropdown.OptionData(substance.ToString()));
            }
        }
    }
}
