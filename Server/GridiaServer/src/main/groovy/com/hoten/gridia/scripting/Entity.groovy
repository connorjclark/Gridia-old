package com.hoten.gridia.scripting

import com.hoten.gridia.map.Coord

public class Entity {
    public def Coord location
    public transient def List<GridiaScript> scripts = []
    public transient def Map<String, Closure> registeredEvents = [:].withDefault { [] } 
    
    def void removeScripts() {
        scripts.each {
            it.server.scriptExecutor.removeScript(it)
        }
    }
    
    def GridiaScript getScript(String name) {
        scripts.find { it.scriptName == name }
    }
    
    def setAttribute(String name, value) {
        this."$name" = value
    }
    
    def getAttribute(String name) {
        this."$name"
    }
    
    // :(
    def callMethod(String name, List params) {
        this."$name"(*params)
    }
}
