entity.maxLife = 6
entity.life = entity.maxLife

entity.metaClass.isAlive = { delegate.life > 0 }

entity.metaClass.hurt = { damage, deathReason, animation = "Attack" ->
    if (delegate.friendly) return
    delegate.life -= damage
    delegate.life = Math.min(delegate.life, delegate.maxLife)
    playAnimation(type: animation)

    def message = server.messageBuilder.setLife(delegate)
    server.sendToClientsWithAreaLoaded(message, delegate.location)

    if (!delegate?.alive) {
        eventDispatcher.dispatch("Death", delegate, [deathReason:deathReason])
    }
}

entity.metaClass.generateDeathReason = {
    def attackVerbs = ["clobber", "bash", "destroy", "exterminate", "off"]
    def verb = attackVerbs[(attackVerbs.size()*Math.random()) as int]
    "was ${verb}ed by $delegate.name"
}

def getHurtBy(attacker) {
    if (attacker.isFriendly || entity.isFriendly) return
    entity.hurt(1, attacker.generateDeathReason())
}

onDeath { event ->
    announce(message: "$entity.name $event.deathReason.")
    playAnimation(type: "diescream")
    spawnItem(item: item(name: "Small Pool Of Blood"), near: entity.location)
    if (entity.belongsToPlayer) {
        teleport(to: server.tileMap.defaultPlayerSpawn) // :(
        entity.life = entity.maxLife
    } else {
        if (entity.inventory) {
            server.dropContainerNear(entity.inventory, entity.location) // :(
        }
        remove(entity)
    }
}

onAction { event ->
    if (event.actionId == 0) {
        if (!entity?.target?.alive) return
        hitAction(entity.target)
    } else if (event.actionId == 1) {
        dashAction(event.location)
    } else if (event.actionId == 2) {
        if (!entity?.target?.alive) return
        fireSpellAction(entity.target)
    } else if (event.actionId == 3) {
        if (!entity?.target?.alive) return
        healingSpellAction(entity.target)
    }
}

def isNear(target) {
    Math.abs(entity.location.x - target.location.x) <= 1 && Math.abs(entity.location.y - target.location.y) <= 1
}

def hitAction(target) {
    if (isNear(target)) {
        target.hurt(1, entity.generateDeathReason())
    }
}

def dashAction(destination) {
    def speed = 12 // tiles per second
    def delta = (entity.location - destination).dist()
    def time = 1000*delta/speed
    playAnimation(type: "Roll", at: entity.location)
    server.moveCreatureTo(entity, destination, time as int, false, false, true)
}

def fireSpellAction(target) {
    target.hurt(3, entity.generateDeathReason(), "Flame")
}

def healingSpellAction(target) {
    target.hurt(-10, entity.generateDeathReason(), "Heal")
}
