onPlayerLogin { event ->
    def player = event.player
    def loc = player.creature.location
    playAnimation(type: "WarpIn", at: loc)
    announce(message: "$player.username has logged in! Say hi!")
    alert(message: "Welcome to Gridia, $player.username!\nType !help into the chat for controls.\n${server.whoIsOnline()}", to: player.creature)
}
