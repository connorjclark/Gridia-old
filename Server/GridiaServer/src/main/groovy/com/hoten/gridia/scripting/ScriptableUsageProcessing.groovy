package com.hoten.gridia.scripting

import com.hoten.gridia.ItemWrapper
import com.hoten.gridia.content.ItemUse
import com.hoten.gridia.content.ItemUseException
import com.hoten.gridia.content.UsageProcessor
import com.hoten.gridia.content.UsageProcessor.UsageResult
import com.hoten.gridia.serving.ServingGridia

class ScriptableUsageProcessing extends UsageProcessor {
    def EventDispatcher eventDispatcher
    
    def ScriptableUsageProcessing(contentManager, eventDispatcher) {
        super(contentManager)
        this.eventDispatcher = eventDispatcher
    }
    
    def void validate(UsageResult result, ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
        super.validate(result, usage, tool, focus)
        def issues = eventDispatcher.dispatch("ValidateItemUse", ServingGridia.instance.worldEntity, [
                result:result,
                usage:usage,
                tool:tool,
                focus:focus
            ])
        if (issues) {
            throw new ItemUseException(issues.join('\n'))
        }
    }
    
    def void implementResult(UsageResult result, ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
        super.implementResult(result, usage, tool, focus)
        eventDispatcher.dispatch("CompleteItemUse", ServingGridia.instance.worldEntity, [
                result:result,
                usage:usage,
                tool:tool,
                focus:focus
            ])
    }
}
