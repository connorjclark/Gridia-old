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
        public Text text;

        public StatusCircle()
        {
            ThetaStep = 0.1f;
            Radius = 16;
        }

        public void Start()
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            _lineRenderer.SetColors(Color.red, Color.red);
            _lineRenderer.SetWidth(Radius*0.4f, Radius*0.4f);
            _lineRenderer.SetVertexCount(Mathf.CeilToInt(2*Mathf.PI/ThetaStep));

            var canvas = Instantiate(Resources.Load("Text")) as GameObject;
            canvas.transform.SetParent(gameObject.transform, false);
            text = canvas.GetComponentInChildren<Text>();
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.text = "";
        }

        public bool IsActive()
        {
            return CurrentHealth != MaxHealth || MaxHealth != 0;
        }

        private void RenderCircle()
        {
            var maxTheta = Mathf.PI*2*(1f*CurrentHealth/MaxHealth);
            var i = 0;
            _lineRenderer.SetVertexCount(Mathf.Max(0, Mathf.CeilToInt(maxTheta/ThetaStep)));
            if (IsActive()) text.text = (int)CurrentHealth + "";
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
