package com.hoten.gridia.scripting

class EventDispatcher {
    def registeredEvents = [:].withDefault { [] } 
    
    def addEventListener(String type, Closure closure, Entity eventTarget) {
        closure.eventType = type
        if (eventTarget) {
            closure.eventTarget = eventTarget
            eventTarget.registeredEvents[type] += closure
        } else {
            registeredEvents[type] += closure
        }
    }
    
    def removeEventListener(String type, Closure closure, Entity eventTarget) {
        if (eventTarget) {
            eventTarget.registeredEvents[type] -= closure
        } else {
            registeredEvents[type] -= closure
        }
    }
    
    def dispatch(String type, Entity eventTarget, Map event) {
        processEvent(getRelevantListeners(type, eventTarget), event)
    }
    
    // :(
    private def getRelevantListeners(String type, Entity eventTarget) {
        type = type.toUpperCase()
        eventTarget ? eventTarget.registeredEvents[type] : registeredEvents[type]
    }
    
    private def processEvent(listeners, event) {
        listeners.collect {
            it.event = event
            it.call()
        }.findAll { it != null }
    }
}
