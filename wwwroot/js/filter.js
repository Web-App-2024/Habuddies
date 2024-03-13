function FilterManager(checkbox, group="check", scroller=null) {
    let checkboxes = document.getElementsByName(group);
    checkboxes.forEach((item) => {
        if (item !== checkbox) {
            item.checked = false;
            item.parentNode.classList.remove("active");
        }
    })

    checkbox.parentNode.classList.toggle("active");

    if (scroller !== null) {
        if (checkbox.checked) scroller.setCategory(checkbox.parentElement.textContent);
        else scroller.setCategory("");

        scroller.loadEvent(true);
    }
}

function ActiveFilterDisplay(elementId, display=false, newLabel="", newColor="") {
    let element = document.getElementById(elementId);
    if (display) {
        element.textContent = newLabel;
        element.style.display = "inline-block";
        element.style.backgroundColor = newColor;
        element.style.borderColor = newColor;
    }
    else {
        element.textContent = "";
        element.style.display = "none";
    }
}