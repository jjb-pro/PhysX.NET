﻿using System;
using System.Linq;
using System.Windows.Input;

namespace PhysX.Samples
{
    public abstract class Sample
    {
        // TODO: Refactor how the physics objects are created, they're too hidden away from the individual samples
        protected Sample(Action<SceneDesc> sceneDescCallback = null)
        {
            Engine = new PhysX.Samples.Engine.Engine(sceneDescCallback);
            Engine.OnUpdate += UpdateEngine;
            Engine.OnDraw += (s, e) => DrawEngine();

            LoadContent();
            LoadPhysics(Engine.Scene);
        }

        private void UpdateEngine(TimeSpan elapsed)
        {
            if (Engine.Keyboard.IsKeyDown(Key.Escape))
            {
                Shutdown();
                return;
            }

            UpdateSample(elapsed);
        }
        protected void DrawEngine()
        {
            Draw();
        }

        protected abstract void LoadContent();
        protected abstract void LoadPhysics(Scene scene);
        private void UpdateSample(TimeSpan elapsed)
        {
            ProcessKeyboard(Engine.Keyboard.PressedKeys.ToArray());

            ProcessMouse(Engine.Camera.MouseDelta.X, Engine.Camera.MouseDelta.Y);

            Update(elapsed);
        }
        protected abstract void Update(TimeSpan elapsed);
        protected abstract void Draw();
        protected virtual void ProcessKeyboard(Key[] pressedKeys)
        {

        }
        protected virtual void ProcessMouse(float deltaX, float deltaY)
        {

        }

        protected void Run()
        {
            Engine.Run();
        }
        protected void Shutdown()
        {
            if (Engine.Physics != null && !Engine.Physics.Disposed)
                Engine.Physics.Dispose();

            Environment.Exit(0);
        }

        //

        protected PhysX.Samples.Engine.Engine Engine { get; private set; }

        public TimeSpan FrameTime => Engine.FrameTime;

        public Scene Scene => Engine.Scene;
    }
}