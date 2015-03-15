package com.hoten.gridia.scripting

class EventDispatcher {    
    def addEventListener(String type, Closure closure, Entity eventTarget) {
        closure.eventType = type
        closure.eventTarget = eventTarget
        eventTarget.registeredEvents[type] += closure
    }
    
    def removeEventListener(String type, Closure closure, Entity eventTarget) {
        eventTarget.registeredEvents[type] -= closure
    }
    
    def dispatch(String type, Entity eventTarget, Map event) {
        def listeners = eventTarget.registeredEvents[type.toUpperCase()]
        processEvent(listeners, event)
    }
    
    private def processEvent(listeners, event) {
        listeners.collect {
            it.event = event
            it.call()
        }.findAll { it != null }
    }
}
