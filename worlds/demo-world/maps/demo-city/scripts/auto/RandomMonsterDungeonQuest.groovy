def findRandomMonster() {
    def monster = monsterData(id: (int) (Math.random() * 812))
    monster ?: findRandomMonster()
}

all = area(loc(-14, 114, 1), 43, 43)
rooms = [
    [area: area(loc(5, 137, 1), 13, 13), numToSpawn: 5], // first room
    [area: area(loc(1, 114, 1), 21, 8), numToSpawn: 10], // big room
    [area: area(loc(-14, 126, 1), 13, 16), numToSpawn: 14] // hall
]
rooms.each { it.spawned = [] }

every(5.seconds) {
    if (findPlayers(area: all)) {
        rooms.findAll { findPlayers(area: it.area) }.each {
            it.spawned = it.spawned.findAll { it.alive }
            if (!it.spawned) {
                def monster = findRandomMonster()
                it.spawned = spawnMonsters(monster: cloneMonster(monster), area: it.area, times: it.numToSpawn)
                announce(from: "RANDOM DUNGEON", at: it.area.middle, message: "A gang of $monster.name appear!")
            }
        }
    } else {
        rooms.each { it.spawned.each { remove it } }
    }
}
