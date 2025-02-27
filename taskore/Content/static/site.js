function showPage(pageId) {
    document.querySelectorAll('.auth-container').forEach(container => {
        container.classList.remove('active');
    });
    document.getElementById(pageId).classList.add('active');
}

function togglePassword(inputId) {
    const passwordInput = document.getElementById(inputId);
    const toggleBtn = passwordInput.nextElementSibling;

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleBtn.textContent = 'Hide';
    } else {
        passwordInput.type = 'password';
        toggleBtn.textContent = 'Show';
    }
}

document.querySelectorAll('form').forEach(form => {
    form.addEventListener('submit', (e) => {
        e.preventDefault();
        console.log('Form submitted:', e.target.id);
    });
});

function toggleEdit(section) {
    const textElement = document.getElementById(`${section}-text`);
    const editElement = document.getElementById(`${section}-edit`);
    const controls = textElement.nextElementSibling.nextElementSibling;

    textElement.classList.add('hidden');
    editElement.classList.add('active');
    controls.style.display = 'block';
