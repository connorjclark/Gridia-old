def start() {
    badFloors = [16, 48]
    creatures = server.creatures
}

every(1.second) {
    creatures.findAll {
        badFloors.contains(floor(it.value.location))
    }.each {
        it.value.hurt(1, "incinerated in lava")
    }
}
