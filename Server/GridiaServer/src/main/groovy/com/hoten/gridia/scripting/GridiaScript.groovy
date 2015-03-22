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
    def Map storage = [:]
    
    def GridiaScript(ServingGridia server, EventDispatcher eventDispatcher, Entity entity, String scriptName) {
        this.server = server
        this.eventDispatcher = eventDispatcher
        this.scriptName = scriptName
        this.entity = entity
        entity.scripts += this
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
        if (params.id && params.name) throw new GridiaDSLException("Expected exactly one of the following: id, name")
        if (params.name) {
            def method = "getItemByName${params.caseSensitive ? '' : 'IgnoreCase'}"
            params.id = server.contentManager."$method"(params.name).id
        }
        
        server.contentManager.createItemInstance(params.id, params.quantity)
    }
    
    def monsterData(Map params) {
        server.contentManager.getMonster(params.id)
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
    
    def cloneMonster(monster) {
        monster.clone()
    }
    
    def walkable(Coord loc) {
        server.tileMap.walkable(loc)
    }
    
    def floor(Coord loc) {
        server.tileMap.getFloor(loc)
    }
    
    def item(Coord loc) {
        server.tileMap.getItem(loc)
    }
    
    def creature(Coord loc) {
        server.tileMap.getCreature(loc)
    }
    
    private def spawn(Map params, Closure generator) {
        prepSpawnParams(params)
        (0..<params.times).collect {
            generator()
        }.findAll { it }
    }
    
    private def prepSpawnParams(Map params) {
        params.with {
            if (area) {
                at = area.loc
                width = area.width
                height = area.height
            }
            times = times ?: 1
            width = width ?: 1
            height = height ?: 1
            range = range ?: 3
            if ([at, near].findAll { it }.size() != 1) {
                throw new GridiaDSLException("Expected exactly one of the following: at, near")
            }
        }
    }
    
    private def rand = new Random()
    private def determineSpawnLocation(Map params) {
        params.near ? params.near : params.at.add(rand.nextInt(params.width), rand.nextInt(params.height), 0)
    }
    
    def spawnMonsters(Map params) {
        if (params.near) {
            throw new GridiaDSLException("near is currently not supported.")
        }
        params.friendly = params.friendly ?: false
        params.forceSpawn = params.forceSpawn ?: false
        
        def generator = {
            def loc = determineSpawnLocation(params)
            if ((walkable(loc) && floor(loc)) || params.forceSpawn) {
                server.createCreature(params.monster, loc, params.friendly)
            }
        }
        spawn(params, generator)
    }
    
    def spawnItems(Map params) {
        def methodCall = params.near ?
        { loc -> server.addItemNear(params.item, loc, params.range, true) }
    :
        { loc -> server.addItem(params.item, loc) }
        def generator = {
            def loc = determineSpawnLocation(params)
            if (item(loc).nothing && floor(loc)) {
                methodCall(loc)
            }
        }
        spawn(params, generator)
    }
    
    def spawnItem(Map params) {
        params.times = 1
        spawnItems(params)[0]
    }
    
    def spawnMonster(Map params) {
        params.times = 1
        params.forceSpawn = params.forceSpawn ?: true
        spawnMonsters(params)[0]
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
        def future = scheduler.scheduleAtFixedRate(catching, duration, duration, TimeUnit.MILLISECONDS)
        scheduledTasks += future
    }
    
    def propertyMissing(String name) {
        if (name == "ItemClass") {
            Item.ItemClass
        } else if (storage[name]) {
            storage[name]
        } else {
            throw new MissingPropertyException(name, GridiaScript)
        }
    }
        
    def void propertyMissing(String name, value) {
        storage[name] = value
        this."$name" // :( removing this line seems to destroy the universe
    }
    
    def methodMissing(String name, args) {
        if (['before', 'on', 'after'].any{ name.startsWith(it) } && (1..2).contains(args.length) && args.last() instanceof Closure) {
            eventDispatcher.addEventListener(name.toUpperCase(), args.last(), args.length == 1 ? entity : args.first())
        } else if (['start', 'update', 'end'].every { it != name } ) {
            throw new MissingMethodException(name, GridiaScript, args)
        }
    }
}
