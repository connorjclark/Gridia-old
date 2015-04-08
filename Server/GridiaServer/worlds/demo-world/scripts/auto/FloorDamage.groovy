badFloors = [16, 48]

every(1.second) {
    server.creatures.findAll {
        badFloors.contains(floor(it.value.location))
    }.each {
        it.value.hurt(1, "incinerated in lava")
    }
}
