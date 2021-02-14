using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NX.UnityBridge
{
  [CustomEditor(typeof(NX.UnityBridge.NetworkItemList))]
  public class NetworkItemListEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      NetworkItemList script = (NetworkItemList)target;
      if (GUILayout.Button("OnData (random)"))
      {
        script.OnData(@"

        ");
      }
    }
  }

}
