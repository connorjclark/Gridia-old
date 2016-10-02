package com.hoten.gridia.serializers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.hoten.gridia.map.Coord;
import java.util.Map;

import com.hoten.gridia.scripting.Entity;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.junit.Assert.*;

public class EntityGsonAdapterTest {

    private static Gson _gson;

    @BeforeClass
    public static void setUpClass() {
        _gson = new GsonBuilder()
                .registerTypeAdapter(Entity.class, new EntityGsonAdapter())
                .create();
    }

    @Test
    public void testSerializeCustomAttribute() {
        Entity entity = new Entity();
        entity.setAttribute("name", "bill");
        String expected = "{'name':{'class':'java.lang.String','value':'bill'}}".replace('\'', '"');
        assertEquals(expected, _gson.toJson(entity));
    }

    @Test
    public void testSerializeLocation() {
        Entity entity = new Entity();
        entity.location = new Coord(1, 2, 3);
        String expected = "{'location':{'class':'com.hoten.gridia.map.Coord','value':{'x':1,'y':2,'z':3}}}".replace('\'', '"');
        assertEquals(expected, _gson.toJson(entity));
    }

    @Test
    public void testDeserializeCustomAttribute() {
        String json = "{'name':{'class':'java.lang.String','value':'bill'}}";
        Entity entity = _gson.fromJson(json, Entity.class);
        assertEquals("bill", entity.getAttribute("name"));
    }

    @Test
    public void testDeserializeLocation() {
        String json = "{'location':{'class':'com.hoten.gridia.map.Coord','value':{'x':1,'y':2,'z':3}}}";
        Entity entity = _gson.fromJson(json, Entity.class);
        assertEquals(new Coord(1, 2, 3), entity.location);
    }

    @Test
    public void testDeserializeLocationNotInStorage() {
        String json = "{'location':{'class':'com.hoten.gridia.map.Coord','value':{'x':1,'y':2,'z':3}}}";
        Entity entity = _gson.fromJson(json, Entity.class);
        Map storage = (Map) entity.getAttribute("storage");
        assertFalse(storage.containsKey("location"));
    }

    @Test
    public void testDoesntSerializeContainer() {
        // ...
    }
}
