mergeInto(LibraryManager.library, {
 
 
  PasteHereWindow: function (sometext) {
    var pastedtext= prompt("Please paste here:", "placeholder");
    SendMessage("CopyPasteObject", "GetPastedText", pastedtext);
  },
 
});