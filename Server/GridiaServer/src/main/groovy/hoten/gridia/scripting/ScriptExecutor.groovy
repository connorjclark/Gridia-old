package hoten.gridia.scripting

import java.util.concurrent.Executors

// Todo: catch exceptions
class ScriptExecutor {
    def scripts = []
    def scheduler = Executors.newScheduledThreadPool(10);
    
    public ScriptExecutor() {        
        def timeUnits = [
            /milliseconds?/ : 1,
            /seconds?/ : 1000,
            /minutes?/ : 1000 * 60,
            /hours?/ : 1000 * 60 * 60,
            /days?/ : 1000 * 60 * 60 * 24,
            /weeks?/ : 1000 * 60 * 60 * 24 * 7,
            /years?/ : 1000 * 60 * 60 * 24 * 7 * 52.1775
        ]
        Integer.metaClass.propertyMissing = { String name ->
            def match = timeUnits.find { name =~ it.key }
            if (match) {
                delegate * match.value
            } else {
                throw new MissingPropertyException(name, ScriptExecutor)
            }
        }
    }
    
    def addScript(script) {
        scripts += script
        script.metaClass.scheduler = scheduler
        script.start()
        script.run()
    }
    
    def removeScript(script) {
        scripts -= script
        script.end()
        script.scheduledTasks.each { it.cancel() }
    }    
}

