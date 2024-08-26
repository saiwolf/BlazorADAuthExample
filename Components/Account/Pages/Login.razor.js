export function onLoad() {    
    console.log('Loaded');
    window.blazorUtils.SSRSpinnerSubmitSetup("loginSubmit", "Logging in, please wait...")
}

export function onUpdate() {
    console.log('Updated');
}

export function onDispose() {
    console.log('Disposed');
}