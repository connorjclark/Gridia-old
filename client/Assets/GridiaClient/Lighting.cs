namespace Gridia
{
    using System.Collections.Generic;

    using UnityEngine;

    public class Lighting
    {
        #region Fields

        private readonly List<GameObject> lights;
        private readonly TileMapView _view;

        #endregion Fields

        #region Constructors

        public Lighting(TileMapView view)
        {
            _view = view;
            lights = new List<GameObject>();
        }

        #endregion Constructors

        #region Methods

        public void SetLights(List<Vector3> lights)
        {
            ClearLights();
            lights.ForEach(lightData =>
            {
                int x = (int)lightData.x;
                int y = (int)lightData.y;
                int intensity = (int)lightData.z;
                CreateLight(x, y, intensity);
            });
        }

        private void ClearLights()
        {
            lights.ForEach(light => MonoBehaviour.Destroy(light));
            lights.Clear();
        }

        private void CreateLight(int x, int y, int intensity)
        {
            var position = _view.Focus.Position;
            var TILE_SIZE = 32f * _view.Scale;
            var lightGameObject = new GameObject("a light");
            lightGameObject.AddComponent<Light>();
            var light = lightGameObject.GetComponent<Light>();
            light.type = LightType.Point;
            light.range = 100;
            light.intensity = 1f;
            light.transform.position = new Vector3((x - (position.x % 1) + .5f) * TILE_SIZE, (y - (position.y % 1) + .5f) * TILE_SIZE, -10f);
            lights.Add(lightGameObject);
            //5000 range -100 z 0.5 I
        }

        #endregion Methods
    }
}