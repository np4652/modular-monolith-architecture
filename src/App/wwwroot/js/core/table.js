import { on, qsa } from "./dom.js";
import { confirmDialog } from "./modal.js";

/**
 * Wires the "confirm before submit" behavior any data table row action can opt
 * into with data-confirm="message", instead of every feature re-implementing it.
 */
export function enableConfirmedForms(root = document) {
    on(root, "submit", "form[data-confirm]", async (event, form) => {
        if (form.dataset.confirmed === "true") return;

        event.preventDefault();
        const confirmed = await confirmDialog(form.dataset.confirm);
        if (confirmed) {
            form.dataset.confirmed = "true";
            form.submit();
        }
    });
}

/** Simple client-side text filter for a table already rendered by the server. */
export function enableClientFilter(tableSelector, inputSelector, root = document) {
    const table = root.querySelector(tableSelector);
    const input = root.querySelector(inputSelector);
    if (!table || !input) return;

    input.addEventListener("input", () => {
        const term = input.value.trim().toLowerCase();
        qsa("tbody tr", table).forEach((row) => {
            row.hidden = term.length > 0 && !row.textContent.toLowerCase().includes(term);
        });
    });
}
