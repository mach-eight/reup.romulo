mergeInto(LibraryManager.library, {

  SendStringToWeb: function (str) {
    const data = { messageType: "messageFromUnity", serializedMessage: UTF8ToString(str) };
    globalThis.postMessage(data, '*');
  }
});