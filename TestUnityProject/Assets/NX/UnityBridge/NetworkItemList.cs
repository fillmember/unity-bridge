using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NX.UnityBridge;

namespace NX.UnityBridge {

    public class NetworkItemList : MonoBehaviour
    {

        public string objectType = "Objects";
        private string eventTemplate = "%SCENE%/%TYPE%/list";

        // private UnityBridgeEventBaseStructure parsedPayload;

        string GetUnityBridgeEventName() {
            return this.eventTemplate
                .Replace("%SCENE%", SceneManager.GetActiveScene().name)
                .Replace("%TYPE%", this.objectType);
        }

        // Start is called before the first frame update
        void Start()
        {
            string name = this.GetUnityBridgeEventName();
            UnityBridgeManager.unityWatch( name , transform.name, "OnData" );
        }

        void OnData(string payload) {

            // JsonUtility.FromJsonOverwrite(payload, parsedPayload);
            // Debug.Log(parsedPayload);

        }

    }

}
