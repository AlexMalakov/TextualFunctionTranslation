//claude made this I have no idea how this works ill be so fr
mergeInto(LibraryManager.library, {
  WebGLCopyToClipboard: function (strPtr) {
    var text = UTF8ToString(strPtr);

    function fallbackCopy() {
      var ta = document.createElement("textarea");
      ta.value = text;
      ta.style.position = "fixed";
      ta.style.opacity = "0";
      document.body.appendChild(ta);
      ta.focus();
      ta.select();
      try { document.execCommand("copy"); } catch (e) { console.warn("Copy failed", e); }
      document.body.removeChild(ta);
    }

    if (navigator.clipboard && navigator.clipboard.writeText) {
      navigator.clipboard.writeText(text).catch(fallbackCopy);
    } else {
      fallbackCopy();
    }
  }
});