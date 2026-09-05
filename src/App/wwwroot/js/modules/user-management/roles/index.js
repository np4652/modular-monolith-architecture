import { enableConfirmedForms } from "../../../core/table.js";

export function initializeRolesPage() {
    enableConfirmedForms();
}

document.addEventListener("DOMContentLoaded", () => {
    initializeRolesPage();
});
