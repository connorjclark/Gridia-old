def start() {
    creatures = server.creatures
}

every(1.second) {
    creatures.findAll { !it.value.justTeleported }.each {
        creature = it.value
        itemUnder = server.tileMap.getItem(creature.location)
        loc = creature.location;
        if (loc.z != server.tileMap.depth - 1 && itemUnder.getItem().itemClass == ItemClass.Cave_down) {
            server.moveCreatureTo(creature, loc.add(0, 0, 1), true);
            creature.justTeleported = true;
        } else if (loc.z != 0 && itemUnder.getItem().itemClass == ItemClass.Cave_up) {
            server.moveCreatureTo(creature, loc.add(0, 0, -1), true);
            creature.justTeleported = true;
        }
    }
}
