using System;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// Provides a non generic type used for calling the BuildScene method.
    /// </summary>
    public interface ISceneBuilder
    {
        /// <summary>
        /// Main method for scene generation. Creates the scene and adds data visualization.
        /// </summary>
        /// <param name="onSceneBuilt">A callback to be executed once the data visualization is complete.</param>
        public void BuildScene(Action onSceneBuilt = null);
    }
}