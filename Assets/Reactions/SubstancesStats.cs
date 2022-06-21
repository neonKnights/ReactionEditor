using System;
using System.Linq;
using pkg.generic_serializable_dictionary.Scripts;
using UnityEngine;
using UnityEngine.Windows;

namespace Game.Reactions
{
    public class SubstancesStats : MonoBehaviour
    {
        [Serializable]
        public struct stats
        {
            public int damage;
            public int heal;
            public int life;
        }

        [SerializeField]
        public GenericDictionary<string, stats> SubstanceStats;
    
        private void Reset()
        {
#if UNITY_EDITOR
            var file = File.ReadAllBytes("Assets/Game/Reactions/Resources/stats.csv");
            var text = file.Aggregate("", (current, c) => current + (char)c);
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                if (line[0] == '/' && line[1] == '/')
                {
                    continue;
                }
                
                var currentLine = line.Trim(' ');
                var tiles = currentLine.Split(',');
                var newStats = new stats()
                {
                    damage = int.Parse(tiles[1]),
                    heal = int.Parse(tiles[2]),
                    life = int.Parse(tiles[3]),
                };

                this.SubstanceStats[tiles[0]] = newStats;
            }
#endif
        }
    }
}
