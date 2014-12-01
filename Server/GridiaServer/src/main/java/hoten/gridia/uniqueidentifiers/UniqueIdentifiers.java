package hoten.gridia.uniqueidentifiers;

import java.util.Queue;
import java.util.concurrent.LinkedBlockingQueue;

public class UniqueIdentifiers {

    protected int _nextNewId;
    protected int _expandAmount;
    protected final Queue<Integer> _available = new LinkedBlockingQueue();
    
    public UniqueIdentifiers(int expandAmount) {
        _expandAmount = expandAmount;
    }

    public UniqueIdentifiers() {
        this(100);
    }

    public int next() {
        if (_available.isEmpty()) {
            expand();
        }
        return _available.remove();
    }

    public void retire(int id) {
        _available.add(id);
    }
    
    private void expand() {
        for (int i = 0; i < _expandAmount; i++) {
            _available.add(++_nextNewId);
        }
    }
}
