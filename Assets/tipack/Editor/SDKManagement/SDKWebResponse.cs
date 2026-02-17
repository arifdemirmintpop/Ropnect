using System;
using Newtonsoft.Json;
using UnityEngine;

namespace tiplay.SDKManagement
{
    [Serializable]
    public class SDKWebResponse
    {
        [SerializeField] private SDKData elephantGameKit;
        [SerializeField] private SDKData externalDependency;
        [SerializeField] private SDKData facebook;
        [SerializeField] private SDKData gameAnalytics;
        [SerializeField] private SDKData elephant;
        [SerializeField] private SDKData tibug;

        public SDKData ElephantGameKit => elephantGameKit;
        public SDKData ExternalDependency => externalDependency;
        public SDKData Facebook => facebook;
        public SDKData GameAnalytics => gameAnalytics;
        public SDKData Elephant => elephant;
        public SDKData Tibug => tibug;
    }
}