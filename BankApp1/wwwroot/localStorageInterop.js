window.localStorageInterop = {
    save: function (key, json) {
        localStorage.setItem(key, json);
    },
    load: function (key) {
        return localStorage.getItem(key);
    },
    remove: function (key) {
        localStorage.removeItem(key);
    }
};
