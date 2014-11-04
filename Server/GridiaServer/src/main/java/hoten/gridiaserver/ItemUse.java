package hoten.gridiaserver;

import java.util.ArrayList;
import java.util.List;

public class ItemUse {

    public String name, successMessage, failureMessage;
    public int tool, focus, focusQuantityConsumed;
    public List<Integer> products = new ArrayList();
    public List<Integer> quantities = new ArrayList();
}
