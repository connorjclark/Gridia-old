def containsIgnoreCase(string, substring) {
    string.toLowerCase().contains(substring.toLowerCase())
}

def isDoor(item) {
    item.itemClass == ItemClass.Wall && containsIgnoreCase(item.name, "Door")
}

def isOpen(item) {
    containsIgnoreCase(item.name, "Open")
}

def isClosed(item) {
    containsIgnoreCase(item.name, "Closed")
}

def close(openDoor) {
    def closedDoorName = openDoor.name.replaceFirst("(?i)Open", "Closed")
    item(name: closedDoorName)
}

def open(closedDoor) {
    def openDoorName = closedDoor.name.replaceFirst("(?i)Closed", "Open")
    item(name: openDoorName)
}

onFailedItemUse { event ->
    def tool = event.tool
    def focus = event.focus
    if (tool.itemInstance.item.id == 0 && isDoor(focus.itemInstance.item)) {
        if (isOpen(focus.itemInstance.item)) {
            focus.changeWrappedItem(close(focus.itemInstance.item))
        } else if (isClosed(focus.itemInstance.item)) {
            focus.changeWrappedItem(open(focus.itemInstance.item))
        } 
    }
}
