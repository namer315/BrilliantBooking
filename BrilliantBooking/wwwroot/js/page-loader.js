(function () {
    const loaderId = 'js-page-loader';
    let hideTimer;
    let navigationPending = false;

    function getLoader() {
        return document.getElementById(loaderId);
    }

    function show(message) {
        const loader = getLoader();
        if (!loader) {
            return;
        }

        if (message) {
            const text = loader.querySelector('.js-page-loader__text');
            if (text) {
                text.textContent = message;
            }
        }

        window.clearTimeout(hideTimer);
        navigationPending = true;
        loader.classList.add('is-visible');
        loader.setAttribute('aria-busy', 'true');
    }

    function hide() {
        const loader = getLoader();
        if (!loader) {
            return;
        }

        window.clearTimeout(hideTimer);
        navigationPending = false;
        loader.classList.remove('is-visible');
        loader.setAttribute('aria-busy', 'false');
    }

    function hideSoon() {
        window.clearTimeout(hideTimer);
        hideTimer = window.setTimeout(hide, 500);
    }

    function isModifiedClick(event) {
        return event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey;
    }

    function shouldShowForLink(event, link) {
        if (isModifiedClick(event) || link.target === '_blank' || link.hasAttribute('download')) {
            return false;
        }

        const href = link.getAttribute('href');
        if (!href || href.startsWith('#') || href.startsWith('mailto:') || href.startsWith('tel:')) {
            return false;
        }

        const destination = new URL(link.href, window.location.href);
        return destination.origin === window.location.origin
            && (destination.pathname !== window.location.pathname || destination.search !== window.location.search);
    }

    function initialize() {
        document.addEventListener('click', function (event) {
            const link = event.target instanceof Element ? event.target.closest('a') : null;
            if (link && shouldShowForLink(event, link)) {
                show('Loading page *');
                hideSoon();
            }
        }, true);

        window.addEventListener('beforeunload', function () {
            show('Loading page **');
        });

        window.addEventListener('load', hide);
        window.addEventListener('pageshow', hide);
        window.addEventListener('popstate', hide);
        document.addEventListener('visibilitychange', function () {
            if (document.visibilityState === 'visible') {
                hide();
            }
        });
        document.addEventListener('enhancedload', hide);

        const originalPushState = history.pushState;
        const originalReplaceState = history.replaceState;

        function handleHistoryNavigation(originalMethod, state, title, url) {
            const destination = url ? new URL(url, window.location.href) : window.location;
            const isDifferentPage = destination.pathname !== window.location.pathname
                || destination.search !== window.location.search;

            if (isDifferentPage) {
                show('Loading page ***');
                hideSoon();
            }

            return originalMethod.call(this, state, title, url);
        }

        history.pushState = function (state, title, url) {
            return handleHistoryNavigation.call(this, originalPushState, state, title, url);
        };

        history.replaceState = function (state, title, url) {
            return handleHistoryNavigation.call(this, originalReplaceState, state, title, url);
        };

        const observer = new MutationObserver(function (mutations) {
            if (!navigationPending) {
                return;
            }

            const loader = getLoader();
            const pageChanged = mutations.some(mutation =>
                mutation.target !== loader && !loader?.contains(mutation.target));

            if (pageChanged) {
                hideSoon();
            }
        });

        if (document.body) {
            observer.observe(document.body, { childList: true, subtree: true });
        }

        if (document.readyState === 'complete') {
            hide();
        }
    }

    window.pageLoader = { show, hide };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize, { once: true });
    } else {
        initialize();
    }
})();
    