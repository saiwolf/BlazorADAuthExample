export function onLoad() {
    console.log('Loaded');
    addEvent();
}

export function onUpdate() {
    console.log('Updated');
}

export function onDispose() {
    console.log('Disposed');
}

function addEvent() {
    const btn = document.getElementById("editUserSubmit")
    const form = document.getElementById("editUserForm")
    const spinnerEl = document.createElement("span");
    spinnerEl.classList.add("spinner-border", "spinner-border-sm");
    spinnerEl.setAttribute("role", "status");
    spinnerEl.setAttribute("aria-hidden", "true");

    btn.disabled = false;

    form.addEventListener("submit", (e) => {
        console.log("Submitting.");
        e.preventDefault();
        btn.disabled = true;
        btn.innerText = "Processing, please wait..."
        btn.append(" ", spinnerEl);
        e.currentTarget.submit();
    });
}