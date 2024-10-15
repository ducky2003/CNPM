const currentLayout = localStorage.getItem("layout") || "light";
const currentMenu = localStorage.getItem("menu") || "side";
const currentSidebar = localStorage.getItem("sidebar");
const customizerBtn = document.querySelector(".customizer-trigger");
const customizer = document.querySelector(".customizer-wrapper");
const customizerClose = document.querySelector(".customizer-close");
const customizerOverlay = document.querySelector(".customizer-overlay");
const sidebarToggle = $(".sidebar-toggle");
function sidebarCollapse(e) {
  e.preventDefault();
  if (currentMenu)
  localStorage.setItem("sidebar", currentSidebar === "collapsed" ? "" : "collapsed");
  document.querySelector(".sidebar").classList.toggle("collapsed");
  document.querySelector(".contents").classList.toggle("expanded");
}
if (sidebarToggle) {
  sidebarToggle.on("click", sidebarCollapse);
}
function closeCustomizer(e) {
    e.preventDefault();
    customizer.classList.remove("show");
    customizerBtn.classList.remove("show");
    customizerOverlay.classList.remove("show");
  }
function loadThemeFromLocal(){
  if (currentSidebar === "collapsed") {
    document.querySelector(".sidebar").classList.add("collapsed");
    document.querySelector(".contents").classList.add("expanded");
  }
    if (currentLayout === "dark") {
    const darkBanner = document.querySelector("#dark-banner-change");
    darkBanner?.classList?.add("active");
      $("body").removeClass("layout-light");
      $("body").addClass("layout-dark");
      }
    if (currentLayout === "light") {
    const lightBanner = document.querySelector("#light-banner-change");
    lightBanner?.classList?.add("active");
      $("body").removeClass("layout-dark");
      $("body").addClass("layout-light");
      }
    if (currentMenu === "side") {
      $("body").removeClass("top-menu");
      $("body").addClass("side-menu");
    }
    if (currentMenu === "top") {
        $('ul.l_navbar li.side a,.l_navbar a.side').removeClass('active');
        $('ul.l_navbar li.top a,.l_navbar a.top').addClass('active');
        $("body").removeClass("side-menu");
        $("body").addClass("top-menu");
        }
}
  loadThemeFromLocal();
  /* Sidebar Change */
  const layoutChangeBtns = document.querySelectorAll("[data-layout]");
  // Get current layout from localStorage
  function changeLayout(e) {
    e.preventDefault();
    if (this.dataset.layout === "light") {
      localStorage.setItem("layout", "light");
      $('ul.l_sidebar li a,.l_sidebar a').removeClass('active');
      $(this).addClass("active");
      $("body").removeClass("layout-dark");
      $("body").addClass("layout-light");
    } else if (this.dataset.layout === "dark") {
      localStorage.setItem("layout", "dark");
      $('ul.l_sidebar li a,.l_sidebar a').removeClass('active');
      $(this).addClass("active");
      $("body").removeClass("layout-light");
      $("body").addClass("layout-dark");
    }
    if (this.dataset.layout === "side") {
      localStorage.setItem("menu", "side");
      $('ul.l_navbar li a,.l_navbar a').removeClass('active');
      $(this).addClass("active");
      $("body").removeClass("top-menu");
      $("body").addClass("side-menu");
    } else if (this.dataset.layout === "top") {
      localStorage.setItem("menu", "top");
      $('ul.l_navbar li a,.l_navbar a').removeClass('active');
      $(this).addClass("active");
      $("body").removeClass("side-menu");
      $("body").addClass("top-menu");
    }
  }
  $('.enable-dark-mode').click(function () {
    $("body").toggleClass('layout-dark');
    $('.enable-dark-mode a').toggleClass('active');
  });

  if (layoutChangeBtns) {
    layoutChangeBtns.forEach((layoutChangeBtn) =>
      layoutChangeBtn.addEventListener("click", changeLayout)
    );
    layoutChangeBtns.forEach((layoutChangeBtn) =>
      layoutChangeBtn.addEventListener("click", closeCustomizer)
    );
  }
$("select").select2({
    // options 
    searchInputPlaceholder: 'Tìm kiếm...',
    "language": {
      "noResults": function(){
          return "Không tìm thấy kết quả nào";
      }
  },
});
