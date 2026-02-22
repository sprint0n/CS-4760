// Grant Portal — site.js

// ─── Dark Mode Toggle ────────────────────────────────────────────
// Theme preference is persisted in localStorage under 'grant-portal-theme'
// The <html> element gets data-theme="dark" when dark mode is active.
// An inline script in each layout's <head> applies the saved theme
// before paint to prevent flash-of-unstyled-content.
// ─────────────────────────────────────────────────────────────────

(function () {
    var THEME_KEY = 'grant-portal-theme';

    function isDark() {
        return document.documentElement.getAttribute('data-theme') === 'dark';
    }

    function updateToggleIcons(btn) {
        if (!btn) return;
        var moon = btn.querySelector('#icon-moon');
        var sun  = btn.querySelector('#icon-sun');
        var dark = isDark();
        if (moon) moon.style.display = dark ? 'none'         : 'inline-block';
        if (sun)  sun.style.display  = dark ? 'inline-block' : 'none';
        btn.setAttribute('title', dark ? 'Switch to light mode' : 'Switch to dark mode');
        btn.setAttribute('aria-label', dark ? 'Switch to light mode' : 'Switch to dark mode');
    }

    function setTheme(dark) {
        if (dark) {
            document.documentElement.setAttribute('data-theme', 'dark');
            localStorage.setItem(THEME_KEY, 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
            localStorage.setItem(THEME_KEY, 'light');
        }
        // Update all toggle buttons on the page (there may be one in the login view)
        document.querySelectorAll('#themeToggle').forEach(function (btn) {
            updateToggleIcons(btn);
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        // Wire up all toggle buttons
        document.querySelectorAll('#themeToggle').forEach(function (btn) {
            updateToggleIcons(btn); // set correct icon for current state

            btn.addEventListener('click', function () {
                setTheme(!isDark());
            });
        });
    });
})();
