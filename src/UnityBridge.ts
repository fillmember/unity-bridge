import set from "lodash/set";
import remove from "lodash/remove";
import isArray from "lodash/isArray";
import unset from "lodash/unset";
import { EventEmitter } from "events";
export type UnityWatchEventPayload = { event: string; objectName: string; functionName: string; handler: Function };
export interface UnityBridgeConstructorOptions {
  unityInstance: UnityInstance;
  assignGlobal?: string;
  unityBridgeManagerName?: string;
  unityBridgeManagerMethodName?: string;
}
export type UnityInstance = {
  Module: {
    // SetFullscreen: (input: boolean) => void;
    SendMessage: (gameObject: string, functionName: string, arg?: string | number) => void;
  };
};
export class UnityBridge {
  options: UnityBridgeConstructorOptions;
  unityInstance: UnityInstance;
  static defaultOptions = {
    assignGlobal: "nx.bridge",
    unityBridgeManagerName: "UnityBridge",
    unityBridgeManagerMethodName: "JsToUnity",
  };
  public constructor(options: UnityBridgeConstructorOptions) {
    this.options = { ...UnityBridge.defaultOptions, ...options };
    this.unityInstance = this.options.unityInstance;
    set(window, this.options.assignGlobal, this);
    this.jsToUnityManager("Ready");
  }
  public jsToUnity(...args: Parameters<UnityInstance["Module"]["SendMessage"]>) {
    this.unityInstance.Module.SendMessage(...args);
  }
  public jsToUnityManager = (arg: string | number): void => {
    const { unityBridgeManagerName, unityBridgeManagerMethodName } = this.options;
    this.unityInstance.Module.SendMessage(unityBridgeManagerName, unityBridgeManagerMethodName, arg);
  };
  private unityToJsHandlers: Record<string, Function[]> = {};
  public registerUnityToJsHandler(eventName: string, fn: Function) {
    const list = this.unityToJsHandlers[eventName];
    if (isArray(list)) {
      list.push(fn);
    } else {
      this.unityToJsHandlers[eventName] = [fn];
    }
  }
  public deregisterUnityToJsHandler(eventName: string, fn: Function) {
    remove(this.unityToJsHandlers[eventName], (f) => f === fn);
  }
  public unityToJs(event: string, payload?: string): string {
    const list = this.unityToJsHandlers[event];
    if (isArray(list)) {
      return list.map((fn) => fn(payload)).join(",");
    }
    return null;
  }
  public events: EventEmitter = new EventEmitter();
  private unityWatchHandlerMap: Record<string, Record<string, Record<string, (...args: any) => void>>> = {};
  public unityWatch(event: string, objectName: string, functionName: string, handler: (data: any) => void): void {
    set(this.unityWatchHandlerMap, `${event}.${objectName}.${functionName}`, handler);
    this.events.on(event, handler);
    this.events.emit("onUnityWatch", { event, objectName, functionName, handler });
  }
  public unityUnwatch(event: string, objectName: string, functionName: string): void {
    const handler = this.unityWatchHandlerMap[event]?.[objectName]?.[functionName];
    if (handler) {
      this.events.off(event, handler);
      this.events.emit("onUnityUnwatch", { event, objectName, functionName, handler });
      unset(this.unityWatchHandlerMap, `${event}.${objectName}.${functionName}`);
    }
  }
}
