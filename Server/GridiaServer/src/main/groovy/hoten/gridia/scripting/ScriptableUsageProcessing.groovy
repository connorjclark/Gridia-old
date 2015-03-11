package hoten.gridia.scripting

import hoten.gridia.ItemWrapper
import hoten.gridia.content.ItemUse
import hoten.gridia.content.UsageProcessor
import hoten.gridia.content.UsageProcessor.UsageResult

class ScriptableUsageProcessing extends UsageProcessor {
    def EventDispatcher eventDispatcher
    
    def ScriptableUsageProcessing(contentManager, eventDispatcher) {
        super(contentManager)
        this.eventDispatcher = eventDispatcher
    }
    
    def boolean validate(UsageResult result, ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
        super.validate(result, usage, tool, focus) && eventDispatcher.dispatch("ValidateItemUse", [
                result:result,
                usage:usage,
                tool:tool,
                focus:focus
            ]).every { it }
    }
    
    def void implementResult(UsageResult result, ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
        super.implementResult(result, usage, tool, focus)
        eventDispatcher.dispatch("CompleteItemUse", [
                result:result,
                usage:usage,
                tool:tool,
                focus:focus
            ])
    }
}
