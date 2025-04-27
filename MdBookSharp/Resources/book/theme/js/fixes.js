document.getElementById("searchbar").placeholder = "Искать в правилах...";
document.getElementById("sidebar-toggle").title = "Содержание";
document.getElementById("theme-toggle").title = "Тема";
document.getElementById("search-toggle").title = "Поиск ()";
document.getElementById("print-button").parentElement.title = "Печать";

var navprev = document.getElementsByClassName("nav-chapters previous");
for (var i = 0; i < navprev.length; i++) 
navprev[i].title = "Назад";

var navprev = document.getElementsByClassName("nav-chapters next");
for (var i = 0; i < navprev.length; i++) 
navprev[i].title = "Вперёд";