using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NX.UnityBridge;

public class ExamplePlayer : MonoBehaviour
{

  public NetworkSystem system;
  public float speed = 2.5f;

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKey(KeyCode.DownArrow))
    {
      transform.Translate(Vector3.forward * speed * Time.deltaTime * -1);
    }
    if (Input.GetKey(KeyCode.UpArrow))
    {
      transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    if (Input.GetKey(KeyCode.LeftArrow))
    {
      transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
    if (Input.GetKey(KeyCode.RightArrow))
    {
      transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
  }
}
