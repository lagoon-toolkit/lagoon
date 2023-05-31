Lagoon.JsDicoManager = (function () {

    return {

        /**
         * Get translation from dictionnary
         * @param {string} key Dico key to retrieve 
         * @param {Array[string]} args optional args for String.Format
         */
        GetDico: function (key, args) {
            if (args == undefined || args == null) args = [];
            return DotNet.invokeMethod("Lagoon.UI", 'JsDicoTranslate', key, args);
        }

    }

})();