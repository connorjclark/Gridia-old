package hoten.gridiaserver;

public class DefaultCreatureImage implements CreatureImage {

    public int spriteIndex;
    public int width;
    public int height;

    public DefaultCreatureImage(int spriteIndex, int width, int height) {
        this.spriteIndex = spriteIndex;
        this.width = width;
        this.height = height;
    }

    public DefaultCreatureImage(int spriteIndex) {
        this(spriteIndex, 1, 1);
    }
}
