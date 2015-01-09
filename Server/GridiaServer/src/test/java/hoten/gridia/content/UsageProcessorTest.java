package hoten.gridia.content;

import hoten.gridia.content.UsageProcessor.ContainerItemWrapper;
import hoten.gridia.content.UsageProcessor.ItemWrapper;
import hoten.gridia.content.UsageProcessor.UsageResult;
import hoten.gridia.content.UsageProcessor.WorldItemWrapper;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ServingGridia;
import java.io.IOException;
import org.easymock.EasyMock;
import static org.junit.Assert.*;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.easymock.EasyMock.*;

public class UsageProcessorTest {

    private static ContentManager _contentManager;
    private UsageProcessor _processor;
    private final ItemInstance _hand = ItemInstance.NONE;
    private final ItemWrapper _handWrapped = new ContainerItemWrapper(null, -1);

    @BeforeClass
    public static void setUpClass() throws IOException {
        _contentManager = new TestContentLoader().load();
    }

    @Before
    public void setUp() throws IOException {
        _processor = new UsageProcessor(_contentManager);
    }

    @Test
    public void testCanary() {
        assert (true);
    }

    @Test
    public void testValidHandUsageLookup() {
        ItemInstance tool = _hand;
        ItemInstance focus = _contentManager.createItemInstanceByName("Closed Door");
        assertFalse(_processor.findUsages(tool, focus).isEmpty());
    }

    @Test
    public void testInvalidHandUsageLookup() {
        ItemInstance tool = _hand;
        ItemInstance focus = _contentManager.createItemInstanceByName("Axe");
        assertTrue(_processor.findUsages(tool, focus).isEmpty());
    }

    @Test
    public void testValidUsageLookup() {
        ItemInstance tool = _contentManager.createItemInstanceByName("Axe");
        ItemInstance focus = _contentManager.createItemInstanceByName("Tree");
        assertFalse(_processor.findUsages(tool, focus).isEmpty());
    }

    @Test
    public void testInvalidUsageLookup() {
        ItemInstance tool = _contentManager.createItemInstanceByName("Tree");
        ItemInstance focus = _contentManager.createItemInstanceByName("Axe");
        assertTrue(_processor.findUsages(tool, focus).isEmpty());
    }

    private UsageResult meatOnFireUsage(int amountOfMeat) {
        ItemInstance tool = _contentManager.createItemInstanceByName("Meat");
        tool.quantity = amountOfMeat;
        ItemInstance focus = _contentManager.createItemInstanceByName("Fire");
        ItemUse usage = _contentManager.getItemUse(tool.data, focus.data, 0);
        return _processor.getUsageResult(usage, tool, focus);
    }
    
    @Test
    public void testDecreaseToolQuantity() {
        UsageResult result = meatOnFireUsage(10);
        assertEquals(9, result.tool.quantity);
    }

    @Test
    public void testNoDecreaseFocusQuantity() {
        UsageResult result = meatOnFireUsage(1);
        assertEquals(1, result.focus.quantity);
    }
    
    @Test
    public void testToolBecomesEmptyItemWhenQuantityReachesZero() {
        UsageResult result = meatOnFireUsage(1);
        assertEquals(ItemInstance.NONE, result.tool);
    }
    
    private UsageResult axeOnLogUsage(int amountOfLogs) {
        ItemInstance tool = _contentManager.createItemInstanceByName("Axe");
        ItemInstance focus = _contentManager.createItemInstanceByName("Logs");
        focus.quantity = amountOfLogs;
        ItemUse usage = _contentManager.getItemUse(tool.data, focus.data, 0);
        return _processor.getUsageResult(usage, tool, focus);
    }

    @Test
    public void testDecreaseFocusQuantity() {
        UsageResult result = axeOnLogUsage(10);
        assertEquals(9, result.focus.quantity);
    }
    
    @Test
    public void testFocusBecomesEmptyItemWhenQuantityReachesZero() {
        UsageResult result = axeOnLogUsage(1);
        assertEquals(ItemInstance.NONE, result.focus);
    }

    private UsageResult axeOnTreeUsage() {
        ItemInstance tool = _contentManager.createItemInstanceByName("Axe");
        ItemInstance focus = _contentManager.createItemInstanceByName("Tree");
        ItemUse usage = _contentManager.getItemUse(tool.data, focus.data, 0);
        return _processor.getUsageResult(usage, tool, focus);
    }

    @Test
    public void testProducts() {
        UsageResult result = axeOnTreeUsage();
        assertEquals("Stump", result.products.get(0).data.name);
        assertEquals("Logs", result.products.get(1).data.name);
        assertEquals("Branches", result.products.get(2).data.name);
    }

    @Test
    public void testProductQuantities() {
        UsageResult result = axeOnTreeUsage();
        assertEquals(1, result.products.get(0).quantity);
        assertEquals(2, result.products.get(1).quantity);
        assertEquals(5, result.products.get(2).quantity);
    }

    @Test
    public void testContainerToWorldUsageFirstProductGoesToWorld() {
        ItemInstance ripeAppleTree = _contentManager.createItemInstanceByName("Ripe Apple Tree");
        ItemInstance bareAppleTree = _contentManager.createItemInstanceByName("Bare Apple Tree");

        ItemWrapper mockFocusWrapped = createNiceMock(WorldItemWrapper.class);
        expect(mockFocusWrapped.getItemInstance()).andReturn(ripeAppleTree);
        expect(mockFocusWrapped.addItemToSource(bareAppleTree)).andReturn(true);
        replay(mockFocusWrapped);

        ItemUse usage = _contentManager.getItemUse(_hand.data, ripeAppleTree.data, 0);
        _processor.processUsage(usage, _handWrapped, mockFocusWrapped);

        EasyMock.verify(mockFocusWrapped);
    }
}
