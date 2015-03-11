def isContainerWrapper(wrapper) {
    wrapper.class.name == "ContainerItemWrapper"
}

def getCave(event) {
    if (event.result?.products?.first()?.item?.isCave() && !isContainerWrapper(event.focus)) {
        event.result.products[0].item
    }
}

onValidateItemUse {
    cave = getCave(event)
    if (!cave) return
    
    if (cave.itemClass == ItemClass.Cave_down) {
        if (event.focus.isLowestLevel()) return "You can't dig any lower"
    } else if (cave.itemClass == ItemClass.Cave_up) {
        if (event.focus.isHighestLevel()) return "You can't go any higher"
    }
}

onCompleteItemUse {
    cave = getCave(event)
    if (!cave) return
    
    focus = event.focus
    if (cave.itemClass == ItemClass.Cave_down) {
        below = focus.itemBelow
        if (below.item.itemClass != ItemClass.Cave_up) {
            if (below.isNothing() || focus.moveItemBelow()) {
                focus.itemBelow = server.contentManager.createItemInstance(981)
            }
        }
    } else {
        above = focus.itemAbove
        if (above.item.itemClass != ItemClass.Cave_down) {
            if (above.isNothing() || focus.moveItemAbove()) {
                focus.itemAbove = server.contentManager.createItemInstance(980)
            }
        }
    }
}
