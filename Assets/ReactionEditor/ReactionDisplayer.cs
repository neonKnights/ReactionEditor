using System;
using System.Collections.Generic;
using System.Linq;
using Game.Reactions;
using TMPro;
using UnityEngine;

namespace Game.Town.ReactionEditor
{
    public class ReactionDisplayer : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown dropdown;
        private ReactionController reactionController;
        private Stack<List<Substance>> reactionResults;
        public List<Substance> availableSubstances;
        private ArrowTrace arrows;
        private List<Substance> availableSubstancesScaled;

        private void Awake()
        {
            this.reactionController = FindObjectOfType<ReactionController>();
            this.availableSubstancesScaled = new List<Substance>();
        }

        public void updateReactions(){
            this.arrows = this.gameObject.GetComponent<ArrowTrace>();
            
            if (this.arrows == null)
            {
                return;
            }

            foreach (var arrowController in this.arrows.arrowList.Select(arrow => arrow.GetComponent<ArrowControl>()))
            {
                string substancesInText =
                    arrowController.origin.GetComponentInChildren<TMP_Dropdown>().captionText.text;
                foreach (String substance in substancesInText.Split('+'))
                {
                    this.availableSubstances.Add(Substance.FromString(substance));
                }
            }
            
            var tempSubstances = new List<Substance>();
            foreach (var substance in this.availableSubstances)
            {
                if (!tempSubstances.Select(s => s.molecule).ToList().Contains(substance.molecule))
                {
                    tempSubstances.Add(substance);
                    continue;
                }

                foreach (var t in tempSubstances.Where<Substance>(t => t.molecule.Equals(substance.molecule)))
                {
                    t.count += substance.count;
                    break;
                }
            }

            this.availableSubstances = tempSubstances;

            this.availableSubstances.Sort();
            this.reactionResults = this.reactionController.GetReactionResult(this.availableSubstances);
            this.availableSubstances.Clear();
            this.dropdown.ClearOptions();
            var outputList = new List<TMP_Dropdown.OptionData>();
            
            foreach (List<Substance> result in this.reactionResults)
            {
                var output = result.Aggregate("", (current, sub) => current + (sub + "+"));
                output = output.Remove(output.Length - 1);
                var optionData = new TMP_Dropdown.OptionData(output);
                outputList.Add(optionData);
            }
            
            this.dropdown.options = outputList;
        }
    }
}
