// JsObjectManager namespace
Lagoon.JsObjectManager = (function () {

    // Key used to identify the link between C# JsObjectRef & Javascript (json) object
    const jsRefKey = '__jsObjectRefId';

    // Store JS object for which we need to keep a reference
    var jsObjectRefs= {};

    // Incremental counter for creating index
    var jsObjectRefId = 0;

    // Return the public object
    return {

        // Store a js object and return it's reference (in the jsObjectRefs Collection)
        Add: function (obj) {
            var id = jsObjectRefId++;
            jsObjectRefs[id] = obj;
            var jsRef = {};
            jsRef[jsRefKey] = id;
            return jsRef;
        },

        // Return a js object corresponding to the ref
        Get: function (ref) {
            return jsObjectRefs[ref];
        },

        // Remove an object from the store
        Free: function (ref) {
            jsObjectRefs[ref] = null;
            delete jsObjectRefs[ref];
        },

        // Call the function 'fn' with it's args and get the result
        // Add the result into the collection of js object reference
        // Return the reference to the object created by 'fn'
        ScriptGetNewRef: function (fn, args) {
            // Execute a function by it's fully qualified name (eg. Namespace.FunctionToExecute)
            function executeFunctionByName(functionName, args) {
                // Split the fn to call
                var namespaces = functionName.split(".");
                // Extraction function name
                var func = namespaces.pop();
                var context = window;
                // Start from the window object and follow the namespace
                for (var i = 0; i < namespaces.length; i++) {
                    context = context[namespaces[i]];
                }
                // Return the corresponding function
                return context[func].apply(context, args);
            }
            // Execute the given function and add it's result to the jsObjectRefs collection
            return this.Add(executeFunctionByName(fn, args));
        },

        // Check if value is an JsObjectReference, and if it's the case return the JS object corresponding to the key
        Reviver: function(key, value) {
            if (value && typeof value === 'object' && value.hasOwnProperty(jsRefKey) && typeof value[jsRefKey] === 'number') {
                var id = value[jsRefKey];
                if (!(id in jsObjectRefs)) {
                    throw new Error("This JS object reference does not exists : " + id);
                }
                const instance = jsObjectRefs[id];
                return instance;
            } else {
                // Unrecognized - let another reviver handle it
                return value;
            }
        }

    }

})();