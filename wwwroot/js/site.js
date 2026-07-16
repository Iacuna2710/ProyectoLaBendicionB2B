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
        function actualizarBotonTema(tema) {
            const icon = themeToggle.querySelector('i');
            icon.className = tema === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
            themeToggle.innerHTML = icon.outerHTML + ' ' + (tema === 'dark' ? 'Modo claro' : 'Modo oscuro');
        }

        // El tema ya se aplicó en el <head> (antes de pintar la página);
        // aquí solo se sincroniza el texto/ícono del botón con lo que quedó activo.
        actualizarBotonTema(document.documentElement.getAttribute('data-bs-theme'));

        themeToggle.addEventListener('click', function () {
            const html = document.documentElement;
            const next = html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
            html.setAttribute('data-bs-theme', next);
            localStorage.setItem('tema', next);
            actualizarBotonTema(next);
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