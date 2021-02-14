using System.Linq;
using System.Collections;
using UnityEngine;

namespace NX.UnityBridge.Types
{
  public class ItemData
  {
    public string Key;
    public Vector3 Position;
    private Vector3 Scale;
    private Quaternion Rotation;
    public ItemData(string key, string psr)
    {
      Key = key;
      CalculatePSR(psr);
    }
    public void CalculatePSR(string input)
    {
      float[] n = input.Split(',').Select(el => float.Parse(el)).ToArray();
      Position = new Vector3(n[0], n[1], n[2]);
      Rotation = new Quaternion(n[3], n[4], n[5], n[6]);
      Scale = new Vector3(n[7], n[8], n[9]);
    }
    public void SetTransform(Transform t)
    {
      t.position = Position;
      t.rotation = Rotation;
      t.localScale = Scale;
    }
  }
}
