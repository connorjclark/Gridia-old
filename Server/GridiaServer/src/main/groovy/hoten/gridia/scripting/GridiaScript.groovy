package hoten.gridia.scripting

import hoten.gridia.serving.ServingGridia
import hoten.gridia.content.Item;

class GridiaScript {
    def ServingGridia server
    def EventDispatcher eventDispatcher
    
    def GridiaScript(ServingGridia server, EventDispatcher eventDispatcher) {
        this.server = server
        this.eventDispatcher = eventDispatcher
    }
    
    def announce(params) {
        server.announce(params.from, params.message)
    }
    
    def propertyMissing(String name) {
        if (name == "ItemClass") {
            Item.ItemClass
        } else {
            throw new MissingPropertyException(name, GridiaScript)
        }
    }
    
    def methodMissing(String name, args) {
        if (name.startsWith("listenFor") && args.length == 1 && args[0] instanceof Closure) {
            def type = name.replaceFirst("listenFor", "")
            eventDispatcher.addEventListener(type, args[0])
        } else if (['start', 'update', 'end'].every { it != name } ) {
            throw new MissingMethodException(name, GridiaScript, args)
        }
    }
}

