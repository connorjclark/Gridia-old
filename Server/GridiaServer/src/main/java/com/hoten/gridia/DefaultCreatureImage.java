package com.hoten.gridia;

public class DefaultCreatureImage implements CreatureImage {

    private int _spriteIndex, _width, _height;

    public DefaultCreatureImage(int spriteIndex, int width, int height) {
        _spriteIndex = spriteIndex;
        _width = width;
        _height = height;
    }

    public DefaultCreatureImage(int spriteIndex) {
        this(spriteIndex, 1, 1);
    }

    public int getSpriteIndex() {
        return _spriteIndex;
    }

    public int getWidth() {
        return _width;
    }

    public int getHeight() {
        return _height;
    }
}
