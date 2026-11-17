window.refreshReserveIcons = function () {
    if (window.lucide) {
        window.lucide.createIcons();
    }
};

window.setDarkMode = function (enabled) {
    document.documentElement.classList.toggle('dark', enabled);
};

window.setCookie = function (name, value, days = 30) {
    const maxAge = Math.max(0, Math.floor(days * 24 * 60 * 60));
    const secure = location.protocol === 'https:' ? '; Secure' : '';

    document.cookie = `${encodeURIComponent(name)}=${encodeURIComponent(value)}; path=/; max-age=${maxAge}; SameSite=Strict${secure}`;
};

window.removeCookie = function (name) {
    document.cookie = `${encodeURIComponent(name)}=; path=/; max-age=0; SameSite=Strict${location.protocol === 'https:' ? '; Secure' : ''}`;
};

