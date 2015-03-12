every(1.second) {
    range = -1..1
    newLocation = entity.location.add(range.sample(), range.sample(), 0)
    if (walkable(newLocation)) {
        entity.location = newLocation
    }
}
