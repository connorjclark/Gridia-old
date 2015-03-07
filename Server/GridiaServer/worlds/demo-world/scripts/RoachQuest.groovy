duration = 30.seconds
numRoaches = 50
tick = 3.seconds
arena = area(loc(70, 166, 1), 24, 24)
winnerTeleportLoc = loc(88, 190)
loserTeleportLoc = loc(86, 192)
roaches = []
arenaIsGoing = false

every(tick) {
    roaches = roaches.findAll { it.alive }
    
    // :(
    if (!server.anyPlayersOnline()) {
        return
    }
    
    playersInArena = findPlayers(area: arena)
    if (arenaIsGoing) {
        if (playersInArena) {
            stepArena(playersInArena)
        } else {
            clearArena()
        }
    } else if (playersInArena) {
        startArena()
    }
}

def stepArena(playersInArena) {
    if (timeLeft) {
        spawnRoaches()
        announceInArena("Seconds left: ${timeLeft/1000}")
        timeLeft -= tick      
        return
    }
    
    if (playersInArena) {
        winnerInfo = playersInArena.collect {
            [player: it, amount: removeItemFrom(container: it.inventory, itemId: 447).quantity]
        }.max { it.amount }
        winner = winnerInfo.player
        playersInArena.remove winner
        playersInArena.each { teleport(target: it, to: loserTeleportLoc) }
        teleport(target: winner, to: winnerTeleportLoc)
        announceInArena("Game over!\nWinner: $winner.name\nMost Antenae: $winnerInfo.amount")
    } else {
        announceInArena("Game over!\nWinner: None\nMost Antenae: 0")
    }
    
    clearArena()
}

def spawnRoaches() {
    int currentAmount = roaches.size()
    roach = server.contentManager.getMonster(42)
    roachData = cloneMonsterAndStripName(roach)
    newRoaches = spawn(monster: roachData, area: arena, amount: numRoaches - currentAmount)
    roaches.addAll(newRoaches)
}

def announceInArena(message) {
    announce(from: "ROACH ARENA", at: arena.middle, message: message)
}

def clearArena() {
    arenaIsGoing = false
    roaches.each { remove it }
    roaches = []
}

def startArena() {
    arenaIsGoing = true
    timeLeft = duration
    spawnRoaches()
    announceInArena("BEGIN!")
    announceInArena("*OBJECTIVE - Collect the most antenae!*")
}
