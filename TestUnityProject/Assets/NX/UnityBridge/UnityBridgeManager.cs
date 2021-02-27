using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using NX.Networking.Firebase;

namespace NX.UnityBridge {


    public class UnityBridgeManager : NX.Singleton<UnityBridgeManager> {

        public enum CommunicationMode {
            JavascriptBridge,
            RestAPI
        }

        public List<GameObject> listInstantiateOnReady;
        public CommunicationMode mode = CommunicationMode.RestAPI;

        #region "Javascript Bridge Methods"

        [DllImport("__Internal")]
        public static extern string unityToJs(string _event, string _payload);

        [DllImport("__Internal")]
        public static extern void unityWatch(string _event, string _objectName, string _functionName);

        [DllImport("__Internal")]
        public static extern void unityUnwatch(string _event, string _objectName, string _functionName);

        #endregion

        #region "Methods"

        public static void Watch(string _event, Transform target, string _functionName) {
            if (Instance.mode == CommunicationMode.JavascriptBridge) {
                unityWatch( _event, target.name, _functionName);
            } else if (Instance.mode == CommunicationMode.RestAPI) {
                FirebaseRestAPI.Instance.Watch(_event, target, _functionName);
            }
        }
        public static void Unwatch(string _event, Transform target, string _functionName) {
            if (Instance.mode == CommunicationMode.JavascriptBridge) {
                unityUnwatch( _event, target.name, _functionName);
            } else if (Instance.mode == CommunicationMode.RestAPI) {
                FirebaseRestAPI.Instance.Unwatch(_event, target, _functionName);
            }
        }
        public static void Emit(string _event, string _payload) {
            if (Instance.mode == CommunicationMode.JavascriptBridge) {
                unityToJs( _event, _payload);
            } else if (Instance.mode == CommunicationMode.RestAPI) {
                // TBD
            }
        }

        private void InstantiateGameObjects() {
            listInstantiateOnReady.ForEach(delegate(GameObject obj) {
                Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity);
            });
        }

        #endregion

        #region "Lifecycle Hooks And Event Handlers"

        public void Start() {
            if (mode == CommunicationMode.RestAPI) {
                InstantiateGameObjects();
            }
        }

        /* Javascript-side Call Receiver */

        public void JsToUnity(string payload) {
            if (payload == "Ready") {
                InstantiateGameObjects();
            } else if (payload == "Stop") {
                // TBD
            }
        }

        #endregion

    }

}
