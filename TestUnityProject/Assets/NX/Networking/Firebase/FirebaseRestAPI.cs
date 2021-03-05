using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.ServerSentEvents;

namespace NX.Networking.Firebase
{
  class WatchEventObject
  {
    private int readBuffer = 1;
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
      if (path.Contains("/list")) {
        source.On("put", OnListPut);
        source.On("patch", OnListPatch);
      } else if (path.Contains("/detail")) {
        source.On("put", OnItemUpdate);
        source.On("patch", OnItemUpdate);
      }
      source.Open();
    }
    public void Close() {
      source.Close();
    }
    private void OnItemUpdate(EventSource source, Message msg) {
      Hashtable parsed = (Hashtable)Procurios.Public.JSON.JsonDecode(msg.Data.ToString());
      Emit( (Hashtable) parsed["data"] );
    }
    private void OnListPut(EventSource source, Message msg) {
      Hashtable parsed = (Hashtable)Procurios.Public.JSON.JsonDecode(msg.Data.ToString());
      string path = (string)parsed["path"];
      if (path == "/") {
        Hashtable list = (Hashtable)parsed["data"];
        foreach(DictionaryEntry entry in list) {
          Dictionary<string,string> parameters = new Dictionary<string, string>();
          parameters.Add("action","child_added");
          parameters.Add("key", (string)entry.Key);
          parameters.Add("value",(string)entry.Value);
          Emit(parameters);
        }
      } else {
        Dictionary<string,string> parameters = new Dictionary<string, string>();
        string key = path.Substring(1);
        string data = (string)parsed["data"];
        string action = data == null ? "child_removed" : "child_changed";
        parameters.Add("action", action);
        parameters.Add("key", key );
        parameters.Add("value", data );
        Emit(parameters);
      }
    }
    private void OnListPatch(EventSource source, Message msg) {
      Debug.Log("== FB OnPatch " + msg.Data.ToString());
    }
    private void Emit(object payload) {
      target.SendMessage(callbackName, payload);
    }
  }
  class FirebaseRestAPI : NX.Singleton<FirebaseRestAPI>
  {
    public string baseUrl = "";
    public string urlPostfix = "?auth=preshare-key";
    public Dictionary<string, WatchEventObject> watchList = new Dictionary<string, WatchEventObject>();
    private string GetKey(string _event, Transform target, string callbackName) {
      return $"{_event}-${target.name}-${callbackName}";
    }
    private string GetPath(string pathname) {
      return $"{baseUrl}{pathname}.json{urlPostfix}";
    }
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
      if (!watchList.ContainsKey(key)) {
        return;
      }
      WatchEventObject obj;
      watchList.TryGetValue(key, out obj);
      if (obj != null) {
        obj.Close();
      }
      watchList.Remove(key);
    }
  }
}
