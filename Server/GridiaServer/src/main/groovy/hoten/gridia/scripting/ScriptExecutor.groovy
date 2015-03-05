package hoten.gridia.scripting

// Todo: catch exceptions
class ScriptExecutor {
    def scripts = []
    def gridiaScript = new GridiaScript()
    
    public ScriptExecutor() {
        println "create exec"
    }
    
    def update() {
        scripts.each { it.update() }
    }
    
    def addScript(script) {
        scripts += script
        script.start()
    }
    
    def removeScript(script) {
        scripts -= script
        script.end()
    }    
}

