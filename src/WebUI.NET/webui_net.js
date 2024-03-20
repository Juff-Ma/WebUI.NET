
{
    class WebUINet {
        static DEBUG_MODE = false;
        // calls a CS function but does not return any result
        static fireCSFunction = (csFuncId, ...args) => this.#_callFireCSFunction(csFuncId, false, args);
        // calls a CS function and returns a promise that will resolve with the result
        static callCSFunction = (csFuncId, ...args) => this.#_callFireCSFunction(csFuncId, true, args);
        static #curCallNumber = 1234;
        static #jsCallIdToPromise = new Map();
        static #completedPromise = Promise.resolve();
        static #_callFireCSFunction(csFuncId, needsReturn, ...args) {
            const callId = this.#curCallNumber++;
            if (this.DEBUG_MODE)
                console.log(`executing js call: ${callId} a cs function: ${csFuncId} with args`, args);
            webui.call("webuiNet_Callback", csFuncId, callId, JSON.stringify(...args));//webui has a 16 arg max limit so we will just stringify args and deserialize in CS
            if (!needsReturn)
                return this.#completedPromise; //not sure if we should do this or return undefined
            let promise = new Promise((resolv) => this.#jsCallIdToPromise.set(callId, resolv));

            return promise;
        }

        /// Options are standard options like capture, once, passive, abortKey is for a mantained internal db of AbortHandlers
        static addCSEventListener(type, elementID, csFuncId, additionalProps, options, abortKey) {
            if (this.DEBUG_MODE)
                console.log(`addCSEventListener on element: ${elementID} for type: ${type}`);
            let abortController = undefined;
            if (abortKey) {
                abortController = this.#abortKeyToHandlerMap.get(abortKey);
                if (abortController == undefined) {
                    abortController = new AbortController();
                    this.#abortKeyToHandlerMap.set(abortKey, abortController);
                }
            }
            let elem = document.getElementById(elementID);
            if (!elem) {
                if (elementID == "window")
                    elem = window;
                else if (elementID == "document")
                    elem = document;
                else {
                    if (this.DEBUG_MODE)
                        console.warn(`cannot find the element to add listener to of id: ${elementID}`);
                    return;
                }
            }
            let opts = { passive: true, signal: abortController };
            Object.assign(opts, options);
            if (additionalProps)
                additionalProps = JSON.parse(additionalProps);
            elem.addEventListener(type, new InternalEventRecord(csFuncId, additionalProps), opts);
        }
        static #abortKeyToHandlerMap = new Map();
        static setCSFunctionResult(jsCallId, result) {
            if (this.DEBUG_MODE)
                console.log(`Setting cs result on jsCall: ${jsCallId} to: ${result}`);
            const promise = this.#jsCallIdToPromise.get(jsCallId);
            if (!promise)
                return;
            promise(result);
        }
        static addCSFunction(csFuncId, registeredName, hasResult) {
            window[registeredName] = hasResult ? this.callCSFunction.bind(null, csFuncId) : this.fireCSFunction.bind(null, csFuncId);
        }
    }
    class SerializableMap extends Map {
        toJSON = () => Object.fromEntries(this);
    }
    class InternalEventRecord {
        constructor(csFuncId, captureProps) {
            this.csFuncId = csFuncId;
            if (captureProps && typeof captureProps[Symbol.iterator] !== 'function')//this is a similar call to what map does
                captureProps = Object.entries(captureProps);
            this.captureProps = captureProps ? new Map(captureProps) : undefined;
        }
        csFuncId;
        captureProps; //map to the additional properties to capture
        static #getEventVal = (obj, path) => path.split('.').reduce((a, v) => (a ? a[v] : undefined), obj);

        /**  @param {Event} evt */
        handleEvent(evt) {
            if (WebUINet.DEBUG_MODE)
                console.log("Event listener callback got event: ", evt);

            const eventInfo = new EventInfo();
            eventInfo.currentTargetId = evt.currentTarget?.id;
            eventInfo.originalTargetId = evt.target?.id;
            eventInfo.timestamp = performance.timing.navigationStart + evt.timeStamp;
            eventInfo.type = evt.type;
            if (this.captureProps) {
                eventInfo.additionalProps = new SerializableMap();
                this.captureProps.forEach((val, key) => eventInfo.additionalProps.set(key, InternalEventRecord.#getEventVal(evt, val)));
            }
            WebUINet.fireCSFunction(this.csFuncId, eventInfo);
        }
    }
    class EventInfo {
        currentTargetId;
        originalTargetId;
        timestamp;
        type;
        additionalProps; //map of additional property values requested
    }
    window.WebUINet = WebUINet;
}