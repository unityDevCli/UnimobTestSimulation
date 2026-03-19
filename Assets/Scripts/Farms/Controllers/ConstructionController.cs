using Farms.Managers;

namespace Farms.Controllers
{
    public class ConstructionController
    {
        private readonly GameManager _gameManager;

        public ConstructionController(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Build(int constructionId)
        {
            var constructionBox = _gameManager.RunTime.constructionBoxes.Find(x => x.constructionId == constructionId);
            if (constructionBox == null || constructionBox.isBuilt) return;
            var plant = _gameManager.BuildSystem.Build(constructionBox, _gameManager.PlantConfig);
            if (plant == null) return;
            _gameManager.RunTime.plants.Add(plant);
            //TODO: Raise event spawn Plant view
        }
    }
}