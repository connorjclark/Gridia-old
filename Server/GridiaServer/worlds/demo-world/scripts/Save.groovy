every(15.minutes) {
    if (server.anyPlayersOnline()) {
        server.save()
    }
}
