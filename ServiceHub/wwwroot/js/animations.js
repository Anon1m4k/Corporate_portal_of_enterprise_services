// Плавное появление карточек при скролле
document.addEventListener('DOMContentLoaded', () => {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

    document.querySelectorAll('.glass-card').forEach(card => {
        card.style.opacity = '0';
        observer.observe(card);
    });
});

// Эффект волны на кнопках
document.addEventListener('click', function (e) {
    const target = e.target.closest('.btn-glass');
    if (!target) return;

    const circle = document.createElement('span');
    circle.classList.add('ripple');
    circle.style.width = circle.style.height = `${Math.max(target.offsetWidth, target.offsetHeight)}px`;
    circle.style.left = `${e.clientX - target.getBoundingClientRect().left - circle.offsetWidth / 2}px`;
    circle.style.top = `${e.clientY - target.getBoundingClientRect().top - circle.offsetHeight / 2}px`;
    circle.style.background = 'rgba(255,255,255,0.5)';
    circle.style.position = 'absolute';
    circle.style.borderRadius = '50%';
    circle.style.transform = 'scale(0)';
    circle.style.animation = 'ripple 0.6s ease-out';
    circle.style.pointerEvents = 'none';

    target.style.position = 'relative';
    target.style.overflow = 'hidden';
    target.appendChild(circle);

    circle.addEventListener('animationend', () => circle.remove());
});

// ripple animation
const style = document.createElement('style');
style.textContent = `
  @keyframes ripple {
    to { transform: scale(4); opacity: 0; }
  }
`;
document.head.appendChild(style);