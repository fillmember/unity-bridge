# UnityBridge.js

## why all these code

I want to use Firebase Realtime Database with my Unity project.

But there's no official Firebase SDK for Unity WebGL target.

This library can also help you handle data subscriptions from Unity to Javascript.

## when to not use this project

- when Firebase release official SDK
- when you don't use Unity WebGL target

## Usage

For setting up the `UnityBridge` & `UnityBridgeFirebaseAddon`, check test/e2e/src/main.ts

For the Unity-side usage, check `example-unity/ExampleUnityScript.cs`.

## API

### UnityBridge

...TBD

### UnityBridgeFirebaseAddon

...TBD

## Test

### Jest

`npm t`

### E2E

1. supply your own firebase config
2. run `npm run test:e2e:dev`
3. run `npm run test:e2e:cypress`

E2E test TBD...
