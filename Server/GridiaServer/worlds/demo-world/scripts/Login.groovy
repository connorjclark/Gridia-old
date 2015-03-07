onPlayerLogin {
    player = event.player
    loc = player.creature.location
    playAnimation(name: "WarpIn", location: loc)
    announce(message: "$player.accountDetails.username has logged in! Say hi!")
    announce(at: loc, message: server.whoIsOnline())
}
