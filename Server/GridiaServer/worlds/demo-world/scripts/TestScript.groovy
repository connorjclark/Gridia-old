def start() {
}
    
def update() {
}
    
def end() {
}

listenForPlayerLogin {
    player = event.player
    announce(from: "WORLD", message: "$player.accountDetails.username has logged in! Say hi!")
}
