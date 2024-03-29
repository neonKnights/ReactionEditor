﻿using UnityEngine;

namespace pkg.generic_serializable_dictionary.Scripts
{
    /// <summary>
    /// An example of using the generic dictionary with a MonoBehaviour.
    /// </summary>
    public class Example : MonoBehaviour
    {
        // Simply declare the key/value types, zero boilerplate.
        public GenericDictionary<string, GameObject> myGenericDict;

        void Start()
        {
            // Runtime test
            string keyToCheck = "abc";
            bool contains = this.myGenericDict.ContainsKey(keyToCheck);
            Debug.LogFormat("myGenericDict contains '{0}': {1}", keyToCheck, contains);
        }

        void Update()
        {
            // Runtime test showing that the inspector reflects runtime additions.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                string newKey = "runtime example";
                this.myGenericDict.Add(newKey, this.gameObject);
                Debug.LogFormat("Added '{0}' to myGenericDict.", newKey);
            }
        }
    }
}
