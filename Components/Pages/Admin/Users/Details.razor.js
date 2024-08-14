export function onLoad() {
    console.log('Loaded');
    addEvent()
}

export function onUpdate() {
    console.log('Updated');
}

export function onDispose() {
    console.log('Disposed');
}

function addEvent() {
    const form = document.getElementById("deleteForm")
    if (!form) {
        console.error("Could not find form element!")
        return;
    }
    const buttonEl = document.getElementById("deleteBtn")
    if (!buttonEl) {
        console.error("Could not find submit button for form!")
        return;
    }

    const spinnerEl = document.createElement("span");
    spinnerEl.classList.add("spinner-border", "spinner-border-sm");
    spinnerEl.setAttribute("role", "status");
    spinnerEl.setAttribute("aria-hidden", "true");

    form.addEventListener('submit', (e) => {
        e.preventDefault()
        
        const result = window.confirm("Are you sure you want to delete this user?")
        if (!result) {
            console.log("ADMIN USER DELETE: Operation aborted.")
            return;
        } else {
            buttonEl.disabled = true;
            buttonEl.innerText = "Processing, please wait..."
            buttonEl.append(" ", spinnerEl)
            e.currentTarget.submit()
        }
    })
}