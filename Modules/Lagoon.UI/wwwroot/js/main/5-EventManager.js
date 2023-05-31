window.Lagoon = window.Lagoon || {};

class EventManager {

    //#region fields   
    events = [];
    debug = false;
    //#endregion

    constructor(element) {
        const self = this;
        // Create or update Dispose method
        if (element && !element.Dispose) {
            element.Dispose = () => {
                self.RemoveAll();
            };
        }
    }

    /**
     * Add event
     * @param {any} target
     * @param {any} type
     * @param {any} callback
     * @param {any} capture
     */
    Add(target, type, callback, capture = false, replace = false) {
        let finalTarget = target;
        // Get html element
        if (typeof finalTarget === 'string') {
            finalTarget = document.querySelectorAll(finalTarget);
        } else {
            finalTarget = [target];
        }
        if (finalTarget.length === 0) throw `EventProxy - "${target}" not exists.`;
        finalTarget.forEach(element => {
            // Remove event if needed
            if (replace) {
                const event = this.events.find(event => {
                    return event.Target === element && event.Type === type;
                });
                if (event) {
                    this.RemoveByTargetType(event.Target, event.Type);
                }
            }
            // Create event object
            const guid = Lagoon.EventManager.GetGuid();
            this.events.push({
                Key: guid,
                Target: element,
                Type: type,
                Callback: callback,
                Capture: capture
            });
            element.addEventListener(type, callback, capture);
            if (this.debug) console.log("XXX Add event => ", element, type, callback, capture);
        });        
    }

    /**
     * Get event by key
     * @param {any} key
     */
    GetByKey(key) {
        return this.events.find(event => {
            return event.Key === key;
        });
    }

    /**
     * Get event by target and type
     * @param {any} target
     * @param {any} type
     */
    GetByType(target, type) {
        return this.events.find(event => {
            return event.Target === target && event.Type === type;
        });
    }

    /**
     * Remove event by its key
     * @param {any} key
     */
    RemoveByKey(key) {        
        this.events.every((event, idx) => {
            if (event.Key === key) {
                event.Target.removeEventListener(event.Type, event.Callback, event.Capture);
                this.events.splice(idx, 1);
                if (this.debug) console.log("XXX RemoveByKey => ", event.Target, event.Type);
                return false;
            }
        });
    }

    /**
     * Remove all target events
     * @param {any} target
     */
    RemoveByTarget(target) {
        let toRemove = [];
        this.events.forEach((event, idx) => {
            if (event.Target === target) {
                event.Target.removeEventListener(event.Type, event.Callback, event.Capture);                
                toRemove.push(idx);                
                if (this.debug) console.log("XXX RemoveByTarget => ", event.Target, event.Type);
            }
        });
        toRemove.reverse().forEach(idx => {
            this.events.splice(idx, 1);
        });
    }

    /**
     * Remove event by its target and type
     * @param {any} target
     * @param {any} type
     */
    RemoveByTargetType(target, type) {
        let toRemove = [];
        this.events.forEach((event, idx) => {
            if (event.Target === target && event.Type === type) {
                event.Target.removeEventListener(event.Type, event.Callback, event.Capture);
                toRemove.push(idx);
                if (this.debug) console.log("XXX RemoveByTargetType => ", event.Target, event.Type);
            }
        });
        toRemove.reverse().forEach(idx => {
            this.events.splice(idx, 1);
        });
    }

    /**
     * Remove all events linked to the EventManager
     * */
    RemoveAll() {
        this.events.forEach(event => {
            event.Target.removeEventListener(event.Type, event.Callback, event.Capture);
            if (this.debug) console.log("XXX RemoveAll => ", event.Target, event.Type);
        });
        this.events = [];
    }

    /**
     * Dispose events
     * */
    Dispose() {
        this.RemoveAll();
    }

    /**
     * Return new GUID
     * */
    static GetGuid() {
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    }

}

Lagoon.EventManager = EventManager;