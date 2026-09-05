import { enableConfirmedForms, enableClientFilter } from "../../../core/table.js";

export function initializeUsersPage() {
    enableConfirmedForms();
    enableClientFilter("#users-table", 'input[name="Search"]');
}

document.addEventListener("DOMContentLoaded", () => {
    initializeUsersPage();
});
