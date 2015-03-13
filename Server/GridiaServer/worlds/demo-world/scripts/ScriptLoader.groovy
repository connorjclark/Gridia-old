file("scripts/auto").eachFileRecurse {
  server.addScript(it, null)
}

file("maps/$server.mapName/scripts/auto").eachFileRecurse {
  server.addScript(it, null)
}
