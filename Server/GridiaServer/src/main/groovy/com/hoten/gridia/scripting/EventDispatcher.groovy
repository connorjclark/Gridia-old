package com.hoten.gridia.scripting

class EventDispatcher {
    def registeredEvents = [:].withDefault { [] } 
    
    def addEventListener(String type, closure) {
        registeredEvents[type] += closure
    }
    
    def dispatch(String type, Map event) {
        registeredEvents[type.toUpperCase()].collect {
            it.event = event
            it.call()
        }.findAll { it != null }
    }
}
