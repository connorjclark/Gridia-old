package com.hoten.gridia.serializers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.hoten.gridia.map.Coord;
import java.util.Map;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.junit.Assert.*;

public class EntityGsonAdapterTest {

    private static Gson _gson;

    @BeforeClass
    public static void setUpClass() {
        _gson = new GsonBuilder()
                .registerTypeAdapter(com.hoten.gridia.scripting.Entity.class, new EntityGsonAdapter())
                .create();
    }

    @Test
    public void testSerializeCustomAttribute() {
        com.hoten.gridia.scripting.Entity entity = new com.hoten.gridia.scripting.Entity();
        entity.setAttribute("name", "bill");
        String expected = "{'name':{'class':'java.lang.String','value':'bill'}}".replace('\'', '"');
        assertEquals(expected, _gson.toJson(entity));
    }

    @Test
    public void testSerializeLocation() {
        com.hoten.gridia.scripting.Entity entity = new com.hoten.gridia.scripting.Entity();
        entity.location = new Coord(1, 2, 3);
        String expected = "{'location':{'class':'com.hoten.gridia.map.Coord','value':{'x':1,'y':2,'z':3}}}".replace('\'', '"');
        assertEquals(expected, _gson.toJson(entity));
    }

    @Test
    public void testDeserializeCustomAttribute() {
        String json = "{'name':{'class':'java.lang.String','value':'bill'}}";
        com.hoten.gridia.scripting.Entity entity = _gson.fromJson(json, com.hoten.gridia.scripting.Entity.class);
        assertEquals("bill", entity.getAttribute("name"));
    }

    @Test
    public void testDeserializeLocation() {
        String json = "{'location':{'class':'com.hoten.gridia.map.Coord','value':{'x':1,'y':2,'z':3}}}";
        com.hoten.gridia.scripting.Entity entity = _gson.fromJson(json, com.hoten.gridia.scripting.Entity.class);
        assertEquals(new Coord(1, 2, 3), entity.location);
    }

    @Test
    public void testDeserializeLocationNotInStorage() {
        String json = "{'location':{'class':'com.hoten.gridia.map.Coord','value':{'x':1,'y':2,'z':3}}}";
        com.hoten.gridia.scripting.Entity entity = _gson.fromJson(json, com.hoten.gridia.scripting.Entity.class);
        Map storage = (Map) entity.getAttribute("storage");
        assertFalse(storage.containsKey("location"));
    }

    @Test
    public void testDoesntSerializeContainer() {
        // ...
    }
}
