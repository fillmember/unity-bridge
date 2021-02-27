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
            UnityBridgeManager.Watch(
                this.system.GetEventName( this.system.detailEventTemplate , transform.name ),
                transform,
                "OnData"
            );
        }

        void OnDestroy() {
            UnityBridgeManager.Unwatch(
                this.system.GetEventName( this.system.detailEventTemplate , transform.name ),
                transform,
                "OnData"
            );
        }

        public void OnData(string payload) {
            gameObject.SendMessage("OnDetailUpdate", (Hashtable)Procurios.Public.JSON.JsonDecode(payload));
        }

    }

}
