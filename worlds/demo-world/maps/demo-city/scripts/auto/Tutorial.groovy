/*
0 -> speak to npc -> 1
1 -> grab his tools -> 2
2 -> chop a tree -> 3
3 -> use saw on logs -> 4
4 -> use saw on planks -> 5
5 -> use saw on staff -> 6
6 -> use wood handle on pick head -> 7
7 -> save Kitty -> 8
8 -> speak to npc for reward -> 9
 */

tools = ['saw', 'wood axe'].collect { item(name: it) }
npcLoc = loc(61, 216, 0)
tutorialArea = area(loc(50, 203, 0), 30, 35, 2)
toolsLoc = loc(61, 216, 1)
pickHeadLoc = loc(64, 220, 1)
caveInArea = area(loc(59, 222, 1), 8, 8)
greaterCaveInArea = area(loc(56, 219, 1), 14, 14)
kittyArea = area(loc(62, 225, 1), 2, 2)

npcData = cloneMonster(monsterData(id: 10))
npcData.name = "Bill"
npc = spawnMonster(monster: npcData, at: npcLoc, friendly: true)
npc.transient = true
npc.life = npc.maxLife = 100000

kittyData = cloneMonster(monsterData(id: 97))
kittyData.name = "Kitty"
kitty = spawnMonster(monster: kittyData, at: kittyArea.middle, friendly: true)
kitty.friendlyMessage = "meow"
kitty.transient = true
kitty.life = kitty.maxLife = 100000

def start() {
    beforeMovedInto(npc) { event ->
        if (event.entity.belongsToPlayer) {
            npc.friendlyMessage = "Oh no! My poor Kitty is stuck in a cave in! Could you grab my tools from downstairs and help me out?"
            if (event.entity.tutorialStage == 0) {
                setTutorialStage(event.entity, 1)
            } else if (event.entity.tutorialStage == 8) {
                setTutorialStage(event.entity, 9)
                npc.friendlyMessage = "My Kitty! Thank you $event.entity.name!"
            } else if (event.entity.tutorialStage == 9) {
                npc.friendlyMessage = "Thank you for your help!"
            }
        }
    }
    
    beforeMovedInto(kitty) { event ->
        if (event.entity.tutorialStage == 7) {
            setTutorialStage(event.entity, 8)
        }
    }
    
    every(3.seconds) {
        def players = findPlayers(area: tutorialArea)
        if (!players) return
        
        npc.location = npc.location == npcLoc ? npcLoc.add(-1, 0, 0) : npcLoc
        kitty.location = kitty.location == kittyArea.middle ? kittyArea.middle.add(-1, 0, 0) : kittyArea.middle
        
        if (!findPlayers(area: greaterCaveInArea)) {
            fillFloor(area: caveInArea, id: 0)
            fillFloor(area: kittyArea, id: 19)
        }
        
        players.each { player ->
            if (player.tutorialStage == null) {
                setTutorialStage(player, 0)
            } else if (player.tutorialStage == 1) {
                if (tools.every { player.inventory.containsItem(it) }) {
                    setTutorialStage(player, 2)
                }
            }
        }
        
        tools.eachWithIndex { tool, i ->
            spawnItem(item: tool, at: toolsLoc.add(i, 0, 0))
        }
        spawnItem(item: item(name: 'pick head'), at: pickHeadLoc)
    }
}

def setTutorialStage(entity, stage) {
    entity.tutorialStage = stage
    alert(message: alerts[stage], to: entity)
}

alerts = [
    "Speak to $npc.name by moving your character into him. He looks concerned...",
    "$npc.name's saw and axe are in his basement. To pick up items, you can either drag them with the mouse to your inventory/character. Or, you can use the ARROW KEYS to highlight items and press SHIFT. Or, you can stand on an item and press SHIFT.",
    "Go chop a tree. Change your inventory item selection by holding CTRL and pressing the ARROW KEYS. The yellow-highlighted tile is your currently selected tool. Find a tree. Now, use the arrow keys to move the blue world selector over a tree, and press SPACE to use your axe on the tree.",
    "Use your saw on the logs to make some planks.",
    "Now, use your saw on the planks and make a staff",
    "Cut the staff in half with your saw",
    "Pick up the wood handles. There is a pick head downstairs. Use the wood handle on the pick head to make a pick axe.",
    "With the pickaxe in your inventory, you can now move into underground walls to clear them. Go dig out Kitty!",
    "Go speak to $npc.name to claim a prize!",
    "Good job! Enjoy exploring the world :)"
]

beforePlayerLogin { event ->
    def cre = event.player.creature
    if (cre.tutorialStage != null) {
        (0..cre.tutorialStage).each {
            alert(message: alerts[it], to: cre)
        }
    }
}

def verifyItemUse(event, toolName, focusName) {
    def tool = item(name: toolName)
    def focus = item(name: focusName)
    event.tool.itemInstance.item.id == tool.item.id && event.focus.itemInstance.item.id == focus.item.id
}

onCompleteItemUse { event ->
    def cre = event.entity
    switch (cre.tutorialStage) {
    case 2:
        def smallLog = item(name: 'small log')
        if (event.result.products.any { it.item.id == smallLog.item.id }) {
            setTutorialStage(cre, 3)
        }
        break
    case 3:
        if (verifyItemUse(event, 'saw', 'small log')) {
            setTutorialStage(cre, 4)
        }
    case 4:
        if (verifyItemUse(event, 'saw', 'wood planks')) {
            setTutorialStage(cre, 5)
        }
        break
    case 5:
        if (verifyItemUse(event, 'saw', 'staff')) {
            setTutorialStage(cre, 6)
        }
        break
    case 6:
        if (verifyItemUse(event, 'wood handle', 'pick head')) {
            setTutorialStage(cre, 7)
        }
        break
    }
}
