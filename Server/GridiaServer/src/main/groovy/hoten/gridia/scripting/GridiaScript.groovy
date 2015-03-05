package hoten.gridia.scripting

import hoten.gridia.serving.ServingGridia

class GridiaScript {
    def ServingGridia server
    
    def GridiaScript(ServingGridia server) {
        this.server = server
    }
    
    def doSomethingCool() {
        println "Cool! world name = $server.worldName"
    }
    
    def announce(message) {
        server.announce(message)
    }
}

