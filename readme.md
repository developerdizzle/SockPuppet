#SockPuppet

Link a .NET object to a Javascript object using HTML5 WebSockets, and call Javascript object methods from your .NET code.

On your web page:

    //create a new WebSocket object as usual
    var server = new WebSocket("ws://localhost:8181");

    //set our context to the window object - all sockpuppet method calls will be performed on this object
    server.puppetContext(window);


In your server-side code:

    //create a dynamic Sockpuppet, passing a Func<string> used to send a message through your Websocket
    dynamic dynamicPuppet = SockPuppet.Puppet.New(r => socket.Send(r));

    //call any method on your client-side context object!
    dynamicPuppet.document.alert("I'm in your browser calling your methods!");
    
Also supports interfaces (that do not have to be explicitely implemented!):

    //create a strongly typed sockpuppet
    IClientWindow windowPuppet = SockPuppet.Puppet.New<IClientWindow>(r => socket.Send(r));
    
    //call any method on your client-side context object!
    windowPuppet.showMessage("This method was called from an interface on the server!");
    
The demo uses Fleck as a Websocket server, but you are free to use whichever you prefer.