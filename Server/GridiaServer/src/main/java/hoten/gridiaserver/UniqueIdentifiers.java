package hoten.gridiaserver;

import java.util.Queue;
import java.util.concurrent.LinkedBlockingQueue;

public class UniqueIdentifiers {

    private int NEXT_NEW_ID;
    private final Queue<Integer> _available = new LinkedBlockingQueue();
    private final int _expandAmount;

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
            _available.add(++NEXT_NEW_ID);
        }
    }
}
