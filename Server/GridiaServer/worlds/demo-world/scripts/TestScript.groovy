listenForPlayerLogin {
    player = event.player
    playAnimation(name: "WarpIn", location: player.creature.location)
    announce(from: "WORLD", message: "$player.accountDetails.username has logged in! Say hi!")
    announce(from: "WORLD", message: server.whoIsOnline())
}
