/**
 * Small DOM query/event helpers shared by every feature module - kept dependency-free
 * so core never needs a bundler or a UI framework.
 */
export function qs(selector, root = document) {
    return root.querySelector(selector);
}

export function qsa(selector, root = document) {
    return Array.from(root.querySelectorAll(selector));
}

export function on(target, eventName, selectorOrHandler, maybeHandler) {
    if (typeof selectorOrHandler === "function") {
        target.addEventListener(eventName, selectorOrHandler);
        return;
    }

    const selector = selectorOrHandler;
    const handler = maybeHandler;
    target.addEventListener(eventName, (event) => {
        const match = event.target.closest(selector);
        if (match && target.contains(match)) {
            handler(event, match);
        }
    });
}
