const fs = require("fs");

module.exports = function setup() {
  const content = fs.readFileSync("./src/jslibUnityBridge.js", "utf-8");
  const script = `
    var Pointer_stringify = v => v
    var LibraryManager = {}
    var mergeInto = function(_,data) {
      return data
    }
    ${content}
  `;
  return eval(script);
};
