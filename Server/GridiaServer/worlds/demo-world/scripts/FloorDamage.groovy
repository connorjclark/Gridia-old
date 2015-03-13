def start() {
    badFloors = [16, 48]
    creatures = server.creatures
}

every(1.second) {
    creatures.findAll {
        floor = server.tileMap.getFloor(it.value.location)
        badFloors.contains(floor)
    }.each {
        it.value.hurt(1, "incinerated in lava")
    }
}
