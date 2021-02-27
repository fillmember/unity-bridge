using UnityEngine;
using System;
using System.Collections.Generic;
using BestHTTP.ServerSentEvents;

namespace NX.Networking.Firebase
{
  class WatchEventObject
  {
    private int readBuffer = 0;
    private string path;
    private Transform target;
    private string callbackName;
    private EventSource source;
    public WatchEventObject(string _path, Transform _target, string _callbackName)
    {
      path = _path;
      target = _target;
      callbackName = _callbackName;
      source = new EventSource(new Uri(path), readBuffer);
      source.On("put", OnPut);
      source.On("patch", OnPatch);
      source.Open();
    }
    void OnPut(EventSource source, Message msg) {
      Debug.Log(string.Format("== FB OnPut: <color=yellow>{0}</color>", msg.Data.ToString()));
    }
    void OnPatch(EventSource source, Message msg) {
      Debug.Log("== FB OnPatch " + msg.Data);
    }
  }
  class FirebaseRestAPI : NX.Singleton<FirebaseRestAPI>
  {
    public string baseUrl = "";
    private string GetKey(string _event, Transform target, string callbackName) {
      return $"{_event}-${target.name}-${callbackName}";
    }
    private string GetPath(string pathname) {
      return $"{baseUrl}{pathname}";
    }
    public Dictionary<string, WatchEventObject> watchList = new Dictionary<string, WatchEventObject>();
    public void Watch(string _event, Transform target, string callbackName)
    {
      string key = GetKey(_event, target, callbackName);
      string path = GetPath(_event);
      WatchEventObject eventObject = new WatchEventObject(path, target, callbackName);
      watchList.Add(key, eventObject);
    }
    public void Unwatch(string _event, Transform target, string callbackName)
    {
      string key = GetKey(_event, target, callbackName);
      watchList.Remove(key);
    }
  }
}
