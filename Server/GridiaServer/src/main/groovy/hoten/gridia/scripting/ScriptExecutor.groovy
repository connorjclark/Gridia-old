package hoten.gridia.scripting

// Todo: catch exceptions
class ScriptExecutor {
    def scripts = []
    
    def update() {
        scripts.each { it.update() }
    }
    
    def addScript(script) {
        scripts += script
        script.run()
        script.start()
    }
    
    def removeScript(script) {
        scripts -= script
        script.end()
    }    
}

