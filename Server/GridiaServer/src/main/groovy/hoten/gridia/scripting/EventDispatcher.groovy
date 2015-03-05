package hoten.gridia.scripting

class EventDispatcher {
    def registeredEvents = [:].withDefault { [] } 
    
    def addEventListener(String type, closure) {
        type = type.toUpperCase() // :(
        registeredEvents[type] += closure
    }
    
    def dispatch(type, event) {
        type = type.toUpperCase() // :(
        registeredEvents[type].each {
            it.event = event
            it.call()
        }
    }
}

