import { UnityBridge } from "../src/UnityBridge";
import { UnityBridgeFirebaseAddon } from "../src/UnityBridgeFirebaseAddon";
import { mockUnityInstance } from "./util.mockUnity";
import firebase, { mockSnapshot } from "./util.mockFirebase";
import EventEmitter from "events";

describe("UnityBridgeFirebaseAddon", () => {
  describe("defaultEventParser", () => {
    const f = UnityBridgeFirebaseAddon.defaultEventParser;
    ["Negative", "Mi/Yee", "//", "///"].forEach((eventName) => {
      it("ignore invalid event: " + eventName, () => {
        const event = { event: eventName, objectName: "", functionName: "", handler: jest.fn() };
        expect(f("onUnityWatch", event)).toBe(false);
        expect(f("onUnityUnwatch", event)).toBe(false);
      });
    });
  });
  it("register event listeners", () => {
    const bridge = new UnityBridge({ unityInstance: mockUnityInstance() });
    const f = new UnityBridgeFirebaseAddon({
      bridge,
      database: firebase.database() as any,
    });
    expect(bridge.events.listeners("onUnityWatch")[0]).toBe(f.onUnityWatch);
    expect(bridge.events.listeners("onUnityUnwatch")[0]).toBe(f.onUnityUnwatch);
  });
  describe("integration tests", () => {
    it("#1", () => {
      const bridge = new UnityBridge({ unityInstance: mockUnityInstance() });
      const database: any = firebase.database();
      const refOnFn = jest.fn();
      const refOffFn = jest.fn();
      database.ref.mockReturnValue({
        on: refOnFn,
        off: refOffFn,
      });
      new UnityBridgeFirebaseAddon({
        bridge,
        database,
      });
      // Watch
      bridge.unityWatch("Test/Players/list", "o", "f", jest.fn());
      expect(database.ref).toBeCalledWith("Test/Players/list");
      expect(refOnFn).toBeCalledTimes(4);
      bridge.unityWatch("Test/Players/detail/yeedog", "o", "f", jest.fn());
      expect(database.ref).toBeCalledWith("Test/Players/detail/yeedog");
      expect(refOnFn).toBeCalledTimes(5);
      // Unwatch
      bridge.unityUnwatch("Test/Players/list", "o", "f");
      expect(refOffFn).toBeCalledTimes(4);
      bridge.unityUnwatch("Test/Players/detail/yeedog", "o", "f");
      expect(refOffFn).toBeCalledTimes(5);
    });
    it("#2 data", () => {
      const bridge = new UnityBridge({ unityInstance: mockUnityInstance() });
      const eventEmitter = new EventEmitter();
      const database: any = {
        ref: () => eventEmitter,
      };
      new UnityBridgeFirebaseAddon({
        bridge,
        database,
      });
      const handler = jest.fn();
      bridge.unityWatch("Test/Players/list", "o", "f", handler);

      eventEmitter.emit("value", mockSnapshot("list", { yeedog: "0,0" }));
      expect(handler).toBeCalledTimes(1);
      expect(handler).toBeCalledWith({ action: "value", data: { yeedog: "0,0" } });

      handler.mockClear();
      eventEmitter.emit("child_added", mockSnapshot("mi", "0,2"));
      expect(handler).toBeCalledTimes(1);
      expect(handler).toBeCalledWith({ action: "child_added", data: { key: "mi", value: "0,2" } });

      handler.mockClear();
      eventEmitter.emit("child_changed", mockSnapshot("yeedog", "0,1"));
      expect(handler).toBeCalledTimes(1);
      expect(handler).toBeCalledWith({ action: "child_changed", data: { key: "yeedog", value: "0,1" } });

      handler.mockClear();
      eventEmitter.emit("child_removed", mockSnapshot("mi", "0,2"));
      expect(handler).toBeCalledTimes(1);
      expect(handler).toBeCalledWith({ action: "child_removed", data: { key: "mi", value: "0,2" } });

      // Unwatch
      bridge.unityUnwatch("Test/Players/list", "o", "f");
      expect(eventEmitter.listenerCount("value")).toBe(0);
      expect(eventEmitter.listenerCount("child_added")).toBe(0);
      expect(eventEmitter.listenerCount("child_changed")).toBe(0);
      expect(eventEmitter.listenerCount("child_removed")).toBe(0);
    });
  });
});
