let state = null;
function beforeLink(event) {
    if (!state?.dirty || event.defaultPrevented || event.button !== 0
        || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return;
    const link = event.target.closest?.('a[href]');
    if (!link || link.hasAttribute('download') || link.target && link.target !== '_self') return;
    const target = new URL(link.href, location.href);
    if (!['http:', 'https:'].includes(target.protocol)) return;
    if (target.origin === location.origin && target.pathname === location.pathname && target.search === location.search) return;
    event.preventDefault();
    event.stopImmediatePropagation();
    if (!state.busy) state.callback.invokeMethodAsync('RequestLeave', target.href);
}
function beforeUnload(event) {
    if (!state?.dirty) return;
    event.preventDefault();
    event.returnValue = '';
}
export function update(id, dirty, busy, callback) {
    if (!state) { document.addEventListener('click', beforeLink, true); window.addEventListener('beforeunload', beforeUnload); }
    state = { id, dirty, busy, callback };
}
export function leave(id, target) {
    if (state?.id !== id) return;
    state.dirty = false;
    location.assign(target);
}
export function remove(id) {
    if (state?.id !== id) return;
    document.removeEventListener('click', beforeLink, true);
    window.removeEventListener('beforeunload', beforeUnload);
    state = null;
}
