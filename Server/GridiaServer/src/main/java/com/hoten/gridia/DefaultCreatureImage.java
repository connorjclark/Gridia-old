package com.hoten.gridia;

public class DefaultCreatureImage implements CreatureImage {

    private int spriteIndex, width, height;

    public DefaultCreatureImage(int spriteIndex, int width, int height) {
        this.spriteIndex = spriteIndex;
        this.width = width;
        this.height = height;
    }

    public DefaultCreatureImage(int spriteIndex) {
        this(spriteIndex, 1, 1);
    }

    public int getSpriteIndex() {
        return spriteIndex;
    }

    public int getWidth() {
        return width;
    }

    public int getHeight() {
        return height;
    }
}
