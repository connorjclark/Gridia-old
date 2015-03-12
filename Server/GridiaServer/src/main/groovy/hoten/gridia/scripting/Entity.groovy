package hoten.gridia.scripting

import hoten.gridia.map.Coord

public class Entity {
    public def Coord location
    public transient def List<GridiaScript> scripts = []
    
    def void removeScripts() {
        scripts.each {
            it.server.scriptExecutor.removeScript(it)
        }
    }
}
