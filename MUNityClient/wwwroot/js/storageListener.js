function registerStorageListener(resolutionService) {
    console.log('Register local Storage listener.');
    window.addEventListener('storage', () => {
        // When local storage changes, dump the list to
        // the console.
        resolutionService.invokeMethodAsync('StorageHasChanged');
        console.log("storage changed!");
    });
}
