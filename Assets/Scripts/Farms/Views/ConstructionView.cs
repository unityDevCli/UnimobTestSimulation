using Farms.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Farms.Views
{
    public class ConstructionView : MonoBehaviour
    {
        [SerializeField] protected Button buildButton;
        public int constructionId;
        private ConstructionController _controller;

        public void Init(ConstructionController controller)
        {
            _controller = controller;
            buildButton.onClick.AddListener(OnClickBuild);
        }

        private void OnClickBuild()
        {
            _controller.Build(constructionId);
        }
    }
}