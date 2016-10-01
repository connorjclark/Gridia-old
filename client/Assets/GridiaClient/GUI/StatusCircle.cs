namespace Gridia
{
    using UnityEngine;
    using UnityEngine.UI;

    public class StatusCircle : MonoBehaviour
    {
        #region Fields

        public Text text;

        private float _currentHealth;
        private LineRenderer _lineRenderer;

        #endregion Fields

        #region Constructors

        public StatusCircle()
        {
            ThetaStep = 0.1f;
            Radius = 16;
        }

        #endregion Constructors

        #region Properties

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

        public int MaxHealth
        {
            get; set;
        }

        public float Radius
        {
            get; set;
        }

        public float ThetaStep
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public bool IsActive()
        {
            return CurrentHealth != MaxHealth || MaxHealth != 0;
        }

        public void SetCurrentHealth(float v)
        {
            _currentHealth = v;
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

        public void Update()
        {
            RenderCircle();
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

        #endregion Methods
    }
}