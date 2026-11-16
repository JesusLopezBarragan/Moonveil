const navigationToggle = document.querySelector(".nav-toggle");
const navigation = document.querySelector("#site-nav");

if (navigationToggle && navigation) {
    navigationToggle.addEventListener("click", () => {
        const isExpanded = navigationToggle.getAttribute("aria-expanded") === "true";
        navigationToggle.setAttribute("aria-expanded", String(!isExpanded));
        navigationToggle.setAttribute("aria-label", isExpanded ? "Abrir navegación" : "Cerrar navegación");
        navigation.classList.toggle("is-open", !isExpanded);
    });

    navigation.querySelectorAll("a").forEach((link) => {
        link.addEventListener("click", () => {
            navigation.classList.remove("is-open");
            navigationToggle.setAttribute("aria-expanded", "false");
            navigationToggle.setAttribute("aria-label", "Abrir navegación");
        });
    });

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && navigation.classList.contains("is-open")) {
            navigation.classList.remove("is-open");
            navigationToggle.setAttribute("aria-expanded", "false");
            navigationToggle.setAttribute("aria-label", "Abrir navegación");
            navigationToggle.focus();
        }
    });
}

const discoverButton = document.querySelector("#boton");

if (discoverButton) {
    discoverButton.addEventListener("click", () => {
        window.alert("¡Gracias por visitar nuestro proyecto!");
    });
}

const revealElements = document.querySelectorAll("[data-reveal]");

if ("IntersectionObserver" in window && revealElements.length > 0) {
    document.documentElement.classList.add("reveal-ready");

    const revealObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("is-visible");
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12 });

    revealElements.forEach((element) => revealObserver.observe(element));
}
