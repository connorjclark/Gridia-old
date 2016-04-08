every(15.minutes) {
    if (server.anyPlayersOnline()) {
        //server.save()
        announce(message: "save has been disabled ...")
    }
}
