file("scripts/auto").eachFileRecurse {
  server.addScript(it)
}

file("maps/$server.mapName/scripts/auto").eachFileRecurse {
  server.addScript(it)
}
