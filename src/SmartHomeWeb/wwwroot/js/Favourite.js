function toggleFavourite(button) {
    button.classList.toggle('active');
    const img = button.querySelector('img');
    const isActive = button.classList.contains('active');

    img.src = isActive ? button.getAttribute('data-icon-active') : button.getAttribute('data-icon-default');
}