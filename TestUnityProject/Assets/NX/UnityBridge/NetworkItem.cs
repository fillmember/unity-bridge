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

        public void OnData(object input) {
            if (input is string) {
                Hashtable payload = (Hashtable)Procurios.Public.JSON.JsonDecode((string)input);
                gameObject.SendMessage("OnDetailUpdate", payload, SendMessageOptions.DontRequireReceiver);
            } else if (input is Hashtable) {
                gameObject.SendMessage("OnDetailUpdate", input, SendMessageOptions.DontRequireReceiver);
            }
        }

    }

}
