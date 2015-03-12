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
        [Number, Integer].each { klass ->
            klass.metaClass.propertyMissing = { String name ->
                (long) timeUnits.find { name =~ it.key }?.value?.multiply(delegate).with {
                    it ?: { throw new MissingPropertyException(name, klass) }()
                }
            }
        }
        
        def random = new Random()
        Range.metaClass.methodMissing = { String name, args ->
            if (name == "sample" && !args) {
                def min = delegate.from
                def max = delegate.to
                min + random.nextInt(max - min + 1)
            } else {
                throw new MissingPropertyException(name, Range)
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
        script.scheduledTasks.each { it.cancel(true) }
        script.entity = null
    }    
}
