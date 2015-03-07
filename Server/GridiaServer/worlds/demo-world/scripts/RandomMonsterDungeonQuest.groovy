def defineArea(l, w, h) {
    mx = (int)(l.x + w/2)
    my = (int)(l.y + h/2)
    [loc: l, width: w, height: h, middle: loc(mx, my, l.z)]
}

def getRandomMonster() {
    monster = server.contentManager.getMonster((int) (Math.random() * 812))
    monster ?: randomMonster
}

all = defineArea(loc(-14, 114, 1), 43, 43)
rooms = [
    [area: defineArea(loc(5, 137, 1), 13, 13), numToSpawn: 5], // first room
    [area: defineArea(loc(1, 114, 1), 21, 8), numToSpawn: 10], // big room
    [area: defineArea(loc(-14, 126, 1), 13, 16), numToSpawn: 14] // hall
]
rooms.each { it.spawned = [] }

every(5.seconds) {
    if (findPlayers(area: all)) {
        rooms.findAll { findPlayers(area: it.area) }.each {
            it.spawned = it.spawned.findAll { it.alive }
            if (!it.spawned) {
                monster = randomMonster
                it.spawned = spawn(monster: cloneMonsterAndStripName(monster), area: it.area, amount: it.numToSpawn)
                announce(from: "RANDOM DUNGEON", at: it.area.middle, message: "A gang of $monster.name appear!")
            }
        }
    } else {
        rooms.each { it.spawned.each { remove it } }
    }
}
