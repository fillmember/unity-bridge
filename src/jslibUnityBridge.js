/**
 *
 * To Use In Unity:
 *
 * 1. Rename this file into unityBridge.jslib
 * 2. Place this file in your Unity project under: Assets/Plugins
 *
 */
mergeInto(LibraryManager.library, {
  unityToJs: function (_event, _payload) {
    var event = Pointer_stringify(_event);
    var payload = Pointer_stringify(_payload);
    return window.nx.bridge.unityToJs(event, payload);
  },
  unityWatch: function (_event, _objectName, _functionName) {
    var event = Pointer_stringify(_event);
    var objectName = Pointer_stringify(_objectName);
    var functionName = Pointer_stringify(_functionName);
    window.nx.bridge.unityWatch(event, objectName, functionName, function (data) {
      window.nx.bridge.jsToUnity(
        objectName,
        functionName,
        typeof data === "string" ? data : typeof data === "number" ? data : JSON.stringify(data)
      );
    });
  },
  unityUnwatch: function (_event, _objectName, _functionName) {
    var event = Pointer_stringify(_event);
    var objectName = Pointer_stringify(_objectName);
    var functionName = Pointer_stringify(_functionName);
    window.nx.bridge.unityUnwatch(event, objectName, functionName);
  },
});
