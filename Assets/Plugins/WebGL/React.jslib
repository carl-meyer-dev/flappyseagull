mergeInto(LibraryManager.library, {
  GameOver: function (score) {
    window.dispatchReactUnityEvent("GameOver", score);
  },
  PlayAgain: function () {
    window.dispatchReactUnityEvent("PlayAgain");
  },
  Quit: function () {
    window.dispatchReactUnityEvent("Quit");
  },
});