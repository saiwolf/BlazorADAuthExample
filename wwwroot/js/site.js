/**
 * Function to set up a spinner wait animation for a form's submit event.
 * Requires Bootstrap 5.3.x https://getbootstrap.com/docs/5.3
 * @param {string} submitBtnId - Element Id of Form's submit button
 * @param {string} loadingMessage - Optional message to display during spinner event 
 */
function SSRSpinnerSubmitSetup(submitBtnId, loadingMessage = "Loading, please wait...") {    
    const btn = document.getElementById(submitBtnId)
    if (!btn) {
        console.error(`Unable to locate submit button element: '${btnName}'`)
        return;
    }
    const form = btn.closest('form')
    if (!form) {
        console.error(`Submit button element: '${btnName} must be in a form!'`)
        return;
    }
    const spinnerEl = document.createElement("span");
    spinnerEl.classList.add("spinner-border", "spinner-border-sm");
    spinnerEl.setAttribute("role", "status");
    spinnerEl.setAttribute("aria-hidden", "true");

    btn.disabled = false;

    form.addEventListener("submit", (e) => {        
        console.log("Submitting.");
        e.preventDefault();
        btn.disabled = true;
        btn.innerText = loadingMessage
        btn.append(" ", spinnerEl);
        e.currentTarget.submit();
    });
}

window.blazorUtils = {}

window.blazorUtils.SSRSpinnerSubmitSetup = SSRSpinnerSubmitSetup