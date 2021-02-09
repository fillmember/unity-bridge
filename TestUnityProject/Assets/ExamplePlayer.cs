using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NX.UnityBridge;

public class ExamplePlayer : MonoBehaviour
{

  public NetworkSystem system;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (system && Input.GetKeyUp(KeyCode.Space))
    {
      UnityBridgeManager.unityToJs(
        "ObjectSystemCreate",
        "{" +
            $"\"scene\":\"{SceneManager.GetActiveScene().name}\"," +
            $"\"strPSR\":\"{ system.SerializeItemData(transform) }\"," +
            "\"strDetail\":{" +
                "\"color\":\"random\"," +
                "\"mood\":\"random\"" +
            "}" +
        "}"
        );
    }
    if (Input.GetKey(KeyCode.DownArrow))
    {
      transform.Translate(Vector3.forward * Time.deltaTime * -1);
    }
    if (Input.GetKey(KeyCode.UpArrow))
    {
      transform.Translate(Vector3.forward * Time.deltaTime);
    }
    if (Input.GetKey(KeyCode.LeftArrow))
    {
      transform.Translate(Vector3.left * Time.deltaTime);
    }
    if (Input.GetKey(KeyCode.RightArrow))
    {
      transform.Translate(Vector3.right * Time.deltaTime);
    }
  }
}
