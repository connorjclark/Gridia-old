onPlayerLogin {
    player = event.player
    loc = player.creature.location
    playAnimation(type: "WarpIn", at: loc)
    announce(message: "$player.username has logged in! Say hi!")
    announce(message: server.whoIsOnline(), at: loc)
}
