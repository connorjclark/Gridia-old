package com.hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import java.lang.reflect.Type;
import org.apache.commons.lang.NotImplementedException;

public class GsonAdapter<T> implements JsonDeserializer<T>, JsonSerializer<T> {

    @Override
    public JsonElement serialize(T item, Type type, JsonSerializationContext jsc) {
        throw new NotImplementedException("No serialize method had been implemented for class: " + type.getTypeName());
    }

    @Override
    public T deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        throw new NotImplementedException("No deserialize method had been implemented for class: " + typeOfT.getTypeName());
    }
}
