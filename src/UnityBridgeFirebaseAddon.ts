import unset from "lodash/unset";
import firebase from "firebase/app";
import { UnityBridge, UnityWatchEventPayload } from "./UnityBridge";
type EventParser = (ctx: string, e: UnityWatchEventPayload) => false | { path: string; directive: string };
export class UnityBridgeFirebaseAddon {
  static defaultEventParser: EventParser = (_ctx, e) => {
    // expected event format: $scene/$category/$directive(/$itemKey)
    const [scene, category, directive, itemKey] = e.event.split("/");
    if (!scene || !category || !directive) return false;
    return { path: e.event, scene, category, directive, itemKey };
  };
  b: UnityBridge;
  db: firebase.database.Database;
  unsubscribers: Record<string, Function[]> = {};
  parseEvent: EventParser;
  constructor({
    bridge,
    database,
    eventParser = UnityBridgeFirebaseAddon.defaultEventParser,
  }: {
    bridge: UnityBridge;
    database: firebase.database.Database;
    eventParser?: EventParser;
  }) {
    this.b = bridge;
    this.db = database;
    this.b.events.on("onUnityWatch", this.onUnityWatch);
    this.b.events.on("onUnityUnwatch", this.onUnityUnwatch);
    this.parseEvent = eventParser;
  }
  onUnityWatch = (evt) => {
    const result = this.parseEvent("onUnityWatch", evt);
    if (result === false) return;
    const { path, directive } = result;
    const dbReference = this.db.ref(result.path);
    const makeDBEvtHandler = (event: firebase.database.EventType) => {
      const isValueEvent = event === "value";
      const handler = (snapshot: firebase.database.DataSnapshot) => {
        this.b.events.emit(path, {
          action: event,
          data: isValueEvent ? snapshot.val() : { key: snapshot.key, value: snapshot.val() },
        });
      };
      dbReference.on(event, handler);
      const unsubscriber = () => {
        dbReference.off(event, handler);
      };
      return unsubscriber;
    };
    const isWatchingList = directive === "list";
    if (isWatchingList) {
      this.unsubscribers[path] = [makeDBEvtHandler("child_added"), makeDBEvtHandler("child_changed"), makeDBEvtHandler("child_removed")];
    } else {
      this.unsubscribers[path] = [makeDBEvtHandler("value")];
    }
  };
  onUnityUnwatch = (evt) => {
    const result = this.parseEvent("onUnityUnwatch", evt);
    if (result === false) return;
    const { path } = result;
    const list = this.unsubscribers[path];
    if (list) {
      list.forEach((fn) => fn());
    }
    unset(this.unsubscribers, path);
  };
}
