package com.hoten.gridia.scripting

class EventDispatcher {    
    def addEventListener(String type, Closure callback, Entity eventTarget) {
        eventTarget.registeredEvents[type] += callback
    }
    
    def removeEventListener(String type, Closure callback, Entity eventTarget) {
        eventTarget.registeredEvents[type] -= callback
    }
    
    def dispatch(String type, Entity eventTarget, Map event) {
        ['before', 'on', 'after'].collect {
            def listeners = eventTarget.registeredEvents["$it$type".toUpperCase()]
            processEvent(listeners, event)
        }.flatten()
    }
    
    private def processEvent(listeners, event) {
        listeners.collect {
            it.call(event)
        }.findAll { it != null }
    }
}
