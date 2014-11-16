package hoten.gridiaserver.content;

import java.util.ArrayList;
import java.util.List;
import org.apache.commons.lang.builder.ReflectionToStringBuilder;

public class ItemUse {

    public String name, successMessage, failureMessage;
    public int tool, focus, focusQuantityConsumed, animation, successTool;
    public List<Integer> products = new ArrayList();
    public List<Integer> quantities = new ArrayList();

    @Override
    public String toString() {
        return ReflectionToStringBuilder.toString(this);
    }
}
