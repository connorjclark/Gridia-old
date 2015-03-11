package hoten.gridia.scripting

class EventDispatcher {
    def registeredEvents = [:].withDefault { [] } 
    
    def addEventListener(String type, closure) {
        registeredEvents[type] += closure
    }
    
    def dispatch(type, event) {
        registeredEvents[type.toUpperCase()].each {
            it.event = event
            it.call()
        }
    }
}
