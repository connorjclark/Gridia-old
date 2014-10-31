package hoten.gridiaserver;

import java.util.HashSet;
import java.util.Set;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

public class UniqueIdentifiersTest {

    UniqueIdentifiers _uniqueIds;

    @Before
    public void setUp() {
        _uniqueIds = new UniqueIdentifiers(10);
    }

    @After
    public void tearDown() {
    }

    @Test
    public void testTwoIdsAreUnique() {
        assertNotSame(_uniqueIds.next(), _uniqueIds.next());
    }
    
    public Set<Integer> getIds(int amount) {
        Set<Integer> ids = new HashSet();
        for (int i = 0; i < amount; i++) {
            ids.add(_uniqueIds.next());
        }
        return ids;
    }

    @Test
    public void testManyIdsAreUnique() {
        assertEquals(50, getIds(50).size());
    }
    
    @Test
    public void testRetiringIdsShowUpAgain() {
        Set<Integer> originalIds = getIds(20);
        originalIds.forEach(id -> {
            _uniqueIds.retire(id);
        });
        for (int i = 0; i < 200 && !originalIds.isEmpty(); i++) {
            originalIds.remove(_uniqueIds.next());
        }
        assertTrue(originalIds.isEmpty());
    }
}
