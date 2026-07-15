document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function () {
            sidebar.classList.toggle('show');
        });
        document.addEventListener('click', function (e) {
            if (window.innerWidth < 768 && !sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                sidebar.classList.remove('show');
            }
        });
    }

    const themeToggle = document.getElementById('themeToggle');
    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            const html = document.documentElement;
            const current = html.getAttribute('data-bs-theme');
            const next = current === 'dark' ? 'light' : 'dark';
            html.setAttribute('data-bs-theme', next);
            const icon = this.querySelector('i');
            icon.className = next === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
            this.innerHTML = icon.outerHTML + ' ' + (next === 'dark' ? 'Modo claro' : 'Modo oscuro');
        });
    }

    setTimeout(function () {
        document.querySelectorAll('.alert-dismissible').forEach(function (el) {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(el);
            bsAlert.close();
        });
    }, 4000);

    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
        new bootstrap.Tooltip(el);
    });

    // Mostrar/ocultar contraseña en Login y Registro: el ícono solo aparece
    // mientras el campo tiene texto escrito.
    document.querySelectorAll('.password-wrapper').forEach(function (wrapper) {
        const input = wrapper.querySelector('input');
        const btn   = wrapper.querySelector('.password-toggle-btn');
        if (!input || !btn) return;

        const icon = btn.querySelector('i');

        input.addEventListener('input', function () {
            btn.style.display = input.value.length > 0 ? 'block' : 'none';
        });

        btn.addEventListener('click', function () {
            const mostrando = input.type === 'text';
            input.type = mostrando ? 'password' : 'text';
            icon.className = mostrando ? 'bi bi-eye' : 'bi bi-eye-slash';
        });
    });
});