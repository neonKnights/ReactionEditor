using System.Collections.Generic;
using UnityEngine;

namespace Game.Town.ReactionEditor
{
    public class ArrowTrace : MonoBehaviour
    {
        public List<GameObject> arrowList;
        public bool isInList(GameObject target){
            ArrowControl targetController = target.GetComponent<ArrowControl>();
            foreach (GameObject arrow in this.arrowList){
                ArrowControl arrowController = arrow.GetComponent<ArrowControl>();
                if (arrowController.origin == targetController.origin && arrowController.target == targetController.target){
                    return true;
                }
            }
            return false;
        }
    }
}
