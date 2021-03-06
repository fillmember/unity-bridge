import firebase from "firebase/app";
import "firebase/auth";
import "firebase/database";
import { UnityBridge } from "../../../src/UnityBridge";
import { UnityBridgeFirebaseAddon } from "../../../src/UnityBridgeFirebaseAddon";
import firebaseConfig from "./firebaseConfig";

const app = firebase.initializeApp(firebaseConfig);
const database = firebase.database(app);
const auth = firebase.auth();

const buildUrl = "unity/Build";
const loaderUrl = buildUrl + "/unity.loader.js";
const config = {
  dataUrl: buildUrl + "/unity.data.gz",
  frameworkUrl: buildUrl + "/unity.framework.js.gz",
  codeUrl: buildUrl + "/unity.wasm.gz",
  streamingAssetsUrl: "StreamingAssets",
  companyName: "DefaultCompany",
  productName: "CrapProject",
  productVersion: "0.1",
};

function objectSystemCreate(payload: string): string {
  try {
    const { scene, strPSR } = JSON.parse(payload);
    database
      .ref(`myGame/${scene}/Object/detail`)
      .push({ type: "cube", user_id: auth.currentUser.uid })
      .then(({ key }) => {
        return database.ref(`myGame/${scene}/Object/list/${key}`).set(strPSR);
      });
    return "OK";
  } catch (error) {
    console.error(error.message);
    return "NOK";
  }
}

const script = document.createElement("script");
script.src = loaderUrl;
script.onload = () => {
  window
    // @ts-ignore
    .createUnityInstance(document.querySelector("canvas"), config, () => {})
    .then((unityInstance) => {
      return auth
        .signInAnonymously()
        .then(() => ({ unityInstance }))
        .catch((error) => {});
    })
    .then(({ unityInstance }) => {
      const bridge = new UnityBridge({ unityInstance, unityBridgeManagerName: "[UnityBridge.js]" });
      bridge.registerUnityToJsHandler("ObjectSystemCreate", (payload) => {
        objectSystemCreate(payload);
      });
      bridge.events.on("onUnityWatch", (e) => console.log("Unity watching", e));
      bridge.events.on("onUnityUnwatch", (e) => console.log("Unity stop watching", e));
      const addon = new UnityBridgeFirebaseAddon({
        bridge,
        database,
        // set eventParser to resolve actual path on database
        eventParser(ctx, event) {
          const result = UnityBridgeFirebaseAddon.defaultEventParser(ctx, event);
          if (result === false) return result;
          return {
            path: result.path,
            directive: result.directive,
          };
        },
      });
      bridge.start();
    })
    .catch((message) => {
      console.error(message);
    });
};
document.body.appendChild(script);
