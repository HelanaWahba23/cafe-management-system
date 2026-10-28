(() => {
    const modal = document.getElementById("components-reconnect-modal");
    const content = document.getElementById("app-content");
    const state = document.getElementById("reconnect-state");
    const reload = document.getElementById("reconnect-reload");
    let reloadRequired = false;

    function pauseEditing() {
        const firstInterruption = !reloadRequired;
        reloadRequired = true;
        modal.dataset.reloadRequired = "true";
        content.inert = true;
        if (firstInterruption) reload.focus();
    }

    function updateStatus() {
        const classes = modal.classList;
        if (!navigator.onLine || classes.contains("components-reconnect-show") ||
            classes.contains("components-reconnect-failed") || classes.contains("components-reconnect-rejected")) {
            pauseEditing();
        }
        if (!reloadRequired) return;
        state.textContent = !navigator.onLine ? "You are offline." :
            classes.contains("components-reconnect-rejected") ? "The session ended. Reload to continue." :
            classes.contains("components-reconnect-failed") ? "Unable to reconnect. Reload when the connection is available." :
            classes.contains("components-reconnect-show") ? "Reconnecting…" :
            "Connection available. Reload to continue.";
    }

    // .NET 8 reports reconnect state through these CSS classes, not newer framework events.
    new MutationObserver(updateStatus).observe(modal, { attributes: true, attributeFilter: ["class"] });
    window.addEventListener("offline", updateStatus);
    window.addEventListener("online", updateStatus);
    reload.addEventListener("click", () => window.location.reload());
    document.addEventListener("submit", event => {
        if (event.target.matches("form[data-admin-write], form[data-order-write]") && (reloadRequired || !navigator.onLine)) {
            event.preventDefault();
            event.stopImmediatePropagation();
            pauseEditing();
        }
    }, true);
    updateStatus();
})();
