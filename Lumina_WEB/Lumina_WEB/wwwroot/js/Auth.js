document.addEventListener('DOMContentLoaded', () => {

    /* MOSTRAR / OCULTAR CONTRASEÑA */
    const togglePassword = document.getElementById('togglePassword');

    togglePassword?.addEventListener('click', function () {
        const input = document.getElementById('password');
        const icon = this.querySelector('i');
        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';
        icon.className = isPassword ? 'bi bi-eye-slash' : 'bi bi-eye';
    });

    /* VALIDACIÓN + SPINNER AL ENVIAR */
    const form = document.querySelector('form');

    form?.addEventListener('submit', (e) => {

        if (!validatePasswords()) {
            e.preventDefault();
            return;
        }

        const btn = form.querySelector('button[type="submit"]');
        if (!btn) return;
        btn.disabled = true;
        btn.innerHTML = `
            <span class="spinner-border spinner-border-sm me-2"
                  role="status" aria-hidden="true">
            </span>
        `;
    });

    function validatePasswords() {
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        const msg = document.getElementById('match-msg');

        if (!msg) return true; // si no está en la página, no valida (ej: login)

        if (password !== confirmPassword) {
            msg.innerHTML = '<span style="color: red;">❌ Las contraseñas no coinciden</span>';
            return false;
        }

        msg.innerHTML = '';
        return true;
    }
    
        // Doble clic en el título "Lúmina" activa/desactiva el modo administrador.
        var titulo = document.getElementById('tituloLogin');
        var campo = document.getElementById('modoAdmin');
        var subtitulo = document.getElementById('subtituloLogin');

        function setModoAdmin(activo) {
        campo.value = activo ? 'true' : 'false';
        titulo.style.color = activo ? '#c2410c' : '';
        if (subtitulo) {
        subtitulo.textContent = activo ? 'Iniciar Sesión · Modo Admin' : 'Iniciar Sesión';
    }
    }

        if (titulo && campo) {
        titulo.addEventListener('dblclick', function (e) {
            e.preventDefault();
            setModoAdmin(campo.value !== 'true');
        });
    }
});