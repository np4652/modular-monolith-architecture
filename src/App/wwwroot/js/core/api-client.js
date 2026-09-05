import { request } from "./http.js";

/**
 * The only sanctioned way feature modules perform AJAX. Every module depends on
 * this abstraction rather than on fetch()/XHR directly (see ARCHITECTURE.md #17).
 */
export const apiClient = {
    get: (url) => request("GET", url),
    post: (url, data) => request("POST", url, data),
    put: (url, data) => request("PUT", url, data),
    delete: (url) => request("DELETE", url),
};
