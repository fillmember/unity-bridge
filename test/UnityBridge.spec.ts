import { UnityBridge } from "../src/UnityBridge";
import { mockUnityInstance } from "./util.mockUnity";
import { default as prepareJslib } from "./util.prepareJsLib";

declare global {
  interface Window {
    nx: {
      bridge: UnityBridge;
    };
  }
}

describe("UnityBridge", () => {
  it("accessible at window.nx.bridge", () => {
    const b = new UnityBridge({ unityInstance: mockUnityInstance() });
    expect(window.nx.bridge).toBe(b);
  });
  it("sends Ready event when start method is called", () => {
    const unityInstance = mockUnityInstance();
    const b = new UnityBridge({ unityInstance });
    b.start();
    expect(unityInstance.Module.SendMessage).toBeCalledWith(
      UnityBridge.defaultOptions.unityBridgeManagerName,
      UnityBridge.defaultOptions.unityBridgeManagerMethodName,
      "Ready"
    );
  });
  describe("jsToUnity", () => {
    it("calls unityInstance.Module.SendMessage", () => {
      const unityInstance = mockUnityInstance(),
        b = new UnityBridge({ unityInstance }),
        args = ["gameObject1", "Method1", "argument1"];
      unityInstance.Module.SendMessage.mockReset();
      b.jsToUnity.apply(b, args);
      expect(unityInstance.Module.SendMessage).toBeCalledWith(...args);
    });
  });
  describe("unityToJs", () => {
    it("functions", () => {
      const b = new UnityBridge({ unityInstance: mockUnityInstance() }),
        event = "e",
        handler = (payload) => `test-${payload}`,
        handler2 = (payload) => `yee-${payload}`,
        payload = "p";
      b.registerUnityToJsHandler(event, handler);
      b.registerUnityToJsHandler(event, handler2);
      expect(b.unityToJs(event, payload)).toBe([handler(payload), handler2(payload)].join(","));
      b.deregisterUnityToJsHandler(event, handler);
      b.deregisterUnityToJsHandler(event, handler2);
      // @ts-ignore
      expect(b.unityToJsHandlers.e).toHaveLength(0);
    });
  });
  describe("unityWatch & unityUnwatch", () => {
    it("registers and unregisters", () => {
      const unityInstance = mockUnityInstance(),
        b = new UnityBridge({ unityInstance }),
        event = "e",
        objectName = "obj",
        functionName = "method1",
        fn = jest.fn();
      b.unityWatch(event, objectName, functionName, fn);
      // @ts-ignore
      expect(b.unityWatchHandlerMap[event][objectName][functionName]).toBe(fn);
      b.unityUnwatch(event, objectName, functionName);
      // @ts-ignore
      expect(b.unityWatchHandlerMap[event][objectName][functionName]).toBeUndefined();
    });
    it("functions", () => {
      const unityInstance = mockUnityInstance(),
        b = new UnityBridge({ unityInstance }),
        event = "e",
        fn = jest.fn(),
        fn2 = jest.fn(),
        payload = "arg";
      const onUnityWatch = jest.fn();
      b.events.on("onUnityWatch", onUnityWatch);
      b.unityWatch(event, "obj1", "fn1", fn);
      expect(onUnityWatch).toBeCalledWith({ event, functionName: "fn1", objectName: "obj1", handler: fn });
      b.unityWatch(event, "obj2", "fn2", fn2);
      expect(onUnityWatch).toBeCalledWith({ event, functionName: "fn2", objectName: "obj2", handler: fn2 });
      b.events.emit(event, payload);
      expect(fn).toBeCalledWith(payload);
      expect(fn2).toBeCalledWith(payload);
    });
  });
  describe("integration test - jslib", () => {
    it("unity: watch & unwatch", () => {
      const unityInstance = mockUnityInstance(),
        b = new UnityBridge({ unityInstance }),
        objectName = "obj",
        functionName = "method1",
        payload = "arg",
        event = "e";
      b.start();
      expect(unityInstance.Module.SendMessage).toBeCalledTimes(1);
      unityInstance.Module.SendMessage.mockReset();
      const lib = prepareJslib();
      lib.unityWatch(event, objectName, functionName);
      b.events.emit(event, payload);
      expect(unityInstance.Module.SendMessage).toBeCalledWith(objectName, functionName, payload);
      lib.unityUnwatch(event, objectName, functionName);
      // @ts-ignore
      expect(b.unityWatchHandlerMap[event][objectName][functionName]).toBeUndefined();
    });
    it("unity: unityToJs", () => {
      const unityInstance = mockUnityInstance(),
        b = new UnityBridge({ unityInstance }),
        lib = prepareJslib();
      unityInstance.Module.SendMessage.mockReset();
      const eventName = "evt",
        payload = "payload";
      b.registerUnityToJsHandler(eventName, (x) => x);
      expect(lib.unityToJs(eventName, payload)).toBe(payload);
      expect(unityInstance.Module.SendMessage).toBeCalledTimes(0);
    });
  });
});
