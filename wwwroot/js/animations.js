/* ══════════════════════════════════════════════════════════════════
   JobBoard — animations.js  ·  Particle Canvas + Micro-interactions
   ══════════════════════════════════════════════════════════════════ */

(function () {
  'use strict';

  /* ── 1. Particle Canvas ─────────────────────────────────────────── */
  const canvas = document.getElementById('particles-canvas');
  if (canvas) {
    const ctx = canvas.getContext('2d');
    let particles = [];
    let mouse = { x: -999, y: -999 };
    let raf;

    function resize() {
      canvas.width  = window.innerWidth;
      canvas.height = window.innerHeight;
    }

    function Particle() {
      this.reset(true);
    }
    Particle.prototype.reset = function (initial) {
      this.x     = Math.random() * canvas.width;
      this.y     = initial ? Math.random() * canvas.height : canvas.height + 10;
      this.size  = Math.random() * 1.8 + 0.4;
      this.speed = Math.random() * 0.4 + 0.1;
      this.vx    = (Math.random() - 0.5) * 0.3;
      this.alpha = Math.random() * 0.5 + 0.05;
      this.hue   = Math.random() > 0.6 ? 260 : (Math.random() > 0.5 ? 250 : 190);
    };
    Particle.prototype.update = function () {
      this.y -= this.speed;
      this.x += this.vx;
      // Mouse repulsion
      const dx = this.x - mouse.x;
      const dy = this.y - mouse.y;
      const dist = Math.sqrt(dx * dx + dy * dy);
      if (dist < 120) {
        const angle = Math.atan2(dy, dx);
        const force = (120 - dist) / 120;
        this.x += Math.cos(angle) * force * 1.5;
        this.y += Math.sin(angle) * force * 1.5;
      }
      if (this.y < -10) this.reset(false);
    };
    Particle.prototype.draw = function () {
      ctx.save();
      ctx.globalAlpha = this.alpha;
      ctx.fillStyle = `hsl(${this.hue}, 80%, 70%)`;
      ctx.beginPath();
      ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
      ctx.fill();
      ctx.restore();
    };

    function initParticles() {
      particles = [];
      const count = Math.min(Math.floor(window.innerWidth / 10), 100);
      for (let i = 0; i < count; i++) particles.push(new Particle());
    }

    function drawConnections() {
      const maxDist = 100;
      for (let i = 0; i < particles.length; i++) {
        for (let j = i + 1; j < particles.length; j++) {
          const dx = particles[i].x - particles[j].x;
          const dy = particles[i].y - particles[j].y;
          const dist = Math.sqrt(dx * dx + dy * dy);
          if (dist < maxDist) {
            ctx.save();
            ctx.globalAlpha = (1 - dist / maxDist) * 0.08;
            ctx.strokeStyle = '#818cf8';
            ctx.lineWidth = 0.5;
            ctx.beginPath();
            ctx.moveTo(particles[i].x, particles[i].y);
            ctx.lineTo(particles[j].x, particles[j].y);
            ctx.stroke();
            ctx.restore();
          }
        }
      }
    }

    function animate() {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      particles.forEach(p => { p.update(); p.draw(); });
      drawConnections();
      raf = requestAnimationFrame(animate);
    }

    resize();
    initParticles();
    animate();

    window.addEventListener('resize', () => { resize(); initParticles(); });
    document.addEventListener('mousemove', e => { mouse.x = e.clientX; mouse.y = e.clientY; });
    document.addEventListener('mouseleave', () => { mouse.x = -999; mouse.y = -999; });
  }

  /* ── 2. Navbar scroll state ─────────────────────────────────────── */
  const navbar = document.querySelector('.navbar');
  if (navbar) {
    const onScroll = () => navbar.classList.toggle('scrolled', window.scrollY > 20);
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
  }

  /* ── 3. Scroll Reveal ───────────────────────────────────────────── */
  const revealObserver = new IntersectionObserver((entries) => {
    entries.forEach(e => {
      if (e.isIntersecting) {
        e.target.classList.add('visible');
        revealObserver.unobserve(e.target);
      }
    });
  }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

  document.querySelectorAll('.reveal').forEach(el => revealObserver.observe(el));

  /* ── 4. Animated Counters ───────────────────────────────────────── */
  function animateCounter(el) {
    const target = parseInt(el.textContent.replace(/\D/g, ''), 10);
    if (isNaN(target) || target === 0) return;
    const duration = 1200;
    const start = performance.now();
    const prefix = el.textContent.replace(/[\d,]/g, '').trim();

    function tick(now) {
      const elapsed = now - start;
      const progress = Math.min(elapsed / duration, 1);
      const eased = 1 - Math.pow(1 - progress, 4);
      el.textContent = Math.round(eased * target).toLocaleString() + (prefix ? ' ' + prefix : '');
      if (progress < 1) requestAnimationFrame(tick);
    }
    requestAnimationFrame(tick);
  }

  const counterObserver = new IntersectionObserver((entries) => {
    entries.forEach(e => {
      if (e.isIntersecting) {
        animateCounter(e.target);
        counterObserver.unobserve(e.target);
      }
    });
  }, { threshold: 0.5 });

  document.querySelectorAll('.stat-value').forEach(el => counterObserver.observe(el));

  /* ── 5. Alert auto-dismiss ──────────────────────────────────────── */
  document.querySelectorAll('.alert').forEach(alert => {
    setTimeout(() => {
      alert.style.transition = 'opacity 0.5s ease, transform 0.5s ease, max-height 0.5s ease, padding 0.5s ease, margin 0.5s ease';
      alert.style.opacity    = '0';
      alert.style.transform  = 'translateY(-8px) scale(0.97)';
      alert.style.maxHeight  = '0';
      alert.style.padding    = '0';
      alert.style.margin     = '0';
      setTimeout(() => alert.remove(), 600);
    }, 5000);
  });

  /* ── 6. Button ripple effect ────────────────────────────────────── */
  document.querySelectorAll('.btn').forEach(btn => {
    btn.addEventListener('click', function (e) {
      const rect   = btn.getBoundingClientRect();
      const ripple = document.createElement('span');
      const size   = Math.max(rect.width, rect.height) * 2;
      ripple.style.cssText = `
        position:absolute; border-radius:50%; pointer-events:none;
        width:${size}px; height:${size}px;
        left:${e.clientX - rect.left - size/2}px;
        top:${e.clientY  - rect.top  - size/2}px;
        background:rgba(255,255,255,0.15);
        transform:scale(0); animation:rippleFx 0.55s ease-out forwards;
      `;
      btn.style.position = 'relative';
      btn.style.overflow = 'hidden';
      btn.appendChild(ripple);
      setTimeout(() => ripple.remove(), 600);
    });
  });

  /* Ripple keyframes injected once */
  if (!document.getElementById('ripple-style')) {
    const style = document.createElement('style');
    style.id = 'ripple-style';
    style.textContent = `@keyframes rippleFx{to{transform:scale(1);opacity:0}}`;
    document.head.appendChild(style);
  }

  /* ── 7. Job card tilt effect ────────────────────────────────────── */
  document.querySelectorAll('.job-card').forEach(card => {
    card.addEventListener('mousemove', function (e) {
      const rect   = card.getBoundingClientRect();
      const cx     = rect.left + rect.width  / 2;
      const cy     = rect.top  + rect.height / 2;
      const dx     = (e.clientX - cx) / (rect.width  / 2);
      const dy     = (e.clientY - cy) / (rect.height / 2);
      card.style.transform = `translateY(-5px) scale(1.01) rotateX(${-dy * 3}deg) rotateY(${dx * 3}deg)`;
    });
    card.addEventListener('mouseleave', function () {
      card.style.transform = '';
    });
  });

  /* ── 8. Smooth form label float ─────────────────────────────────── */
  document.querySelectorAll('.form-control').forEach(input => {
    input.addEventListener('focus', () => {
      input.closest('.form-group')?.classList.add('focused');
    });
    input.addEventListener('blur', () => {
      input.closest('.form-group')?.classList.remove('focused');
    });
  });

  /* ── 9. Search bar typing animation ─────────────────────────────── */
  const searchInput = document.querySelector('.search-input-wrap input');
  if (searchInput && !searchInput.value) {
    const placeholders = [
      'Search for React Developer…',
      'Find remote UX jobs…',
      'Browse Marketing roles…',
      'Explore Finance careers…',
      'Search Software Engineer…',
    ];
    let idx = 0; let charIdx = 0; let deleting = false;
    let typingTimer;

    function type() {
      const current = placeholders[idx];
      if (!deleting) {
        charIdx++;
        searchInput.setAttribute('placeholder', current.slice(0, charIdx));
        if (charIdx === current.length) {
          deleting = true;
          typingTimer = setTimeout(type, 2200);
          return;
        }
      } else {
        charIdx--;
        searchInput.setAttribute('placeholder', current.slice(0, charIdx));
        if (charIdx === 0) {
          deleting = false;
          idx = (idx + 1) % placeholders.length;
        }
      }
      typingTimer = setTimeout(type, deleting ? 40 : 70);
    }

    searchInput.addEventListener('focus', () => clearTimeout(typingTimer));
    searchInput.addEventListener('blur', () => { charIdx = 0; deleting = false; type(); });
    type();
  }

  /* ── 10. Page load fade-in ──────────────────────────────────────── */
  document.body.style.opacity = '0';
  document.body.style.transition = 'opacity 0.4s ease';
  window.addEventListener('load', () => { document.body.style.opacity = '1'; });

  /* ── 11. Smooth anchor scroll ───────────────────────────────────── */
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
      const target = document.querySelector(this.getAttribute('href'));
      if (target) { e.preventDefault(); target.scrollIntoView({ behavior: 'smooth', block: 'start' }); }
    });
  });

  /* ── 12. Confirm dialogs with custom styling ────────────────────── */
  // Override native confirms for delete buttons
  document.querySelectorAll('form[data-confirm]').forEach(form => {
    form.addEventListener('submit', function (e) {
      if (!window.confirm(this.dataset.confirm)) e.preventDefault();
    });
  });

})();
