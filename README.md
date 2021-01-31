# UnityBridge.js

## why all these code

I want to use Firebase Realtime Database with my Unity project.

But there's no official Firebase SDK for Unity WebGL target.

This library can also help you handle data subscriptions from Unity to Javascript.

## when to not use this project

- when Firebase release official SDK
- when you don't use Unity WebGL target

## API

```js
createUnityInstance().then((unityInstance) => {
  const bridge = new UnityBridge({
    unityInstance: unityInstance,
    assignGlobal: "nx.bridge", // bridge is accessible on window.nx.bridge
  });
});
```

TBD...
