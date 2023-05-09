using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Convai.ConvaiAPIKey
{
    [CreateAssetMenu(fileName = "ConvaiAPIKey", menuName = "Convai/API Key")]
    public class ConvaiAPIKeySetup : ScriptableObject
    {
        public string APIKey;
    }
}