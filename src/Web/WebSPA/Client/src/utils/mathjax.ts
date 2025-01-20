// @ts-nocheck
window.MathJax = {
    tex: {
        inlineMath: [
            ["$", "$"],
            ["\\(", "\\)"],
        ],
        displayMath: [
            ["$$", "$$"],
            ["\\[", "\\]"],
        ],
    },
    startup: {
        ready() {
            MathJax.startup.defaultReady();
        },
    },
};
  