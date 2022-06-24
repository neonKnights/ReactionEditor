using System.Collections.Generic;
using System.Linq;
using pkg.generic_serializable_dictionary.Scripts;
using UnityEngine;

/*
 * Attach this script to any game object,
 * on the scene, and now you can make
 * an object in your script, call it whatever
 * you want, add reference in editor. Tada!
 * now you can call GetReactionResultas many times
 * as you want!
 */
namespace Game.Reactions
{
    public class ReactionController : MonoBehaviour
    {
        [SerializeField]
        GenericDictionary<List<Substance>,List<Substance>> ReactionRegister;
        
        Stack<List<Substance>> result;

        private void OnValidate()
        {
            var file = Resources.Load("reactions") as TextAsset;
            if (file == null)
            {
                Debug.LogError("File not loaded!");
                return;
            }

            var text = file.text;
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                if (line.Length < 2)
                {
                    return;
                }
                
                if (line[0] == '/' && line[1] == '/')
                {
                    continue;
                }
                
                var currentLine = line;
                currentLine = currentLine.Replace(" ", string.Empty);
                var splited = currentLine.Split(',');
                var substrats = splited[0];
                var substratsList = substrats.Split('+').Select(s => Substance.FromString(s)).ToList();
                substratsList.Sort();
                var results = splited[1];
                var resultsList = results.Split('+').Select(s => Substance.FromString(s)).ToList();
                resultsList.Sort();
                this.ReactionRegister.Add(substratsList, resultsList);
            }
        }

        private void Awake()
        {
            var file = Resources.Load("Assets/Game/Manager/Resources/reactions.txt");
            Debug.Log(file);
        }
        
        // @Garnn co to robi???
        private static List<Substance> SimplifyList(List<Substance> list)
        {
            return list.Select(substance => new Substance(1, substance.molecule)).ToList();
        }

        private static bool IsSublist(List<Substance> list1, List<Substance> list2)
        {
            var simpleList1 = SimplifyList(list1);
            var simpleList2 = SimplifyList(list2);
            simpleList1.Sort();
            simpleList2.Sort();
            foreach (var substance in simpleList1) {
                    if (!simpleList2.Contains(substance)) {
                        return false;
                    }
            }
            return true;
        }

        private static List<Substance> MultiplyReaction(List<Substance> list, int multiplier)
        {
            var outputList = new List<Substance>();
            
            foreach (var sub in list)
            {
                outputList.Add(new Substance(sub.count*multiplier,sub.molecule));
            }

            return outputList;
        }

        private static int findFirstOccurenceIndex(List<Substance> list, Substance substance)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (substance.molecule==list[i].molecule)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public Stack<List<Substance>> GetReactionResult(List<Substance> Substrates)
        {
            Debug.Log("---");
            
            var reactions = new List<List<Substance>>();
            var finalReactions = new Stack<List<Substance>>();
            var reactionKeys = new List<List<Substance>>();
            var simpleList = SimplifyList(Substrates);
            
            // searching for available reactions with substrates given
            foreach(var reaction in this.ReactionRegister)
            {
                if (!IsSublist(reaction.Key, Substrates))
                {
                    continue;
                }

                reactions.Add(reaction.Value);
                reactionKeys.Add(reaction.Key);
            }
            
            // do not lead more than one reactions
            // select reactions to lead
            for (var i = 0; i < reactionKeys.Count; i++)
            {
                var reactionKey = reactionKeys[i];
                var multiplier = Mathf.Infinity;
                foreach (var substance in reactionKey)
                {
                    var index = findFirstOccurenceIndex(Substrates, substance);
                    
                    var hypotheticalMultiplier = (float)Substrates[index].count / (float)substance.count;
                    if (hypotheticalMultiplier < multiplier)
                    {
                        multiplier = hypotheticalMultiplier;
                    }
                }

                if (Mathf.Floor(multiplier) == 0)
                {
                    continue;
                }
                
                var multipliedReaction = MultiplyReaction(reactions[i], (int)multiplier);
                finalReactions.Push(multipliedReaction);
            }

            // determine which of reactions to use
            // for each of found reactions,
            // for (var x = 0; x < reactionKeys.Count; x++)
            // {
                // assign reactions key to var
                // var key = reactionKeys[x];
                
                // set smallest?? to infinity
                // var smallest = Mathf.Infinity;

                // iterate through substrates
                // for (var i = 0; i < Substrates.Count; i++)
                // {
                    // try
                    // {
                        // var comparedSubstance = key[findFirstOccurenceIndex(key, Substrates[i])];
                        // if ((float)Substrates[i].count/comparedSubstance.count < smallest)
                        // {
                            // smallest = (float)Substrates[i].count / key[i].count;
                        // }
                    // }
                    // catch
                    // {
                        // ignored
                    // }
                // }
                // if (float.IsPositiveInfinity(smallest))
                // {
                    // smallest = 0;
                // }

                // smallest = Mathf.Floor(smallest);
                // Debug.Log(smallest);
                // if (smallest != 0)
                // {
                    // finalReactions.Push(MultiplyReaction(reactions[x], (int)smallest));
                // }
            // }
            
            return finalReactions;
        }
    }
}
