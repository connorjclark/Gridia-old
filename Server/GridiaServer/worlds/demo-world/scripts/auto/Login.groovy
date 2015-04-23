onPlayerLogin { event ->
    def player = event.player
    def loc = player.creature.location
    playAnimation(type: "WarpIn", at: loc)
    announce(message: "$player.username has logged in! Say hi!")
    alert(message: "Welcome to Gridia, $player.username!\nType !help into the chat for controls.\n${server.whoIsOnline()}", to: player.creature)
    alert(message: "PLEASE READ! This build is meant to prototype the combat system. Here are all the controls you need to know:" +
            "\n" +
            "\nClick on a creature to target it" +
            "\nOr, press 'T', and press the key corresponding to the target you want." +
            "\nTo un-target, tap 'T' twice" +
            "\nCurrently, there are two types of actions:" +
            "\n" +
            "\nA) Target-actions, which perform the action on the target" +
            "\nB) Destination-actions, which performs the action on a specific tile" +
            "\n" +
            "\nIf you attempt to use a target-action without a target selected, the closest creature will be auto-targeted" +
            "\n" +
            "\nHere are the current actions. You can activate them by clicking on them at the bottom of the screen, or by hitting the corresponding number key" +
            "\n" +
            "\n1) [Target] Hit - basic attack" +
            "\n2) [Destination] Dash - move quickly to a tile" +
            "\n3) [Target] Flame - cast a fire spell" +
            "\n4) [Target] Heal - cast a healing spell" +
            "\n" +
            "\nNow go kill each other.", to: player.creature)
}
