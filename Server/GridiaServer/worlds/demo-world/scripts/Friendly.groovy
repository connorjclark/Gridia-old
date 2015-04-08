onMovedInto { event ->
    announce(message: entity.friendlyMessage, to: event.entity, at: event.location, from: entity.name)
}
