def start() {
    badFloors = [16, 48]
    creatures = server.creatures
}

every(1.second) {
    creatures.findAll {
        floor = server.tileMap.getFloor(it.value.location)
        badFloors.contains(floor)
    }.each {
        server.hurtCreature(it.value, 1);
    }
}
