document.addEventListener("DOMContentLoaded", () => {
    const allItems = Array.from(document.querySelectorAll('.chapter-item'));

    allItems.forEach(item => {
        if (!item.classList.contains('collapsible')) return;
        const menuId = item.getAttribute('data-menu-id');

        item.addEventListener('click', (e) => {
            if (e.target.tagName === 'A') return;
            e.preventDefault();

            const isNowCollapsed = item.classList.toggle('collapsed');
            localStorage.setItem(`menu-collapsed-${menuId}`, isNowCollapsed);
        });
    });
});
