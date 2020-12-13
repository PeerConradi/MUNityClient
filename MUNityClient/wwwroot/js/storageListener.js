function registerStorageListener(resolutionService) {
    console.log('Register local Storage listener.');
    window.addEventListener('storage', () => {
        // When local storage changes, dump the list to
        // the console.
        console.log("storage changed!");
    });
    window.onsotrage = () => {
        console.log("Storage changed 2.")
    };
    onsotrage = () => {
        console.log("Storage 3");
    };
    self.onstorage = () => {
        console.log("Storage 4");
    };
    addEventListener('storage', () => {
        // When local storage changes, dump the list to
        // the console.
        console.log("storage changed!");
    });
}
