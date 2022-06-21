using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Town.ReactionEditor
{
    public class DropdownEnabler : MonoBehaviour
    {
        [SerializeField]
        private GameObject Dropdownsource;
        private TMP_Dropdown Dropdown;
        private ColorBlock colorBlock;
        private void Awake(){
            this.Dropdown = this.Dropdownsource.GetComponent<TMP_Dropdown>();
            this.colorBlock = this.Dropdown.colors;
            this.colorBlock.colorMultiplier = 0;
            this.Dropdown.colors = this.colorBlock;
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.LeftShift)||Input.GetKeyDown(KeyCode.RightShift)){
                this.Dropdown.interactable = true;
                this.colorBlock.colorMultiplier = 1;
                this.Dropdown.colors = this.colorBlock;
            }
            if(Input.GetKeyUp(KeyCode.LeftShift)||Input.GetKeyUp(KeyCode.RightShift)){
                this.Dropdown.interactable=false;
                this.colorBlock.colorMultiplier = 0;
                this.Dropdown.colors = this.colorBlock;
            }
        }
    }
}
