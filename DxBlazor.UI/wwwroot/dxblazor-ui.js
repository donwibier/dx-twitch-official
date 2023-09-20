export function isDarkMode() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
}

export function addThemeSwitchListener(dotNetReference) {
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
        dotNetReference.invokeMethodAsync("OsThemeSwitched", event.matches);
    });
}