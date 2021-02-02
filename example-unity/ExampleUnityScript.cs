using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class BridgeManagerTest : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern string unityToJs(string _event, string _payload);

    [DllImport("__Internal")]
    private static extern void unityWatch(string _event, string _objectName, string _functionName);

    [DllImport("__Internal")]
    private static extern void unityUnwatch(string _event, string _objectName, string _functionName);

    void JsToUnity(string payload)
    {
      if (payload == "Ready")
      {
        // Start interacting only when UnityBridge says it is ready.
        Debug.Log("UnityBridge.js is ready");
        string result = unityToJs("appendYee", "x");
        Debug.Log("appendYee's result is: "+result);
        unityWatch( "game1/" + SceneManager.GetActiveScene().name + "/Example/list", transform.name, "OnListData");
        unityWatch( "game1/" + SceneManager.GetActiveScene().name + "/Example/detail/key-example", transform.name, "OnItemData");
      } else if (payload == "Stop")
      {
      	// Now Start Unwatching
      	unityUnwatch( "game1/" + SceneManager.GetActiveScene().name + "/Example/detail/key-example", transform.name, "OnItemData");
      	unityUnwatch( "game1/" + SceneManager.GetActiveScene().name + "/Example/list", transform.name, "OnListData");
      }
    }

    void OnListData(string payload)
    {
      Debug.Log("receive list data: " + payload);
    }

    void OnItemData(string payload) {
      Debug.Log("receive data for one item: " + payload);
    }
}
