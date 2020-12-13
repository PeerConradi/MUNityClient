window.onstorage = () => {
    console.log('Storage changed js');
    DotNet.invokeMethodAsync('Managing.InteropLocalStorageJs', 'LocalStorageChanged');
}

