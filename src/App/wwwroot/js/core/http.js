/**
 * The single place that knows how to talk HTTP to this application - antiforgery
 * header, JSON (de)serialization, and error normalization. Feature modules must
 * never call fetch() directly; they go through api-client.js, which is built on this.
 */
function getAntiforgeryToken() {
    const meta = document.querySelector('meta[name="csrf-token"]');
    return meta ? meta.content : null;
}

export class HttpError extends Error {
    constructor(status, message) {
        super(message);
        this.name = "HttpError";
        this.status = status;
    }
}

export async function request(method, url, body) {
    const headers = { Accept: "application/json" };
    const token = getAntiforgeryToken();
    if (token) headers["X-CSRF-TOKEN"] = token;

    const init = {
        method,
        headers,
        credentials: "same-origin",
        // Identifies this as an AJAX call so the server's exception middleware
        // returns a JSON error instead of redirecting to an HTML error page.
        // (see ExceptionHandlingMiddleware.IsAjaxRequest)
    };
    headers["X-Requested-With"] = "XMLHttpRequest";

    if (body !== undefined) {
        headers["Content-Type"] = "application/json";
        init.body = JSON.stringify(body);
    }

    const response = await fetch(url, init);

    if (!response.ok) {
        let message = `Request failed with status ${response.status}.`;
        try {
            const problem = await response.json();
            if (problem?.error) message = problem.error;
        } catch {
            // Response had no JSON body - keep the generic message.
        }
        throw new HttpError(response.status, message);
    }

    if (response.status === 204) return null;

    const contentType = response.headers.get("content-type") ?? "";
    return contentType.includes("application/json") ? response.json() : response.text();
}
