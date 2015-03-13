package com.hoten.gridia.scripting

import com.hoten.gridia.serving.ServingGridia
import com.hoten.gridia.content.Item
import com.hoten.gridia.map.Coord
import com.hoten.gridia.content.Monster
import java.util.concurrent.TimeUnit
import java.util.logging.Level
import java.util.logging.Logger

class GridiaDSLException extends Exception {}

public class GridiaScript {
    def ServingGridia server
    def EventDispatcher eventDispatcher
    def scheduledTasks = []
    def Entity entity
    def String scriptName
    
    def GridiaScript(ServingGridia server, EventDispatcher eventDispatcher, String scriptName) {
        this.server = server
        this.eventDispatcher = eventDispatcher
        this.scriptName = scriptName
    }
    
    def file(name) {
        new File(server.worldTopDirectory, name)
    }
    
    def loc(x, y, z = 0) {
        new Coord(x, y, z)
    }
    
    def area(l, w, h) {
        def mx = (int)(l.x + w/2)
        def my = (int)(l.y + h/2)
        [loc: l, width: w, height: h, middle: loc(mx, my, l.z)]
    }
    
    def item(Map params) {
        params.quantity = params.quantity ?: 1
        server.contentManager.createItemInstance(params.id, params.quantity)
    }
    
    def setFloor(Map params) {
        server.changeFloor(params.at, params.id)
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
        params.with {
            if (area) {
                at = area.loc
                width = area.width
                height = area.height
            }
            amount = amount ?: 1
            width = width ?: 1
            height = height ?: 1
            range = range ?: 3
            if ([at, near].findAll { it }.size() != 1) {
                throw new GridiaDSLException("Expected exactly one of the following: at, near")
            }
            if ([item, monster].findAll { it }.size() != 1) {
                throw new GridiaDSLException("Expected exactly one of the following: item, monster")
            }
        }
            
        def rand = new Random()
        def generator
            
        if (params.at) {
            if (params.item) {
                generator = {
                    def loc = params.at.add(rand.nextInt(params.width), rand.nextInt(params.height), 0)
                    if (walkable(loc) && floor(loc)) {
                        server.addItem(loc, params.item)
                    }
                }
            } else if (params.monster) {
                generator = {
                    def loc = params.at.add(rand.nextInt(params.width), rand.nextInt(params.height), 0)
                    if (walkable(loc) && floor(loc)) {
                        server.createCreature(params.monster, loc)
                    }
                }
            }
        } else if (params.near) {
            if (params.item) {
                generator = {
                    server.addItemNear(params.near, params.item, params.range, true)
                }
            } else if (params.monster) {
                throw new GridiaDSLException("near & monster is currently not supported.")
            }
        }
            
        def spawned = []
        params.amount.times {
            spawned += generator()
        }
        spawned.findAll { it != null }
    }
    
    def remove(creature) {
        server.removeCreature(creature)
    }
    
    def teleport(Map params) {
        params.target = params.target ?: entity
        
        server.teleport(params.target, params.to)
    }

    def announce(Map params) {
        params.from = params.from ?: "WORLD"
        params.at = params.at ?: (params.to ? params.to.location : loc(0, 0))
        
        if (params.to) {
            server.announce(params.from, params.message, params.at, params.to)
        } else {
            server.announce(params.from, params.message, params.at)
        }
    }
    
    def playAnimation(Map params) {
        params.at = params.at ?: entity.location
        
        server.playAnimation(params.type, params.at)
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
            eventDispatcher.addEventListener(type, args[0], entity)
        } else if (['start', 'update', 'end'].every { it != name } ) {
            throw new MissingMethodException(name, GridiaScript, args)
        }
    }
}
