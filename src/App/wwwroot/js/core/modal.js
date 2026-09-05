import { qs } from "./dom.js";

const ROOT_ID = "modal-root";

/**
 * A single confirmation modal mounted once in _Layout.cshtml (#modal-root) and
 * reused by every feature - no page builds its own ad-hoc confirm dialog markup.
 */
export function confirmDialog(message) {
    const root = qs(`#${ROOT_ID}`);
    if (!root) return Promise.resolve(window.confirm(message));

    return new Promise((resolve) => {
        root.innerHTML = `
            <div class="modal-backdrop"></div>
            <div class="modal-dialog" role="alertdialog" aria-modal="true">
                <p class="modal-message"></p>
                <div class="modal-actions">
                    <button type="button" class="btn btn-secondary" data-action="cancel">Cancel</button>
                    <button type="button" class="btn btn-danger" data-action="confirm">Confirm</button>
                </div>
            </div>`;
        qs(".modal-message", root).textContent = message;
        root.hidden = false;

        const close = (result) => {
            root.hidden = true;
            root.innerHTML = "";
            resolve(result);
        };

        qs('[data-action="confirm"]', root).addEventListener("click", () => close(true));
        qs('[data-action="cancel"]', root).addEventListener("click", () => close(false));
        qs(".modal-backdrop", root).addEventListener("click", () => close(false));
    });
}
