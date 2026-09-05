import { qsa, on } from "../../../core/dom.js";

/** Adds a "select all / none" convenience above the role checkbox list. */
export function initializeUserEditPage() {
    const checkboxes = qsa('input[name="Input.RoleIds"]');
    if (checkboxes.length === 0) return;

    const toggle = document.createElement("button");
    toggle.type = "button";
    toggle.className = "btn btn-link";
    toggle.textContent = "Select all";
    checkboxes[0].closest("fieldset")?.prepend(toggle);

    on(toggle, "click", () => {
        const shouldCheck = checkboxes.some((cb) => !cb.checked);
        checkboxes.forEach((cb) => (cb.checked = shouldCheck));
        toggle.textContent = shouldCheck ? "Select none" : "Select all";
    });
}

document.addEventListener("DOMContentLoaded", () => {
    initializeUserEditPage();
});
