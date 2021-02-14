using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NX.UnityBridge
{
  [CustomEditor(typeof(NX.UnityBridge.UnityBridgeManager))]
  public class UnityBridgeManagerEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      UnityBridgeManager script = (UnityBridgeManager)target;
      if (GUILayout.Button("mock Ready event"))
      {
        script.JsToUnity("Ready");
      }
    }
  }

}
