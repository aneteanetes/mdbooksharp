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


document.addEventListener("DOMContentLoaded", () => {
    if (window.location.hash) {
        const targetId = decodeURIComponent(window.location.hash).replace('#', '');
        const targetElement = document.getElementById(targetId);

        if (targetElement) {
            targetElement.scrollIntoView(); 
        }
    }
    const headerLinks = document.querySelectorAll(".page-headers-tree .header-link");
    if (headerLinks.length === 0) return;

    const elementsMap = [];
    headerLinks.forEach(link => {
        const id = decodeURIComponent(link.getAttribute("href").replace("#", ""));
        const element = document.getElementById(id);
        if (element) {
            elementsMap.push({ link, element });
        }
    });

    const updateActiveHeader = () => {
        let activeLink = null;
        const triggerPoint = 100;

        if (elementsMap.length > 0) {
            activeLink = elementsMap[0].link;
        }

        let closestDistance = -Infinity;

        for (let i = 0; i < elementsMap.length; i++) {
            const { element, link } = elementsMap[i];
            const rect = element.getBoundingClientRect();

            if (rect.top <= triggerPoint) {
                if (rect.top > closestDistance) {
                    closestDistance = rect.top;
                    activeLink = link;
                }
            }
        }

        if (elementsMap.length > 0) {
            const lastIndex = elementsMap.length - 1;
            const lastRect = elementsMap[lastIndex].element.getBoundingClientRect();
            const scrollHeight = document.documentElement.scrollHeight;
            const scrollTop = window.scrollY || window.pageYOffset;
            const clientHeight = window.innerHeight;

            if ((scrollTop + clientHeight >= scrollHeight - 20) && lastRect.top < clientHeight) {
                activeLink = elementsMap[lastIndex].link;
            }
        }

        headerLinks.forEach(link => {
            if (link === activeLink) {
                link.classList.add("active");
            } else {
                link.classList.remove("active");
            }
        });
    };




    window.addEventListener("scroll", updateActiveHeader, { passive: true });

    updateActiveHeader();
});