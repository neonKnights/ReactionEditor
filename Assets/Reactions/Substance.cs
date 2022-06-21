using System;
using UnityEngine;

namespace Game.Reactions
{
    [Serializable]
    public class Substance : IComparable<Substance>, IEquatable<Substance> {
        public int count;
        public string molecule;

        public Substance(int count, string molecule)
        {
            this.count = count;
            this.molecule = molecule;
        }

        public override string ToString()
        {
            if (this.count == 0)
            {
                Debug.LogError("error: substance count cannot be 0, invalid reaction, invalid substance, fatal error");
                return "N/A";
            }
            
            var result = "";
            
            if (this.count > 1)
            {
                result += this.count.ToString();
            }

            var isSub = false;
            foreach (var c in this.molecule)
            {
                if (c >= '0' && c <= '9')
                {
                    result += c;
                }
                else
                {
                    if (isSub)
                    {
                        result += "</sub>";
                    }

                    isSub = true;
                    result += c + "<sub>";
                }
            }
            
            if (isSub)
            {
                result += "</sub>";
            }

            return result;
        }
        
        // FromString allows to revere ToString operation.
        // so ( substance == Substance.FromString(substance.ToString()) )
        public static Substance FromString(string input)
        {
            var molecule = "";
            
            // separate count
            var countStr = "";
            foreach (var c in input)
            {
                if (c >= '0' && c <= '9')
                {
                    countStr += c;
                }
                else
                {
                    break;
                }
            }
            
            var count = countStr.Length > 0 ? int.Parse(countStr) : 1;
            
            // separate molecule
            molecule = input.Substring(countStr.Length);
            
            // trim HTML tags
            molecule = molecule.Replace("</sub>", "");
            molecule = molecule.Replace("<sub>", "");
            
            return new Substance(count, molecule);
        }

        public int CompareTo(Substance other)
        {
            // Debug.Log("this: " + this);
            // Debug.Log("other" + other);
            // Debug.Log(this.molecule.CompareTo(other.molecule));
            return this.molecule.CompareTo(other.molecule);
        }

        public bool Equals(Substance other)
        {
            return other.count == this.count && other.molecule == this.molecule;
        }
    }
}
