using System.Collections;
using UnityEngine;
using NX.UnityBridge;
using NX.UnityBridge.Types;

namespace NX.UnityBridge {

    public class NetworkItemList : MonoBehaviour
    {

        public NetworkSystem system;

        void Start()
        {
            UnityBridgeManager.unityWatch(
                this.system.GetEventName( this.system.listEventTemplate ),
                transform.name,
                "OnData"
            );
        }

        void OnDestroy() {
            UnityBridgeManager.unityUnwatch(
                this.system.GetEventName( this.system.listEventTemplate ),
                transform.name,
                "OnData"
            );
        }

        public void OnData(string payload) {

            Hashtable parsedJson = (Hashtable)Procurios.Public.JSON.JsonDecode(payload);

            string action = (string)parsedJson["action"];
            Hashtable data = (Hashtable)parsedJson["data"];

            if (action == "child_added") {

                CreateChild( (string) data["key"], (string) data["value"] );

            } else if (action == "child_changed") {

                SetChild( (string) data["key"], (string) data["value"] );

            } else if (action == "child_removed") {

                RemoveChild( (string) data["key"] );

            }

        }

        private void SetTransformWithPSRString( Transform t, string strPSR ) {
            ItemListData d = this.system.ParseListData(strPSR);
            t.localPosition = d.position;
            t.localRotation = d.rotation;
            t.localScale = d.scale;
        }
        protected void CreateChild( string key, string strPSR ) {
            Debug.Log($"CreateChild {key}, {strPSR}");
            GameObject child = Instantiate(this.system.itemPrefab, new Vector3(0,0,0), Quaternion.identity, this.transform);
            child.name = key;
            SetTransformWithPSRString(child.transform, strPSR);
        }
        protected void SetChild( string key, string strPSR ) {
            Transform childTransform = transform.Find(key);
            if (childTransform) {
                Debug.Log($"SetChild {key} {strPSR}");
                SetTransformWithPSRString(childTransform, strPSR);
            }
        }
        protected void RemoveChild( string key ) {
            Transform childTransform = transform.Find(key);
            if (childTransform) {
                Debug.Log($"RemoveChild {key}");
                Destroy(childTransform.gameObject);
            }
        }

    }

}
