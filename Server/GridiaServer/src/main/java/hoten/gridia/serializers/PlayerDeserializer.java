package hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;
import hoten.gridia.Player;
import hoten.gridia.Player.PlayerFactory;
import java.lang.reflect.Type;

public class PlayerDeserializer implements JsonDeserializer<Player> {
    
    private final PlayerFactory _playerFactory;
    
    public PlayerDeserializer(PlayerFactory playerFactory) {
        _playerFactory = playerFactory;
    }

    @Override
    public Player deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        Player player = null;
        // do stuff
        return null;
    }
}
