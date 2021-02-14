using System.Collections;
using UnityEngine;
using NX.UnityBridge;
using NX.UnityBridge.Types;

namespace NX.UnityBridge {

    public class NetworkItem : MonoBehaviour
    {

        public NetworkSystem system;

        void Start()
        {
            UnityBridgeManager.unityWatch(
                this.system.GetEventName( this.system.detailEventTemplate , transform.name ),
                transform.name,
                "OnData"
            );
        }

        void OnDestroy() {
            UnityBridgeManager.unityUnwatch(
                this.system.GetEventName( this.system.detailEventTemplate , transform.name ),
                transform.name,
                "OnData"
            );
        }

        public void OnData(string payload) {
            gameObject.SendMessage("OnDetailUpdate", (Hashtable)Procurios.Public.JSON.JsonDecode(payload));
        }

    }

}
