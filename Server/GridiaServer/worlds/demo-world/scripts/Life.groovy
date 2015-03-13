entity.life = 3

entity.metaClass.isAlive = { delegate.life > 0 }

entity.metaClass.hurt = { damage, deathReason ->
    delegate.life -= damage
    playAnimation(type: "Attack")
    if (!delegate?.alive) {
        eventDispatcher.dispatch("Death", delegate, [deathReason:deathReason])
    }
}

if (!entity.isFriendly && !entity.belongsToPlayer) {
    onMovedInto {
        verbs = ["clobber", "bash", "destroy", "exterminate", "off"]
        entity.hurt(1, "was ${verbs[(verbs.size()*Math.random()) as int]}ed by $event.entity.name")
    }
}

onDeath {
    announce(message: "$entity.name $event.deathReason.")
    playAnimation(type: "diescream")
    spawnItems(item: item(id: 1022), near: entity.location, range: 3)
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
