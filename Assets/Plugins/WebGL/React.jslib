mergeInto(LibraryManager.library, {
  GameOver: function (score) {
    window.dispatchReactUnityEvent("GameOver", score);
  },
});