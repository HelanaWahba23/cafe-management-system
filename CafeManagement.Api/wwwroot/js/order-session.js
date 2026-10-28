(() => {
    "use strict";
    const pendingKey = "cafe.order.pending.v1";
    const trackingKey = "cafe.order.tracking.v1";
    const valid = key => typeof key === "string" && /^[A-Za-z0-9_-]{42}[AEIMQUYcgkosw048]$/.test(key);
    function read(key) {
        const text = sessionStorage.getItem(key);
        if (text === null) return null;
        const value = JSON.parse(text);
        if (!value || !valid(value.capability) || typeof value.started !== "boolean" ||
            (value.reference !== null && (typeof value.reference !== "string" || !value.started ||
                !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value.reference))))
            throw new Error("Private order storage is unavailable or invalid.");
        return { capability: value.capability, started: value.started, reference: value.reference };
    }
    function write(key, value) {
        const text = JSON.stringify(value);
        sessionStorage.setItem(key, text);
        if (sessionStorage.getItem(key) !== text) throw new Error("Private order storage could not be verified.");
        return read(key);
    }
    function generate() {
        if (!window.isSecureContext || !window.crypto?.getRandomValues)
            throw new Error("Secure private order storage is required.");
        const bytes = crypto.getRandomValues(new Uint8Array(32));
        const capability = btoa(String.fromCharCode(...bytes)).replaceAll("+", "-").replaceAll("/", "_").replace(/=+$/, "");
        return { capability, started: false, reference: null };
    }
    function requirePending(capability) {
        const pending = read(pendingKey);
        if (!pending || pending.capability !== capability) throw new Error("The pending order context changed. Check its outcome.");
        return pending;
    }
    window.cafeOrders = Object.freeze({
        readPending: () => read(pendingKey),
        ensureAttempt: () => read(pendingKey) ?? write(pendingKey, generate()),
        markStarted(capability) {
            const pending = requirePending(capability);
            return write(pendingKey, { ...pending, started: true });
        },
        markNotAccepted(capability) {
            const pending = requirePending(capability);
            if (pending.reference) throw new Error("This order was already saved.");
            return write(pendingKey, { ...pending, started: false });
        },
        markSaved(capability, reference) {
            const pending = requirePending(capability);
            const saved = write(pendingKey, { ...pending, started: true, reference });
            write(trackingKey, saved);
            return saved;
        },
        newAttempt() {
            const pending = read(pendingKey);
            if (pending && !pending.reference) throw new Error("Check the pending order before starting another.");
            return write(pendingKey, generate());
        },
        privateLink(capability, trustedOrigin) {
            if (!valid(capability)) throw new Error("Invalid private context.");
            const origin = new URL(trustedOrigin);
            if (origin.protocol !== "https:" || origin.username || origin.password || origin.pathname !== "/" || origin.search || origin.hash)
                throw new Error("Invalid configured public origin.");
            return new URL("/orders/track#key=" + capability, origin.origin).href;
        },
        trackingContext() {
            if (location.hash) {
                const fragment = location.hash.slice(1);
                // Remove valid and invalid credentials before loading any private order data.
                history.replaceState(null, "", location.pathname + location.search);
                sessionStorage.removeItem(trackingKey);
                if (!/^key=[A-Za-z0-9_-]{43}$/.test(fragment) || !valid(fragment.slice(4)))
                    throw new Error("This private order link is invalid.");
                const saved = write(trackingKey, { capability: fragment.slice(4), started: true, reference: null });
                return saved.capability;
            }
            return read(trackingKey)?.capability ?? null;
        },
        async copyLink(capability, trustedOrigin) {
            await navigator.clipboard.writeText(window.cafeOrders.privateLink(capability, trustedOrigin));
        }
    });
})();
