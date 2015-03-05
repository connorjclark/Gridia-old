def start() {
}

every(2.seconds) {
    println "tick..."
}
    
def end() {
}

listenForPlayerLogin {
    player = event.player
    announce(from: "WORLD", message: "$player.accountDetails.username has logged in! Say hi!")
}
