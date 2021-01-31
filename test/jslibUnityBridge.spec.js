const fs = require("fs");
const prepareJslib = require("./util.prepareJsLib");

describe("unityBridge.jslib", () => {
  function setup() {
    window.nx = {
      bridge: {
        unityToJs: jest.fn(),
        unityWatch: jest.fn(),
        unityUnwatch: jest.fn(),
        jsToUnity: jest.fn(),
      },
    };
    return prepareJslib();
  }
  describe("unityToJs", () => {
    it("exists", () => {
      const fn = setup().unityToJs;
      expect(fn).toBeTruthy();
    });
    it("returns a value and pass args to UnityBridge.JS", () => {
      const fn = setup().unityToJs;
      window.nx.bridge.unityToJs.mockReturnValueOnce("test");
      expect(fn("a", "b")).toBe("test");
      window.nx.bridge.unityToJs.mockReturnValueOnce(0);
      expect(fn("a", "b")).toBe(0);
      expect(window.nx.bridge.unityToJs).toBeCalledWith("a", "b");
    });
  });
  describe("unityWatch", () => {
    it("exists", () => {
      const fn = setup().unityWatch;
      expect(fn).toBeTruthy();
    });
    it("should only return undefined (void function)", () => {
      const fn = setup().unityWatch;
      expect(fn()).toBe(undefined);
    });
    it("interacts wtih UnityBridge.JS propery", () => {
      const fn = setup().unityWatch;
      fn("event", "obj", "fn");
      expect(window.nx.bridge.unityWatch).toBeCalledTimes(1);
      const args = window.nx.bridge.unityWatch.mock.calls[0];
      expect(args[0]).toBe("event");
      expect(args[1]).toBe("obj");
      expect(args[2]).toBe("fn");
      args[3]("data");
      expect(window.nx.bridge.jsToUnity).toBeCalledWith("obj", "fn", "data");
      args[3](0);
      expect(window.nx.bridge.jsToUnity).toBeCalledWith("obj", "fn", 0);
    });
  });
  describe("unityUnwatch", () => {
    it("exists", () => {
      const fn = setup().unityUnwatch;
      expect(fn).toBeTruthy();
    });
    it("should only return undefined (void function)", () => {
      const fn = setup().unityUnwatch;
      expect(fn()).toBe(undefined);
    });
    it("pass along arguments to UnityBridge.JS", () => {
      const fn = setup().unityUnwatch;
      fn("a", "b", "c");
      expect(window.nx.bridge.unityUnwatch).toBeCalledWith("a", "b", "c");
    });
  });
});
