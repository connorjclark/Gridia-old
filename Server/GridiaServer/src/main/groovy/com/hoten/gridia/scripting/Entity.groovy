package com.hoten.gridia.scripting

import com.hoten.gridia.serving.ServingGridia
import com.hoten.gridia.map.Coord
import com.hoten.gridia.uniqueidentifiers.UniqueIdentifiers

public class Entity {
    private static final UniqueIdentifiers uniqueIds = new UniqueIdentifiers(100);
    
    public def Coord location
    public def int id = uniqueIds.next()
    public transient def List<GridiaScript> scripts = []
    public transient def Map<String, Closure> registeredEvents = [:].withDefault { [] }
    private def Map storage = [:]
    
    public void retire() {
        uniqueIds.retire(id);
    }
    
    def void removeScripts() {
        scripts.each {
            it.server.scriptExecutor.removeScript(it)
        }
    }
    
    def GridiaScript getScript(String name) {
        scripts.find { it.scriptName == name }
    }
    
    def void setLocation(Coord newLocation) {
        ServingGridia.instance.moveCreatureTo(this, newLocation, false);
    }
    
    // :(
    def boolean hasAttribute(String name) {
        this.hasProperty(name)
    }
    
    // :(
    def setAttribute(String name, value) {
        this."$name" = value
    }
    
    // :(
    def getAttribute(String name) {
        this."$name"
    }
    
    // :(
    def callMethod(String name, List params) {
        this."$name"(*params)
    }
    
    def propertyMissing(String name, value) {
        storage[name] = value
    }
    
    def propertyMissing(String name) {
        storage[name] 
    }
}
