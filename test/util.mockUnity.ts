export function mockUnityInstance() {
  const unityInstance = {
    Module: {
      SendMessage: jest.fn(),
    },
  };
  return unityInstance;
}
