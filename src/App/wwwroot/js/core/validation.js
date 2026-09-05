import { qs, qsa } from "./dom.js";

/**
 * Renders server-side validation failures (ModelState, translated from
 * FluentValidation) onto the same field-level spans ASP.NET's unobtrusive
 * validation already uses - client-side rules are a convenience, the server
 * is always authoritative (see ARCHITECTURE.md #26).
 */
export function clearFieldErrors(form) {
    qsa(".field-validation-error", form).forEach((el) => (el.textContent = ""));
}

export function applyFieldErrors(form, errors) {
    clearFieldErrors(form);

    for (const [field, messages] of Object.entries(errors ?? {})) {
        const target = qs(`[data-valmsg-for="${field}"]`, form) ?? qs(`[data-valmsg-for="Input.${field}"]`, form);
        if (target) target.textContent = Array.isArray(messages) ? messages[0] : messages;
    }
}
