// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', () => { });

self.addEventListener('storage', () => {
    // When local storage changes, dump the list to
    // the console.
    console.log('Sorage Changed JS2');
});