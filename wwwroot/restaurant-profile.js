const menuTabs = Array.from(document.querySelectorAll("[data-menu-tab]"));

function activateMenuTab(tab, moveFocus = false) {
  const selectedIndex = menuTabs.indexOf(tab);
  menuTabs.forEach((item, index) => {
    const selected = index === selectedIndex;
    item.setAttribute("aria-selected", String(selected));
    const panel = document.getElementById(item.getAttribute("aria-controls"));
    if (panel) {
      panel.hidden = !selected;
    }
  });

  if (moveFocus) {
    tab.focus();
  }
}

menuTabs.forEach((tab) => {
  tab.addEventListener("click", () => activateMenuTab(tab));
  tab.addEventListener("keydown", (event) => {
    const currentIndex = menuTabs.indexOf(tab);
    let nextIndex;
    if (event.key === "ArrowRight") {
      nextIndex = (currentIndex + 1) % menuTabs.length;
    } else if (event.key === "ArrowLeft") {
      nextIndex = (currentIndex - 1 + menuTabs.length) % menuTabs.length;
    }

    if (nextIndex !== undefined) {
      event.preventDefault();
      activateMenuTab(menuTabs[nextIndex], true);
    }
  });
});

const galleryDialog = document.getElementById("gallery-dialog");
const galleryImage = document.getElementById("gallery-dialog-image");

if (galleryDialog && galleryImage) {
  document.querySelectorAll("[data-gallery-src]").forEach((trigger) => {
    trigger.addEventListener("click", () => {
      galleryImage.src = trigger.dataset.gallerySrc;
      galleryImage.alt = trigger.dataset.galleryAlt ?? "Restaurant photo";
      galleryDialog.showModal();
    });
  });
}
