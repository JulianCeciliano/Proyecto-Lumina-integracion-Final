// =========================================================
// ProfileCreate_Scripts.js
// Micro-interacciones y manejo del formulario de Crear.cshtml
// =========================================================

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('profileForm');
    const overlay = document.getElementById('successOverlay');
    const icon = document.getElementById('successIcon');
    const pfpPreview = document.getElementById('pfp-preview');
    const pfpPlaceholder = document.getElementById('pfp-placeholder');
    const pfpWrapper = document.querySelector('.pfp-wrapper');
    const inputs = document.querySelectorAll('.custom-input');

    // Envío del formulario
    form.addEventListener('submit', (e) => {
        e.preventDefault();

        // Mostrar pantalla de éxito
        overlay.classList.add('show');
        setTimeout(() => {
            icon.classList.add('show');
        }, 300);
    });

    // Selección de imagen de perfil (mock)
    pfpWrapper.addEventListener('click', () => {
        // En una app real, esto abriría un selector de archivos
        pfpPlaceholder.classList.add('d-none');
        pfpPreview.classList.remove('d-none');
    });

    // Efecto de desplazamiento en inputs al enfocar
    inputs.forEach(input => {
        input.addEventListener('focus', () => {
            input.parentElement.classList.add('is-focused');
        });
        input.addEventListener('blur', () => {
            input.parentElement.classList.remove('is-focused');
        });
    });
});