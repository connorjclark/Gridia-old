state = 'randomWalk'
target = null
targetSearchDistance = 5
chaseDistance = 8
retreatDistance = 10

every((1..3).sample().seconds) {
    if (state == 'randomWalk') {
        randomWalk()
        target = nearestPlayer(targetSearchDistance)
        if (target?.alive) state = 'attack'
    } else if (target?.alive) {
        if (state == 'attack') {
            if (isNearTarget(1)) {
                target.hurt(1, entity.generateDeathReason())
            } else {
                moveRelativeToTarget(true)
            }
            if (target?.alive) if (!isNearTarget(chaseDistance)) state = 'randomWalk'
        } else if (state == 'retreat') {
            moveRelativeToTarget(false)
            if (!isNearTarget(retreatDistance)) state = 'randomWalk'
        }
    } else {
        state = 'randomWalk'
    }
}

def isNearTarget(distance) {
    Math.abs(entity.location.x - target.location.x) <= distance && Math.abs(entity.location.y - target.location.y) <= distance
}

def moveRelativeToTarget(towards) {
    def dx = Math.signum(target.location.x - entity.location.x) as int
    def dy = Math.signum(target.location.y - entity.location.y) as int
    if (!towards) {
        dx = -dx
        dy = -dy
    }
    entity.location = entity.location.add(dx, dy, 0)
}

def randomWalk() {
    def range = -1..1
    def newLocation = entity.location.add(range.sample(), range.sample(), 0)
    if (walkable(newLocation) || creature(newLocation)) {
        entity.location = newLocation
    }
}

def nearestPlayer(distance) {
    def loc = server.findNearestTile(entity.location, distance, false) { coord ->
        !!server.tileMap.getCreature(coord)?.belongsToPlayer
    }
    if (loc != null) {
        server.tileMap.getCreature(loc)
    }
}
