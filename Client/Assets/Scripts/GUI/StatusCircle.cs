using UnityEngine;
using UnityEngine.UI;

namespace Gridia
{
    public class StatusCircle : MonoBehaviour
    {
        private float _currentHealth;

        public void SetCurrentHealth(float v)
        {
            _currentHealth = v;
        }

        public float CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", _currentHealth,
                    "to", value,
                    "time", 0.5,
                    "onupdate",
                    "SetCurrentHealth")
                    );
            }
        }

        public int MaxHealth { get; set; }
        public float Radius { get; set; }
        public float ThetaStep { get; set; }
        private LineRenderer _lineRenderer;
        private Text _text;

        public StatusCircle()
        {
            ThetaStep = 0.1f;
            MaxHealth = 200;
            Radius = 16;
        }

        public void Start()
        {
            //var go = new GameObject();
            //go.transform.parent = gameObject.transform;

            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            _lineRenderer.SetColors(Color.red, Color.red);
            _lineRenderer.SetWidth(Radius*0.4f, Radius*0.4f);
            _lineRenderer.SetVertexCount(Mathf.CeilToInt(2*Mathf.PI/ThetaStep));

            var canvas = Instantiate(Resources.Load("Text")) as GameObject;
            canvas.transform.parent = gameObject.transform;
            _text = canvas.GetComponentInChildren<Text>();
            _text.alignment = TextAnchor.MiddleCenter;
            _text.fontStyle = FontStyle.Bold;
        }

        private void RenderCircle()
        {
            var maxTheta = Mathf.PI*2*(1f*CurrentHealth/MaxHealth);
            var i = 0;
            _lineRenderer.SetVertexCount(Mathf.CeilToInt(maxTheta/ThetaStep));
            _text.text = CurrentHealth + "";
            for (var theta = 0.0f; theta < maxTheta; theta += ThetaStep)
            {
                var x = Radius * Mathf.Cos(theta);
                var y = Radius * Mathf.Sin(theta);
                var pos = new Vector3(x, y, 0);
                _lineRenderer.SetPosition(i++, pos);
            }
        }

        public void Update()
        {
            RenderCircle();
        }
    }
}
