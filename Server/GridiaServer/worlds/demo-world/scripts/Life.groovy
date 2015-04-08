entity.life = 3

entity.metaClass.isAlive = { delegate.life > 0 }

entity.metaClass.hurt = { damage, deathReason ->
    if (delegate.friendly) return
    delegate.life -= damage
    playAnimation(type: "Attack")
    if (!delegate?.alive) {
        eventDispatcher.dispatch("Death", delegate, [deathReason:deathReason])
    }
}

def generateDeathReason(attacker) {
    def attackVerbs = ["clobber", "bash", "destroy", "exterminate", "off"]
    def verb = attackVerbs[(attackVerbs.size()*Math.random()) as int]
    "was ${verb}ed by $attacker.name"
}

def getHurtBy(attacker) {
    if (attacker.isFriendly || entity.isFriendly) return
    entity.hurt(1, generateDeathReason(attacker))
}

onDeath {
    announce(message: "$entity.name $event.deathReason.")
    playAnimation(type: "diescream")
    spawnItem(item: item(name: "Small Pool Of Blood"), near: entity.location)
    if (entity.belongsToPlayer) {
        teleport(to: server.tileMap.defaultPlayerSpawn) // :(
        entity.life = 3
    } else {
        if (entity.inventory) {
            server.dropContainerNear(entity.inventory, entity.location) // :(
        }
        remove(entity)
    }
}

onAction { event ->
    if (event.actionId == 1) {
        if (!entity?.target?.alive) return
        hitAction(entity.target)
    } else if (event.actionId == 2) {
        dashAction(event.location)
    }
}

def isNear(target) {
    Math.abs(entity.location.x - target.location.x) <= 1 && Math.abs(entity.location.y - target.location.y) <= 1
}

def getNear(target) {
    def dx = Math.signum(target.location.x - entity.location.x) as int
    def dy = Math.signum(target.location.y - entity.location.y) as int
    entity.location = entity.location.add(dx, dy, 0)
}

def hitAction(target) {
    if (isNear(target)) {
        target.hurt(1, generateDeathReason(entity))
    } else {
        getNear(target)
    }
}

def dashAction(destination) {
    speed = 12 // tiles per second
    delta = (entity.location - destination).dist()
    time = 1000*delta/speed
    server.moveCreatureTo(entity, destination, time as int, false, false, true)
    playAnimation(type: "Roll", at: entity.location)
}
