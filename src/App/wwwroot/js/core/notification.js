import { qs } from "./dom.js";

const CONTAINER_ID = "notification-container";
const AUTO_DISMISS_MS = 6000;

function getContainer() {
    return qs(`#${CONTAINER_ID}`);
}

function show(message, variant) {
    const container = getContainer();
    if (!container) return;

    const node = document.createElement("div");
    node.className = `notification notification--${variant}`;
    node.setAttribute("role", variant === "error" ? "alert" : "status");
    node.textContent = message;

    container.appendChild(node);
    setTimeout(() => node.remove(), AUTO_DISMISS_MS);
}

export const notification = {
    success: (message) => show(message, "success"),
    error: (message) => show(message, "error"),
};
