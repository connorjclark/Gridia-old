package com.hoten.gridia.serializers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
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
    public void testSerializesCustomAttribute() {
        com.hoten.gridia.scripting.Entity entity = new com.hoten.gridia.scripting.Entity();
        entity.setAttribute("name", "bill");
        String expected = "{\"name\":\"bill\"}";
        assertEquals(expected, _gson.toJson(entity));
    }

    @Test
    public void testDeserializesCustomAttribute() {
        String json = "{\"name\":\"bill\"}";
        com.hoten.gridia.scripting.Entity entity = _gson.fromJson(json, com.hoten.gridia.scripting.Entity.class);
        assertEquals("bill", entity.getAttribute("name"));
    }
}
