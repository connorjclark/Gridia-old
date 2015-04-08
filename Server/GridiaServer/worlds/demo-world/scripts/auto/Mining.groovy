def hasPickAxe(container) {
    container.containsItemId(901)
}

onMovedInto { event ->
    if (floor(event.location) != 0) { return }
    
    if (hasPickAxe(event.entity.inventory)) {
        setFloor(id: 19, at: event.location)
        int oreId = Math.random() > 0.7 ? 0 : server.contentManager.getRandomItemOfClassByRarity(ItemClass.Ore).id
        spawnItem(item: item(id: oreId), near: event.location)
        playAnimation(type: "MiningSound", at: event.location)
    } else {
        announce(message: "You need a pickaxe to mine!", to: event.entity)
    }
}
