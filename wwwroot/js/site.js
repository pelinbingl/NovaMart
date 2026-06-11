// ===== TOAST =====
function showToast(msg, duration = 3000) {
    const toast = document.getElementById('toast');
    const toastMsg = document.getElementById('toast-msg');
    if (!toast) return;
    toastMsg.textContent = msg;
    toast.classList.add('show');
    clearTimeout(toast._timer);
    toast._timer = setTimeout(() => toast.classList.remove('show'), duration);
}

// ===== SEPETE EKLE =====
function addToCart(productName) {
    // Sepet badge güncelle
    const badge = document.querySelector('.cart-badge');
    if (badge) {
        const current = parseInt(badge.textContent) || 0;
        badge.textContent = current + 1;
        badge.style.transform = 'scale(1.4)';
        setTimeout(() => badge.style.transform = '', 200);
    }
    showToast('🛒 "' + productName.substring(0, 24) + (productName.length > 24 ? '…' : '') + '" sepete eklendi!');
}

// ===== NAVBAR SCROLL =====
window.addEventListener('scroll', () => {
    const nav = document.querySelector('.navbar');
    if (nav) {
        if (window.scrollY > 20) {
            nav.style.background = 'rgba(10,10,15,0.97)';
        } else {
            nav.style.background = 'rgba(10,10,15,0.85)';
        }
    }
});

// ===== PAGE LOAD ANIMATION =====
document.addEventListener('DOMContentLoaded', () => {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

    document.querySelectorAll('.product-card, .category-card, .promo-card, .cart-item').forEach((el, i) => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = `opacity 0.5s ease ${i * 0.06}s, transform 0.5s ease ${i * 0.06}s, border-color 0.3s, box-shadow 0.3s, background 0.3s`;
        observer.observe(el);
    });
});

// ===== PAGINATION =====
document.querySelectorAll('.page-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        if (this.textContent === '...') return;
        document.querySelectorAll('.page-btn').forEach(b => b.classList.remove('active'));
        this.classList.add('active');
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
});
