var oldWebSocket = window.WebSocket;
function WrappedWebSocket(loc) {
    this.prototype = new oldWebSocket(loc);
    this.__proto__ = this.prototype;
    var wrapper = this;
    this.onmessage = function (message) {
        try {
            var parsedData = JSON.parse(message.data);

            if (parsedData.SockPuppetCall) {
                var subContext = context;
                var namespaces = parsedData.SockPuppetCall.Name.split('.');

                var methodName = namespaces[namespaces.length - 1];

                if (namespaces.length > 1) {
                    namespaces.splice(namespaces.length - 1, 1);

                    for (i in namespaces) {
                        subContext = subContext[namespaces[i]];
                    }
                }

                var arguments = parsedData.SockPuppetCall.Arguments;

                subContext[methodName].apply(subContext, arguments);

                return;
            }
        } catch (e) { }
        wrapper.baseonmessage(message);
    }
    this.__defineSetter__('onmessage', function (val) {
        wrapper.baseonmessage = val;
    });

    var context = window;
    this.puppetContext = function (newContext) {
        context = newContext;
    }
}
window.WebSocket = WrappedWebSocket;

