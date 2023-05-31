window.Lagoon = window.Lagoon || {}

window.Lagoon.hightlight = function(elt) {
    elt.querySelectorAll('pre code').forEach((el) => {
        hljs.highlightElement(el);
    });
}