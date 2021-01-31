export function mockSnapshot(key: string, val: any) {
  return {
    key,
    val() {
      return val;
    },
  };
}

export default {
  database() {
    return {
      ref: jest.fn(),
    };
  },
};
