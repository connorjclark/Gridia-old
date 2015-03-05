package hoten.gridia.scripting

class EventDispatcher {
    def registeredEvents = [:]
    
    def addEventListener(String type, closure) {
        type = type.toUpperCase() // :(
        if (registeredEvents[type] == null) {
            registeredEvents[type] = []
        }
        registeredEvents[type] += closure
    }
    
    def dispatch(type, event) {
        type = type.toUpperCase() // :(
        registeredEvents[type].each {
            it.event = event
            it.call(event)
        }
    }
}

