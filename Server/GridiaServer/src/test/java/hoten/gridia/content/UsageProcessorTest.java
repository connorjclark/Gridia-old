package hoten.gridia.content;

import hoten.gridia.ItemWrapper;
import hoten.gridia.content.UsageProcessor.UsageResult;
import java.io.IOException;
import static org.junit.Assert.*;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.easymock.EasyMock.*;

public class UsageProcessorTest {

    private static ContentManager _contentManager;
    private UsageProcessor _processor;
    private final ItemInstance _hand = ItemInstance.NONE;

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
        ItemInstance tool = _contentManager.createItemInstanceByName("Meat", amountOfMeat);
        ItemInstance focus = _contentManager.createItemInstanceByName("Fire");
        ItemUse usage = _contentManager.getItemUse(tool.getItem(), focus.getItem(), 0);
        return _processor.getUsageResult(usage, tool, focus);
    }

    @Test
    public void testDecreaseToolQuantity() {
        UsageResult result = meatOnFireUsage(10);
        ItemInstance expected = _contentManager.createItemInstanceByName("Meat", 9);
        assertEquals(expected, result.tool);
    }

    @Test
    public void testNoDecreaseFocusQuantity() {
        UsageResult result = meatOnFireUsage(1);
        assertEquals(1, result.focus.getQuantity());
    }

    @Test
    public void testToolBecomesEmptyItemWhenQuantityReachesZero() {
        UsageResult result = meatOnFireUsage(1);
        assertEquals(ItemInstance.NONE, result.tool);
    }

    private UsageResult axeOnLogUsage(int amountOfLogs) {
        ItemInstance tool = _contentManager.createItemInstanceByName("Axe");
        ItemInstance focus = _contentManager.createItemInstanceByName("Logs", amountOfLogs);
        ItemUse usage = _contentManager.getItemUse(tool.getItem(), focus.getItem(), 0);
        return _processor.getUsageResult(usage, tool, focus);
    }

    @Test
    public void testDecreaseFocusQuantity() {
        UsageResult result = axeOnLogUsage(10);
        ItemInstance expected = _contentManager.createItemInstanceByName("Logs", 9);
        assertEquals(expected, result.focus);
    }

    @Test
    public void testFocusBecomesEmptyItemWhenQuantityReachesZero() {
        UsageResult result = axeOnLogUsage(1);
        assertEquals(ItemInstance.NONE, result.focus);
    }

    private UsageResult axeOnTreeUsage() {
        ItemInstance tool = _contentManager.createItemInstanceByName("Axe");
        ItemInstance focus = _contentManager.createItemInstanceByName("Tree");
        ItemUse usage = _contentManager.getItemUse(tool.getItem(), focus.getItem(), 0);
        return _processor.getUsageResult(usage, tool, focus);
    }

    @Test
    public void testProducts() {
        UsageResult result = axeOnTreeUsage();
        assertEquals("Stump", result.products.get(0).getItem().name);
        assertEquals("Logs", result.products.get(1).getItem().name);
        assertEquals("Branches", result.products.get(2).getItem().name);
    }

    @Test
    public void testProductQuantities() {
        UsageResult result = axeOnTreeUsage();
        assertEquals(1, result.products.get(0).getQuantity());
        assertEquals(2, result.products.get(1).getQuantity());
        assertEquals(5, result.products.get(2).getQuantity());
    }

    @Test
    public void testContainerToWorldUsageFirstProductGoesToWorldRestToContainer() {
        ItemInstance ripeAppleTree = _contentManager.createItemInstanceByName("Ripe Apple Tree");
        ItemInstance bareAppleTree = _contentManager.createItemInstanceByName("Bare Apple Tree");
        ItemInstance apples = _contentManager.createItemInstanceByName("Apple", 10);
        
        ItemWrapper mockToolWrapped = createMock(ItemWrapper.class);
        expect(mockToolWrapped.getItemInstance()).andReturn(_hand).anyTimes();
        expect(mockToolWrapped.addItemToSource(apples)).andReturn(true);
        replay(mockToolWrapped);

        ItemWrapper mockFocusWrapped = createMock(ItemWrapper.class);
        expect(mockFocusWrapped.getItemInstance()).andReturn(ripeAppleTree);
        /* expect */ mockFocusWrapped.changeWrappedItem(ItemInstance.NONE);
        expectLastCall();
        expect(mockFocusWrapped.addItemToSource(bareAppleTree)).andReturn(true);
        replay(mockFocusWrapped);

        ItemUse usage = _contentManager.getItemUse(_hand.getItem(), ripeAppleTree.getItem(), 0);
        _processor.processUsage(usage, mockToolWrapped, mockFocusWrapped);

        verify(mockFocusWrapped);
    }

    @Test
    public void testToolDoesntChangeWhenIsHand() {
        ItemInstance closedDoor = _contentManager.createItemInstanceByName("Closed Door");
        ItemUse usage = _contentManager.getItemUse(_hand.getItem(), closedDoor.getItem(), 0);

        ItemWrapper mockToolWrapped = createMock(ItemWrapper.class);
        expect(mockToolWrapped.getItemInstance()).andReturn(_hand).anyTimes();
        replay(mockToolWrapped);

        ItemWrapper mockFocusWrapped = createNiceMock(ItemWrapper.class);
        expect(mockFocusWrapped.getItemInstance()).andReturn(closedDoor);
        replay(mockFocusWrapped);

        _processor.processUsage(usage, mockToolWrapped, mockFocusWrapped);
        
        verify(mockToolWrapped);
    }
    
    @Test
    public void testToolWhenNotHandAndFocusChanges() {
        ItemInstance axe = _contentManager.createItemInstanceByName("Axe");
        ItemInstance tree = _contentManager.createItemInstanceByName("Tree");
        ItemUse usage = _contentManager.getItemUse(axe.getItem(), tree.getItem(), 0);

        ItemWrapper mockToolWrapped = createNiceMock(ItemWrapper.class);
        expect(mockToolWrapped.getItemInstance()).andReturn(axe).anyTimes();
        mockToolWrapped.changeWrappedItem(anyObject());
        replay(mockToolWrapped);

        ItemWrapper mockFocusWrapped = createNiceMock(ItemWrapper.class);
        expect(mockFocusWrapped.getItemInstance()).andReturn(tree).anyTimes();
        mockFocusWrapped.changeWrappedItem(anyObject());
        replay(mockFocusWrapped);

        _processor.processUsage(usage, mockToolWrapped, mockFocusWrapped);
        
        verify(mockToolWrapped);
    }
}
