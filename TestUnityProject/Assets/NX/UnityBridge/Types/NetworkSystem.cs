using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using NX.UnityBridge.Types;

namespace NX.UnityBridge
{
  [CreateAssetMenu(fileName = "MyUBObjectType", menuName = "ScriptableObjects/NX.UnityBridge/ObjectType", order = 1)]
  public class NetworkSystem : ScriptableObject
  {
    public string systemName;
    public string listEventTemplate = "myGame/%SCENE%/%TYPE%/list";
    public string detailEventTemplate = "myGame/%SCENE%/%TYPE%/detail/%KEY%";
    public string GetEventName(string eventTemplate, string key = "")
    {
      return eventTemplate
        .Replace("%SCENE%", SceneManager.GetActiveScene().name)
        .Replace("%TYPE%", this.systemName)
        .Replace("%KEY%", key);
    }

    public GameObject itemPrefab;

    public ItemListData ParseListData(string input)
    {
      float[] n = input.Split(',').Select(el => float.Parse(el)).ToArray();
      ItemListData t;
      t.position = new Vector3(n[0], n[1], n[2]);
      t.rotation = new Quaternion(n[3], n[4], n[5], n[6]);
      t.scale = new Vector3(n[7], n[8], n[9]);
      return t;
    }

    public string SerializeItemData(Transform t)
    {
      Vector3 p = t.localPosition;
      Quaternion q = t.localRotation;
      Vector3 s = t.localScale;
      return $"{p.x},{p.y},{p.z},{q.x},{q.y},{q.z},{q.w},{s.x},{s.y},{s.z}";
    }

  }
}
