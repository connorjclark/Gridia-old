package hoten.gridia.scripting

import hoten.gridia.serving.ServingGridia
import hoten.gridia.content.Item
import hoten.gridia.map.Coord
import hoten.gridia.content.Monster
import java.util.concurrent.TimeUnit
import java.util.logging.Level
import java.util.logging.Logger

class GridiaScript {
    def ServingGridia server
    def EventDispatcher eventDispatcher
    def scheduledTasks = []
    def Entity entity
    
    def GridiaScript(ServingGridia server, EventDispatcher eventDispatcher) {
        this.server = server
        this.eventDispatcher = eventDispatcher
    }
    
    def loc(x, y, z = 0) {
        new Coord(x, y, z)
    }
    
    def area(l, w, h) {
        def mx = (int)(l.x + w/2)
        def my = (int)(l.y + h/2)
        [loc: l, width: w, height: h, middle: loc(mx, my, l.z)]
    }
    
    def findCreatures(Map params) {
        params.with {
            if (area) {
                width = area.width
                height = area.height
                at = area.loc
            }
        }
        
        def creatures = []
        for (x in 0..<params.width) {
            for (y in 0..<params.height) {
                def loc = params.at.add(x, y, 0)
                def cre = server.tileMap.getCreature(loc)
                if (cre != null) creatures += cre
            }
        }
        creatures
    }
    
    def findPlayers(Map params) {
        findCreatures(params).findAll { it.belongsToPlayer }
    }
    
    def removeItemFrom(Map params) {
        def itemRemoved = server.contentManager.createItemInstance(params.itemId, 0)
        params.container.items.eachWithIndex { item, i ->
            if (item.item.id == params.itemId) {
                itemRemoved = itemRemoved.add item.quantity
                params.container.deleteSlot i
            }
        }
        itemRemoved
    }
    
    def cloneMonsterAndStripName(monster) {
        def cloned = monster.clone()
        cloned.name = ""
        cloned
    }
    
    def walkable(loc) {
        server.tileMap.walkable(loc)
    }
    
    def floor(loc) {
        server.tileMap.getFloor(loc)
    }
    
    def spawn(Map params) {
        if (params.area) {
            params.at = params.area.loc
            params.width = params.area.width
            params.height = params.area.height
        }
        
        def spawned = []
        def rand = new Random()
        params.amount.times {
            def loc = params.at.add(rand.nextInt(params.width), rand.nextInt(params.height), 0)
            if (walkable(loc) && floor(loc)) {
                spawned += server.createCreature(params.monster, loc)
            }
        }
        spawned
    }
    
    def remove(creature) {
        server.removeCreature(creature)
    }
    
    def teleport(Map params) {
        server.teleport(params.target, params.to)
    }

    def announce(Map params) {
        params.from = params.from ?: "WORLD"
        params.at = params.at ?: loc(0, 0)
        
        server.announce(params.from, params.message, params.at)
    }
    
    def playAnimation(Map params) {
        server.playAnimation(params.name, params.location)
    }
    
    def every(duration, closure) {
        def catching = {
            try {
                closure.call()
            } catch (ex) {
                Logger.getLogger(GridiaScript.class.name).log(Level.SEVERE, null, ex);
                announce(from: "SCRIPT EXECUTOR", message: "Script error: $ex")
                future.cancel()
                scheduledTasks -= future
            }
        }
        catching.delegate = this
        def future = scheduler.scheduleAtFixedRate(catching, 0, duration, TimeUnit.MILLISECONDS)
        scheduledTasks += future
    }
    
    def propertyMissing(String name) {
        if (name == "ItemClass") {
            Item.ItemClass
        } else {
            throw new MissingPropertyException(name, GridiaScript)
        }
    }
    
    def methodMissing(String name, args) {
        if (name.startsWith("on") && args.length == 1 && args[0] instanceof Closure) {
            def type = name.replaceFirst("on", "").toUpperCase()
            eventDispatcher.addEventListener(type, args[0])
        } else if (['start', 'update', 'end'].every { it != name } ) {
            throw new MissingMethodException(name, GridiaScript, args)
        }
    }
}
